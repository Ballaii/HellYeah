using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MusicStarter : MonoBehaviour
{
    public AudioClip[] musicClips;
    private AudioSource audioSource;
    private Coroutine musicCoroutine;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
    }

    /// <summary>
    /// Public method you can call from another script to start music
    /// </summary>
    public void PlayMusicCoroutine()
    {
        if (musicCoroutine == null)
        {
            musicCoroutine = StartCoroutine(PlayMusic());
        }
    }

    /// <summary>
    /// Internal coroutine that plays music clips in order
    /// </summary>
    private IEnumerator PlayMusic()
    {
        foreach (AudioClip clip in musicClips)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
        }

        musicCoroutine = null; // Reset so it can be triggered again if needed
    }
}
