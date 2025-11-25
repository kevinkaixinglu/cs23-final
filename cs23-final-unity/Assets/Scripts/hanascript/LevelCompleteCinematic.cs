using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class LevelCompleteCinematic : MonoBehaviour
{
    [Header("References")]
    public Image darkOverlay;
    public Image spotlight;
    public RectTransform banner;
    public RectTransform femaleBird;
    public RectTransform raven;
    public ParticleSystem confetti;
    public ParticleSystem confetti1;

    [Header("Audio (Optional)")]
    public AudioClip overlaySound;
    public AudioClip bannerDropSound;
    public AudioClip birdChirpSound;
    public AudioClip victoryMusic;
    private AudioSource audioSource;

    [Header("Timings")]
    public float overlayFadeTime = 1.0f;
    public float spotlightFadeTime = 1.0f;
    public float bannerDropTime = 1.0f;
    public float birdBounceHeight = 20f;
    public float birdBounceTime = 0.7f;
    public float stepDelay = 0.3f;
    public float idleFloatAmount = 5f;
    public float idleFloatSpeed = 2f;

    [Header("Events")]
    public UnityEvent onSequenceComplete;

    private bool sequenceComplete = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        
        if (confetti != null) confetti.Stop();
        if (confetti1 != null) confetti1.Stop();
        
        darkOverlay.color = new Color(0, 0, 0, 0);
        SetAlpha(spotlight, 0f);
        banner.gameObject.SetActive(false);
        banner.localScale = Vector3.one;

        StartCoroutine(LevelCompleteSequence());
    }

    IEnumerator LevelCompleteSequence()
    {
        if (victoryMusic != null)
        {
            audioSource.clip = victoryMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        PlaySound(overlaySound);
        yield return StartCoroutine(FadeImageAlpha(darkOverlay, 0f, 0.6f, overlayFadeTime));
        yield return new WaitForSeconds(stepDelay);

        yield return StartCoroutine(FadeImageAlpha(spotlight, 0f, 1f, spotlightFadeTime));
        yield return new WaitForSeconds(stepDelay);

        banner.gameObject.SetActive(true);
        PlaySound(bannerDropSound);
        
        if (confetti != null) confetti.Play();
        if (confetti1 != null) confetti1.Play();
        
        Vector3 originalPos = banner.anchoredPosition;
        Vector3 startPos = originalPos + new Vector3(0, 400, 0);
        banner.anchoredPosition = startPos;
        banner.localScale = Vector3.one * 0.8f;

        float t = 0f;
        while (t < bannerDropTime)
        {
            t += Time.deltaTime;
            float p = t / bannerDropTime;
            
            float ease = ElasticEaseOut(p);
            banner.anchoredPosition = Vector3.Lerp(startPos, originalPos, ease);
            
            float scale = 1f + 0.2f * (1f - p) * Mathf.Sin(p * Mathf.PI * 3f);
            banner.localScale = Vector3.one * Mathf.Clamp(scale, 0.8f, 1.2f);
            
            yield return null;
        }
        
        banner.anchoredPosition = originalPos;
        banner.localScale = Vector3.one;
        yield return new WaitForSeconds(stepDelay);

        PlaySound(birdChirpSound);
        StartCoroutine(BirdCelebrate(femaleBird, 0f));
        StartCoroutine(BirdCelebrate(raven, 0.1f));
        
        yield return new WaitForSeconds(birdBounceTime + 0.5f);

        StartCoroutine(IdleFloat(femaleBird));
        StartCoroutine(IdleFloat(raven));

        sequenceComplete = true;
        onSequenceComplete?.Invoke();
    }

    IEnumerator BirdCelebrate(RectTransform bird, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Vector3 originalPos = bird.anchoredPosition;
        Vector3 originalScale = bird.localScale;
        
        float t = 0f;
        while (t < birdBounceTime)
        {
            t += Time.deltaTime;
            float p = t / birdBounceTime;
            
            float bounce = Mathf.Sin(p * Mathf.PI);
            Vector3 offset = new Vector3(0, birdBounceHeight * bounce, 0);
            bird.anchoredPosition = originalPos + offset;
            
            float rotation = Mathf.Sin(p * Mathf.PI * 2f) * 8f;
            bird.localRotation = Quaternion.Euler(0, 0, rotation);
            
            float scale = 1f + 0.1f * Mathf.Sin(p * Mathf.PI);
            bird.localScale = originalScale * scale;
            
            yield return null;
        }
        
        bird.anchoredPosition = originalPos;
        bird.localRotation = Quaternion.identity;
        bird.localScale = originalScale;
    }

    IEnumerator IdleFloat(RectTransform bird)
    {
        Vector3 basePos = bird.anchoredPosition;
        float randomOffset = Random.Range(0f, Mathf.PI * 2f);
        
        while (sequenceComplete)
        {
            float t = Time.time * idleFloatSpeed + randomOffset;
            float yOffset = Mathf.Sin(t) * idleFloatAmount;
            bird.anchoredPosition = basePos + new Vector3(0, yOffset, 0);
            yield return null;
        }
    }

    float ElasticEaseOut(float t)
    {
        if (t == 0f || t == 1f) return t;
        
        float p = 0.3f;
        float s = p / 4f;
        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - s) * (2f * Mathf.PI) / p) + 1f;
    }

    void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    IEnumerator FadeImageAlpha(Image img, float from, float to, float duration)
    {
        float t = 0f;
        Color c = img.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            img.color = c;
            yield return null;
        }
        c.a = to;
        img.color = c;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void ProceedToNextScreen()
    {
        Debug.Log("Proceeding to next screen...");
    }
}