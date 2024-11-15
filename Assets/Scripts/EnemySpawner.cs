using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject player; // Set this to the player GameObject in the Inspector

    public List<GameObject> enemyPrefabs;    // List of enemy prefabs to spawn
    public List<GameObject> bugPrefabs;      // List of bug prefabs associated with enemies
    public float minSpawnRadius = 5f;        // Minimum spawn distance from the player
    public float maxSpawnRadius = 5f;        // Maximum spawn distance from the player
    public float minXBound = 5f;             // Minimum X boundary
    public float maxXBound = 5f;             // Maximum X boundary
    public float minYBound = 5f;             // Minimum Y boundary
    public float maxYBound = 5f;             // Maximum Y boundary
    public float playerSafeRadius = 10f;     // Safe radius around the player where enemies won't spawn
    public float spawnInterval = 5f;         // Time interval between spawns
    public int maxSpawnCount = 50;           // Maximum number of enemies allowed at one time
    public int startingSpawnCount;           // Number of enemies to spawn at the start
    public LayerMask obstacleLayer;          // Layer for obstacles to avoid
    public float patrolRadius = 10f;         // Radius for patrol points

    private List<GameObject> activeEnemies = new List<GameObject>(); // List to track active enemies
    private bool playerAlive = true;         // Flag to control spawning while the player is alive
    public float minDistanceBetweenEnemies = 5f; // Minimum distance between enemy spawns

    void Start()
    {
        // Spawn initial enemies at the start
        for (int i = 0; i < startingSpawnCount; i++)
        {
            SpawnRegularEnemy();
        }
        // Start enemy spawning coroutine
        StartCoroutine(ManageEnemySpawning());
    }

    IEnumerator ManageEnemySpawning()
    {
        while (playerAlive)
        {
            if (activeEnemies.Count < maxSpawnCount)
            {
                SpawnRegularEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRegularEnemy()
    {
        if (activeEnemies.Count >= maxSpawnCount) return;

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Vector2 spawnPosition = FindSpreadOutSpawnPosition(minSpawnRadius,maxSpawnRadius,minXBound,maxXBound,minYBound,maxYBound);

        if (spawnPosition != Vector2.zero)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            EnemyUnit enemyComponent = enemy.GetComponent<EnemyUnit>();

            if (enemyComponent != null)
            {
                enemyComponent.playerObject = player; // Assign the player GameObject to access components
                enemyComponent.playerPosition = player.transform; // Assign the player Transform for tracking

                List<GameObject> matchingBugs = bugPrefabs.FindAll(bug => bug.GetComponent<BugFollowPlayer>().strengthLevel == enemyComponent.strengthLevel);
                enemyComponent.AssignBugAlly(matchingBugs);

                Vector2 patrolPoint1 = GeneratePatrolPoint(spawnPosition);
                Vector2 patrolPoint2 = GeneratePatrolPoint(spawnPosition);
                while (!IsPathClear(patrolPoint1, patrolPoint2))
                {
                    patrolPoint1 = GeneratePatrolPoint(spawnPosition);
                    patrolPoint2 = GeneratePatrolPoint(spawnPosition);
                }

                enemyComponent.patrolPoints = new Transform[2]
                {
                    CreatePatrolPointObject(patrolPoint1),
                    CreatePatrolPointObject(patrolPoint2)
                };

                activeEnemies.Add(enemy);

                // Subscribe to the enemy's OnDeath event
                enemyComponent.OnDeath += HandleEnemyDeath;
            }
        }
    }

    Vector2 FindSpreadOutSpawnPosition(float minRadius, float maxRadius, float minX, float maxX, float minY, float maxY)
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            // Generate a random angle in radians
            float angle = Random.Range(0, 2 * Mathf.PI);

            // Randomize the spawn distance within the min and max radius range
            float spawnDistance = Random.Range(minRadius, maxRadius);

            // Calculate the candidate position using the angle and spawn distance
            Vector2 candidatePosition = (Vector2)player.transform.position +
                                        new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnDistance;

            // Clamp the position to stay within the map boundaries
            candidatePosition.x = Mathf.Clamp(candidatePosition.x, minX, maxX);
            candidatePosition.y = Mathf.Clamp(candidatePosition.y, minY, maxY);

            // Check if the position is outside the player safe radius
            if (Vector2.Distance(candidatePosition, player.transform.position) < playerSafeRadius)
            {
                continue;
            }

            // Check if the position is far enough from other enemies and is not blocked by obstacles
            bool isPositionValid = true;
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy != null && Vector2.Distance(candidatePosition, enemy.transform.position) < minDistanceBetweenEnemies)
                {
                    isPositionValid = false;
                    break;
                }
            }

            // Check if the position is not inside an obstacle layer
            if (isPositionValid && !Physics2D.OverlapCircle(candidatePosition, 0.5f, obstacleLayer))
            {
                return candidatePosition;
            }
        }

        return Vector2.zero; // Return Vector2.zero if no valid position is found after maxAttempts
    }

    Vector2 GeneratePatrolPoint(Vector2 spawnPoint)
    {
        Vector2 patrolPoint;
        int maxAttempts = 10;
        int attempts = 0;

        do
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            float distance = Random.Range(1f, patrolRadius);
            patrolPoint = spawnPoint + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            attempts++;
        }
        while ((Vector2.Distance(patrolPoint, player.transform.position) < minSpawnRadius ||
                Physics2D.OverlapCircle(patrolPoint, 0.5f, obstacleLayer)) &&
                attempts < maxAttempts);

        return patrolPoint;
    }

    bool IsPathClear(Vector2 point1, Vector2 point2)
    {
        RaycastHit2D hit = Physics2D.Linecast(point1, point2, obstacleLayer);
        return hit.collider == null;
    }

    Transform CreatePatrolPointObject(Vector2 position)
    {
        GameObject patrolPointObj = new GameObject("PatrolPoint");
        patrolPointObj.transform.position = position;
        return patrolPointObj.transform;
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public void OnPlayerDeath()
    {
        playerAlive = false;
    }
}
