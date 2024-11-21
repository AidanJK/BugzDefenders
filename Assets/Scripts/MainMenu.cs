using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene"); // Replace "GameScene" with the name of your game scene
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); // Only visible in the editor
    }
}
