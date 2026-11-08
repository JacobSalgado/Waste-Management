using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    public InputAction pauseButton;

    private string startMenuScene = "Menu";

    private void Awake()
    {
        pauseButton.Enable();
    }

    private void Update()
    {
        if (pauseButton.triggered)
        {
            Debug.Log("Escape Key being pressed!");
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Debug.Log("LOADING MENU...");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(startMenuScene);
    }
}
