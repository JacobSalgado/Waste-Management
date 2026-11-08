using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    public InputAction pauseButton;

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

    void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
