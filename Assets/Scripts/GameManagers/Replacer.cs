using UnityEngine;

public class Replacer : MonoBehaviour
{

        [Header("References")]
        public GameObject Statue;
        public AudioClip spawnClip;
        public AudioSource source;

  // Update is called once per frame
  void Awake()
  {
    Statue.SetActive(false);
        source.PlayOneShot(spawnClip);
      }
}
