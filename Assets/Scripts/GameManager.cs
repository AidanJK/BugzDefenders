using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using UnityEngine.UI; // Required for UI elements

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject deathScreenUI; // Reference to the death screen UI
    public TextMeshProUGUI scoreText; // Reference to the score display in the UI
    public TextMeshProUGUI finalScoreText; // Reference to the final score display on the death screen

    private int score = 0; // Tracks the player's score

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Persist GameManager across scenes
    }

    private void Start()
    {
        // Ensure the death screen is initially hidden
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }
        else
        {
            Debug.LogError("DeathScreenUI reference is missing in GameManager.");
        }

        UpdateScoreUI(); // Initialize the score display
    }

    // Method to add points to the score
    public void AddScore(int points)
    {
        score += points; // Increment the score
        UpdateScoreUI(); // Update the UI to reflect the new score
    }

    // Updates the score UI element
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score; // Update the text component
        }
    }

    // Method to handle player death
    public void PlayerDied()
    {
        // Activate the death screen UI
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }

        // Display the final score on the death screen
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + score;
        }

        // Optionally, pause the game
        Time.timeScale = 0f; // Pause the game
    }

    // Method to restart the current level
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Resume time in case it was paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }

    // Method to quit the game (only works in a built application)
    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}

