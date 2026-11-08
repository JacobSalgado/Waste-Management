using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    [SerializeField] private Pause pauseScript;

    public void PlayGame()  
    {
        //pauseScript.ResumeGame();
        Pause.GameIsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("User Quits!!!");
        Application.Quit();     
    }
}
