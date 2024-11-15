using System.Collections.Generic;
using UnityEngine;

public class PlayerParty : MonoBehaviour
{
    public List<GameObject> partyBugs = new List<GameObject>(); // List of bugs in the player's party
    public float spawnOffsetRadius = 1.5f; // Distance around player to spawn the bug

    public void AddBugToParty(GameObject bugPrefab)
    {
        if (bugPrefab != null)
        {
            // Calculate a random position near the player within the specified radius
            Vector2 spawnOffset = Random.insideUnitCircle * spawnOffsetRadius;
            Vector3 spawnPosition = transform.position + new Vector3(spawnOffset.x, spawnOffset.y, 0);

            // Instantiate the bug at the calculated position
            GameObject newBug = Instantiate(bugPrefab, spawnPosition, Quaternion.identity);

            // Add the bug to the party list without making it a child
            partyBugs.Add(newBug);
            Debug.Log("Bug added to the party: " + newBug.name);

            // Optionally, set the player as the target for the bug
            BugFollowPlayer bugFollowScript = newBug.GetComponent<BugFollowPlayer>();
            if (bugFollowScript != null)
            {
                bugFollowScript.player = transform; // Assign the player as the target to follow
            }
        }
        else
        {
            Debug.LogWarning("Bug prefab is missing.");
        }
    }
}
