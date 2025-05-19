using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;
    public static bool paused = false;

    void Start()
    {
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameOver.dead) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        menu.SetActive(true);
        Time.timeScale = 0;
        paused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    ////////////////////////////////
    ////// Pause Menu Buttons //////
    ////////////////////////////////

    public void Resume()
    { 
        menu.SetActive(false);
        Time.timeScale = 1;
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Settings()
    {

    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
