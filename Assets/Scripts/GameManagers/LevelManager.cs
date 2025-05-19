using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Lock Icons")]
    public GameObject museumLockIcon;
    public GameObject bikerLockIcon;
    public GameObject gymLockIcon;
    public GameObject constructionLockIcon;
    public GameObject idkLockIcon;
    public GameObject hunterLockIcon;
    public GameObject manCaveLockIcon;
    public GameObject bossLockIcon;


    // Dictionary to store lock status per level
    public Dictionary<string, bool> levelLocks = new Dictionary<string, bool>();

    private void Start()
    {
// Initialize level locks
        levelLocks["Bar"] = false;          // Always unlocked
        levelLocks["Museum"] = false;
        levelLocks["Biker"] = true;
        levelLocks["Gym"] = true;
        levelLocks["Construction"] = true;
        levelLocks["Idk"] = true;
        levelLocks["Hunter"] = true;
        levelLocks["ManCave"] = true;
        levelLocks["Boss"] = true;

        // TODO: Load level locks from Player
        UpdateLevelLocksUI();
    }

    public void LoadLevel(string levelName)
    {
        if (levelLocks.ContainsKey(levelName) && levelLocks[levelName])
        {
            Debug.Log($"{levelName} is locked!");
            return;
        }

        SceneManager.LoadScene(levelName);
    }

    private void UpdateLevelLocksUI()
    {
        museumLockIcon.SetActive(levelLocks["Museum"]);
        bikerLockIcon.SetActive(levelLocks["Biker"]);
        gymLockIcon.SetActive(levelLocks["Gym"]);
        constructionLockIcon.SetActive(levelLocks["Construction"]);
        idkLockIcon.SetActive(levelLocks["Idk"]);
        hunterLockIcon.SetActive(levelLocks["Hunter"]);
        manCaveLockIcon.SetActive(levelLocks["ManCave"]);
        bossLockIcon.SetActive(levelLocks["Boss"]);
    }


    // These functions can be called by buttons in the UI
    public void LoadBar() => LoadLevel("Bar");
    public void LoadMuseum() => LoadLevel("Museum");
    public void LoadBiker() => LoadLevel("Biker");
    public void LoadGym() => LoadLevel("Gym");
    public void LoadConstruction() => LoadLevel("Construction");
    public void LoadIdk() => LoadLevel("Idk");
    public void LoadHunter() => LoadLevel("Hunter");
    public void LoadManCave() => LoadLevel("ManCave");
    public void LoadBoss() => LoadLevel("Boss");
}
