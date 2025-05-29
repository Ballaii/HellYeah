using UnityEngine;
using TMPro;

public class EnemyCount : MonoBehaviour
{
    public int enemiesKilled = 0;
    public TextMeshProUGUI enemyCountText;

    void Start()
    {
        enemyCountText = GetComponent<TextMeshProUGUI>();
        enemiesKilled = WaveManager.enemiesDead;
    }

    // Update is called once per frame
    void Update()
    {
        enemiesKilled = WaveManager.enemiesDead;
        enemyCountText.text = enemiesKilled.ToString();
    }
}
