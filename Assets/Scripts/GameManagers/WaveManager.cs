using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static int currentWave = 0;
    public static int enemiesDead = 0;
    
    public int[] wavesEnemiesCount; // Example counts for each wave
    public GameObject[] waves;

    void Start()
    {
        waves[currentWave].SetActive(true);
    }

      private void OnGUI()
      {
            GUI.Label(new Rect(500, 10, 200, 20), "Wave: " + (currentWave + 1));
            //GUI.Label(new Rect(500, 30, 200, 20), "Enemies Dead: " + enemiesDead);
      }

      private void Update(){
        //Show wave number on screen

        // only do the 1→2 transition if we're still on wave 0
        if (currentWave < waves.Length && enemiesDead == wavesEnemiesCount[currentWave])
        {
        waves[currentWave].SetActive(false);
        currentWave++;

            if (currentWave < waves.Length)
            {
            waves[currentWave].SetActive(true);
            }
            else
             {
            // All waves completed
            Debug.Log("All waves complete!");
            // Trigger win condition, reset, etc.
            }
        }
    }

}
