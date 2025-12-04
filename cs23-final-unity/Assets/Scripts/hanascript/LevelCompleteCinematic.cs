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
    public GameObject nextButton; 
    public GameObject menuButton;  
    
    private CanvasGroup femaleBirdCanvasGroup;
    private CanvasGroup ravenCanvasGroup;
    private Image[] femaleBirdImages;
    private Image[] ravenImages;

    [Header("Audio Sources (drag full AudioSources here)")]
    public AudioSource overlaySource;
    public AudioSource bannerDropSource;
    public AudioSource birdChirpSource;
    public AudioSource victorySource;
    public AudioSource idleSource;

    [Header("Timings")]
    public float overlayFadeTime = 1.0f;
    public float spotlightFadeTime = 1.0f;
    public float bannerDropTime = 1.0f;
    public float birdBounceHeight = 20f;
    public float birdBounceTime = 0.7f;
    public float stepDelay = 0.3f;
    public float idleFloatAmount = 5f;
    public float idleFloatSpeed = 2f;
    public float buttonFadeInTime = 0.5f; 

    [Header("Events")]
    public UnityEvent onSequenceComplete;

    private bool sequenceComplete = false;
    private Vector3 bannerOriginalScale;

    void Start()
    {
        // Setup CanvasGroups
        femaleBirdCanvasGroup = GetOrAddCanvasGroup(femaleBird.gameObject);
        ravenCanvasGroup = GetOrAddCanvasGroup(raven.gameObject);
        
        femaleBirdImages = femaleBird.GetComponentsInChildren<Image>();
        ravenImages = raven.GetComponentsInChildren<Image>();
        
        SetBirdToBlack(femaleBirdImages);
        SetBirdToBlack(ravenImages);
        
        if (confetti != null) confetti.Stop();
        if (confetti1 != null) confetti1.Stop();
        
        darkOverlay.color = new Color(0, 0, 0, 0);
        SetAlpha(spotlight, 0f);
        banner.gameObject.SetActive(false);
        bannerOriginalScale = banner.localScale;
        
        if (nextButton != null) nextButton.SetActive(false);
        if (menuButton != null) menuButton.SetActive(false);

        StartCoroutine(LevelCompleteSequence());
    }
    
    CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }
    
    void SetBirdToBlack(Image[] images)
    {
        foreach (Image img in images)
            img.color = Color.black;
    }
    
    IEnumerator RestoreBirdColors(Image[] images, float duration)
    {
        Color[] originalColors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
            originalColors[i] = Color.white;
        
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            
            for (int i = 0; i < images.Length; i++)
                images[i].color = Color.Lerp(Color.black, originalColors[i], p);
            
            yield return null;
        }
        
        for (int i = 0; i < images.Length; i++)
            images[i].color = originalColors[i];
    }

    IEnumerator LevelCompleteSequence()
    {
        // Play victory music
        if (victorySource != null)
        {
            victorySource.loop = false;
            victorySource.Play();

            if (idleSource != null)
                StartCoroutine(PlayIdleMusicAfter(victorySource.clip.length));
        }

        // 1. Fade in dark overlay
        PlaySound(overlaySource);
        yield return StartCoroutine(FadeImageAlpha(darkOverlay, 0f, 0.6f, overlayFadeTime));
        yield return new WaitForSeconds(stepDelay);

        // 2. Spotlight + reveal birds
        StartCoroutine(FadeImageAlpha(spotlight, 0f, 1f, spotlightFadeTime));
        StartCoroutine(RestoreBirdColors(femaleBirdImages, spotlightFadeTime));
        yield return StartCoroutine(RestoreBirdColors(ravenImages, spotlightFadeTime));
        yield return new WaitForSeconds(stepDelay);

        // 3. Banner drop
        banner.gameObject.SetActive(true);
        banner.localScale = bannerOriginalScale;
        PlaySound(bannerDropSource);
        
        if (confetti != null) confetti.Play();
        if (confetti1 != null) confetti1.Play();
        
        Vector3 originalPos = banner.anchoredPosition;
        Vector3 startPos = originalPos + new Vector3(0, 400, 0);
        banner.anchoredPosition = startPos;
        banner.localScale = bannerOriginalScale * 0.8f;

        float t = 0f;
        while (t < bannerDropTime)
        {
            t += Time.deltaTime;
            float p = t / bannerDropTime;
            
            float ease = ElasticEaseOut(p);
            banner.anchoredPosition = Vector3.Lerp(startPos, originalPos, ease);
            
            float scale = 1f + 0.2f * (1f - p) * Mathf.Sin(p * Mathf.PI * 3f);
            banner.localScale = bannerOriginalScale * Mathf.Clamp(scale, 0.8f, 1.2f);
            
            yield return null;
        }
        
        banner.anchoredPosition = originalPos;
        banner.localScale = bannerOriginalScale;
        yield return new WaitForSeconds(stepDelay);

        // 4. Birds celebrate
        PlaySound(birdChirpSource);
        StartCoroutine(BirdCelebrate(femaleBird, 0f));
        StartCoroutine(BirdCelebrate(raven, 0.1f));
        
        yield return new WaitForSeconds(birdBounceTime + 0.5f);

        // 5. Idle floating animation
        StartCoroutine(IdleFloat(femaleBird));
        StartCoroutine(IdleFloat(raven));

        // 6. Show buttons
        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(FadeInButtons());

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
            bird.anchoredPosition = originalPos + new Vector3(0, birdBounceHeight * bounce, 0);
            
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

    IEnumerator FadeInButtons()
    {
        if (nextButton != null) nextButton.SetActive(true);
        if (menuButton != null) menuButton.SetActive(true);
        
        CanvasGroup nextGroup = nextButton != null ? nextButton.GetComponent<CanvasGroup>() : null;
        CanvasGroup menuGroup = menuButton != null ? menuButton.GetComponent<CanvasGroup>() : null;
        
        if (nextButton != null && nextGroup == null) nextGroup = nextButton.AddComponent<CanvasGroup>();
        if (menuButton != null && menuGroup == null) menuGroup = menuButton.AddComponent<CanvasGroup>();
        
        if (nextGroup != null) nextGroup.alpha = 0f;
        if (menuGroup != null) menuGroup.alpha = 0f;
        
        float t = 0f;
        while (t < buttonFadeInTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / buttonFadeInTime);
            
            if (nextGroup != null) nextGroup.alpha = alpha;
            if (menuGroup != null) menuGroup.alpha = alpha;
            
            yield return null;
        }
        
        if (nextGroup != null) nextGroup.alpha = 1f;
        if (menuGroup != null) menuGroup.alpha = 1f;
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
    
    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    void PlaySound(AudioSource src)
    {
        if (src != null)
            src.Play();
    }

    public void ProceedToNextScreen()
    {
        if (victorySource != null) victorySource.Stop();
        if (idleSource != null) idleSource.Stop();
        Debug.Log("Proceeding to next screen...");
    }
    
    public void ReturnToMenu()
    {
        if (victorySource != null) victorySource.Stop();
        if (idleSource != null) idleSource.Stop();
        Debug.Log("Returning to menu...");
    }

    IEnumerator PlayIdleMusicAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (idleSource != null)
        {
            idleSource.loop = true;
            idleSource.Play();
        }
    }
}
