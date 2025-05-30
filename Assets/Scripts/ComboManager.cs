using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using Unity.Cinemachine;

public class ComboManager : MonoBehaviour
{
    [Header("Combo Settings")]
    public float comboResetTime = 1.5f;

    [Header("UI References")]
    public CanvasGroup comboCanvasGroup;
    public TextMeshProUGUI comboText;
    public Animator screenFlashAnimator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip doubleKillClip;
    public AudioClip tripleKillClip;
    public AudioClip megaKillClip;
    public AudioClip ultraKillClip;
    public AudioClip godlikeClip;

    [Header("Effects")]
    public CinemachineImpulseSource impulseSource;
    public PostProcessVolume postProcessVolume;

    [Header("Events")]
    public UnityEvent onComboTriggered;

    private int comboCount = 0;
    private float lastKillTime = float.NegativeInfinity;
    private Coroutine fadeCoroutine;

    private ChromaticAberration chromatic;

    private Coroutine comboTriggerCoroutine;

    private void Start()
    {
        ResetCombo();
        if (comboCanvasGroup != null)
            comboCanvasGroup.alpha = 0f;

        postProcessVolume.profile.TryGetSettings(out chromatic);
        if (chromatic != null)
            chromatic.intensity.value = 0f;
    }

    

    public void RegisterKill()
    {
          comboCount = (Time.time - lastKillTime <= comboResetTime) ? comboCount + 1 : 1;
        lastKillTime = Time.time;

        // Restart combo trigger timer
        if (comboTriggerCoroutine != null)
        StopCoroutine(comboTriggerCoroutine);

        comboTriggerCoroutine = StartCoroutine(WaitAndTriggerCombo());
    }

    private IEnumerator WaitAndTriggerCombo()
    {
        yield return new WaitForSeconds(comboResetTime);

        if (comboCount >= 2)
            TriggerCombo();  // Waited long enough without another kill

        ResetCombo();  // After firing the combo effect, reset state
    }

    private void TriggerCombo()
    {
        PlayComboSound();
        AnimateFlash();
        ShakeScreen();
        TriggerPostFX();
        ShowComboText();

        onComboTriggered?.Invoke();

        comboCount = 0; // Reset to allow new combos
    }

    private void PlayComboSound()
    {
        if (audioSource == null) return;

        AudioClip clip = null;
        switch (comboCount)
        {
            case 2: clip = doubleKillClip; break;
            case 3: clip = tripleKillClip; break;
            case 4: clip = megaKillClip; break;
            case 5: clip = ultraKillClip; break;
            default:
                if (comboCount >= 6)
                    clip = godlikeClip;
                break;
        }

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void AnimateFlash()
    {
        if (screenFlashAnimator != null)
            screenFlashAnimator.SetTrigger("Flash");
    }

    private void ShakeScreen()
    {
        if (impulseSource != null)
            impulseSource.GenerateImpulse();
    }

    private void TriggerPostFX()
    {
        if (chromatic == null) return;

        StopAllCoroutines(); // in case already animating
        StartCoroutine(PostFXFlash());
    }

    private IEnumerator PostFXFlash()
    {
        float t = 0f;
        while (t < 0.2f)
        {
            chromatic.intensity.value = Mathf.Lerp(0f, 1f, t / 0.2f);
            t += Time.deltaTime;
            yield return null;
        }
        chromatic.intensity.value = 1f;

        yield return new WaitForSeconds(0.3f);

        t = 0f;
        while (t < 0.5f)
        {
            chromatic.intensity.value = Mathf.Lerp(1f, 0f, t / 0.5f);
            t += Time.deltaTime;
            yield return null;
        }
        chromatic.intensity.value = 0f;
    }

    private void ShowComboText()
    {
        if (comboCanvasGroup == null || comboText == null) return;

        comboText.text = GetComboLabel(comboCount);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeComboUI());
    }

    private string GetComboLabel(int count)
    {
        switch (count)
        {
            case 2: return "DOUBLE KILL!";
            case 3: return "TRIPLE KILL!";
            case 4: return "MEGA KILL!";
            case 5: return "ULTRAKILL!";
            default:
                return count >= 6 ? "GODLIKE!" : $"KILL COMBO ×{count}!";
        }
    }

    private IEnumerator FadeComboUI()
    {
        float t = 0f;
        while (t < 0.2f)
        {
            comboCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / 0.2f);
            t += Time.deltaTime;
            yield return null;
        }
        comboCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(0.6f);

        t = 0f;
        while (t < 0.5f)
        {
            comboCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
            t += Time.deltaTime;
            yield return null;
        }
        comboCanvasGroup.alpha = 0f;
    }

    private void ResetCombo()
    {
        comboCount = 0;
        if (comboCanvasGroup != null)
            comboCanvasGroup.alpha = 0f;
    }
}
