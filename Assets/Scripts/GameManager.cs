using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using UnityEngine.UI; // Required for UI elements

public class GameManager : MonoBehaviour
{
    public GameObject deathScreenUI; // Reference to the death screen UI

    void Start()
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
    }

    // Method to handle player death
    public void PlayerDied()
    {
        // Activate the death screen UI
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
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

