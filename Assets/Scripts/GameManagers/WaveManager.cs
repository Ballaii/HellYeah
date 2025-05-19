using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static int currentWave = 0;
    public static int enemiesDead = 0;
    public int IstWave;
    public int IIndwave;
    public int IIIrdWave;
    public GameObject[] waves;

    void Start()
    {
        waves[currentWave].SetActive(true);
    }

      private void OnGUI()
      {
            GUI.Label(new Rect(500, 10, 200, 20), "Wave: " + (currentWave + 1));
            GUI.Label(new Rect(500, 30, 200, 20), "Enemies Dead: " + enemiesDead);
      }

      private void Update()
    {
        //Show wave number on screen

        // only do the 1→2 transition if we're still on wave 0
        if (currentWave == 0 && enemiesDead == IstWave)
        {
            waves[0].SetActive(false);
            currentWave = 1;
            waves[1].SetActive(true);
        }
        // only do the 2→3 transition if we're still on wave 1
        else if (currentWave == 1 && enemiesDead == IIndwave)
        {
            waves[1].SetActive(false);
            currentWave = 2;
            waves[2].SetActive(true);
        }
        // and so on…
        else if (currentWave == 2 && enemiesDead == IIIrdWave)
        {
            waves[2].SetActive(false);
            currentWave = 3;
            waves[3].SetActive(true);
        }
    }

}
