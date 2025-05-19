using System.Collections;
using UnityEngine;

public class Replacer : MonoBehaviour
{

        [Header("References")]
        public GameObject statue;
        public AudioClip spawnClip;
        public AudioSource source;
        public ParticleSystem spawnParticle;
        public GameObject enemyPrefab;

  // Update is called once per frame
  void Awake()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

  IEnumerator SpawnEnemyRoutine()
  {
    // Hide statue

    if (statue != null)
      statue.SetActive(false);

    // Play particle
    ParticleSystem effect = null;
    if (spawnParticle != null)
    {
      effect = Instantiate(spawnParticle, transform.position, Quaternion.identity);
      if (effect != null) effect.Play();
    }

    // Play sound once
    if (source != null && spawnClip != null)
      source.PlayOneShot(spawnClip);

    // Wait for 2 seconds
    yield return new WaitForSeconds(1.2f);

    // Spawn enemy
    if (enemyPrefab != null)
    {
      GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

    // Clean up particle
    if (effect != null)
      Destroy(effect.gameObject);
  }
}
