using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject settings;
    public GameObject levelsselect;

    private void Start()
    {
     menu.SetActive(false);
     settings.SetActive(false);
     levelsselect.SetActive(false);
        menu.SetActive(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Play()
    {
        levelsselect.SetActive(true);
        menu.SetActive(false);
    }

    public void Settings()
    {
        menu.SetActive(false);
        settings.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
