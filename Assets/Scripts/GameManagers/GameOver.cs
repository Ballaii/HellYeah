using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static bool dead = false;

    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void GameOverScreen()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        dead = true;
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        dead = false;
        Time.timeScale = 1;
        gameObject.SetActive(false);
        WaveManager.enemiesDead = 0;
        WaveManager.currentWave = 0;

    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        dead = false;
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
