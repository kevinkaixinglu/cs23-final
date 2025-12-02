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
    public GameObject nextButton; // Next/Continue button
    public GameObject menuButton;  // Menu button
    
    private CanvasGroup femaleBirdCanvasGroup;
    private CanvasGroup ravenCanvasGroup;
    private Image[] femaleBirdImages;
    private Image[] ravenImages;

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
    public float buttonFadeInTime = 0.5f; // Time for buttons to fade in

    [Header("Events")]
    public UnityEvent onSequenceComplete;

    private bool sequenceComplete = false;
    private Vector3 bannerOriginalScale;

    void Start()
    {
        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // Setup CanvasGroups for bird fading
        femaleBirdCanvasGroup = GetOrAddCanvasGroup(femaleBird.gameObject);
        ravenCanvasGroup = GetOrAddCanvasGroup(raven.gameObject);
        
        // Get all images from birds (including children)
        femaleBirdImages = femaleBird.GetComponentsInChildren<Image>();
        ravenImages = raven.GetComponentsInChildren<Image>();
        
        // Start birds as black silhouettes
        SetBirdToBlack(femaleBirdImages);
        SetBirdToBlack(ravenImages);
        
        // Stop confetti initially
        if (confetti != null) confetti.Stop();
        if (confetti1 != null) confetti1.Stop();
        
        // Start invisible
        darkOverlay.color = new Color(0, 0, 0, 0);
        SetAlpha(spotlight, 0f);
        banner.gameObject.SetActive(false);
        bannerOriginalScale = banner.localScale;
        
        // Hide buttons initially
        if (nextButton != null) nextButton.SetActive(false);
        if (menuButton != null) menuButton.SetActive(false);

        StartCoroutine(LevelCompleteSequence());
    }
    
    CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = obj.AddComponent<CanvasGroup>();
        }
        return cg;
    }
    
    void SetBirdToBlack(Image[] images)
    {
        foreach (Image img in images)
        {
            img.color = Color.black;
        }
    }
    
    IEnumerator RestoreBirdColors(Image[] images, float duration)
    {
        // Store original colors
        Color[] originalColors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            // Get the original color from the sprite or use white as default
            originalColors[i] = Color.white; 
        }
        
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = Color.Lerp(Color.black, originalColors[i], p);
            }
            
            yield return null;
        }
        
        // Ensure final colors are set
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = originalColors[i];
        }
    }

    IEnumerator LevelCompleteSequence()
    {
        // Play victory music
        if (victoryMusic != null)
        {
            audioSource.clip = victoryMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 1. Fade in dark overlay
        PlaySound(overlaySound);
        yield return StartCoroutine(FadeImageAlpha(darkOverlay, 0f, 0.6f, overlayFadeTime));
        yield return new WaitForSeconds(stepDelay);

        // 2. Fade in spotlight and reveal birds
        StartCoroutine(FadeImageAlpha(spotlight, 0f, 1f, spotlightFadeTime));
        StartCoroutine(RestoreBirdColors(femaleBirdImages, spotlightFadeTime));
        yield return StartCoroutine(RestoreBirdColors(ravenImages, spotlightFadeTime));
        yield return new WaitForSeconds(stepDelay);

        // 3. Banner drop with bounce
        banner.gameObject.SetActive(true);
        banner.localScale = bannerOriginalScale; // Ensure correct scale
        PlaySound(bannerDropSound);
        
        // Trigger confetti bursts
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
            
            // Elastic ease-out with overshoot
            float ease = ElasticEaseOut(p);
            banner.anchoredPosition = Vector3.Lerp(startPos, originalPos, ease);
            
            // Scale pop
            float scale = 1f + 0.2f * (1f - p) * Mathf.Sin(p * Mathf.PI * 3f);
            banner.localScale = bannerOriginalScale * Mathf.Clamp(scale, 0.8f, 1.2f);
            
            yield return null;
        }
        
        banner.anchoredPosition = originalPos;
        banner.localScale = bannerOriginalScale;
        yield return new WaitForSeconds(stepDelay);

        // 4. Birds celebrate
        PlaySound(birdChirpSound);
        StartCoroutine(BirdCelebrate(femaleBird, 0f));
        StartCoroutine(BirdCelebrate(raven, 0.1f));
        
        yield return new WaitForSeconds(birdBounceTime + 0.5f);

        // 5. Idle floating animation
        StartCoroutine(IdleFloat(femaleBird));
        StartCoroutine(IdleFloat(raven));

        // 6. Show buttons after a brief moment
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
        
        // Smooth celebratory dance
        float t = 0f;
        while (t < birdBounceTime)
        {
            t += Time.deltaTime;
            float p = t / birdBounceTime;
            
            // Smooth bounce curve
            float bounce = Mathf.Sin(p * Mathf.PI);
            Vector3 offset = new Vector3(0, birdBounceHeight * bounce, 0);
            bird.anchoredPosition = originalPos + offset;
            
            // Gentle sway rotation
            float rotation = Mathf.Sin(p * Mathf.PI * 2f) * 8f;
            bird.localRotation = Quaternion.Euler(0, 0, rotation);
            
            // Subtle scale pulse
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
        // Activate buttons
        if (nextButton != null) nextButton.SetActive(true);
        if (menuButton != null) menuButton.SetActive(true);
        
        // Get CanvasGroup components or add them
        CanvasGroup nextGroup = nextButton != null ? nextButton.GetComponent<CanvasGroup>() : null;
        CanvasGroup menuGroup = menuButton != null ? menuButton.GetComponent<CanvasGroup>() : null;
        
        // Add CanvasGroup if missing
        if (nextButton != null && nextGroup == null) nextGroup = nextButton.AddComponent<CanvasGroup>();
        if (menuButton != null && menuGroup == null) menuGroup = menuButton.AddComponent<CanvasGroup>();
        
        // Start from transparent
        if (nextGroup != null) nextGroup.alpha = 0f;
        if (menuGroup != null) menuGroup.alpha = 0f;
        
        // Fade in
        float t = 0f;
        while (t < buttonFadeInTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / buttonFadeInTime);
            
            if (nextGroup != null) nextGroup.alpha = alpha;
            if (menuGroup != null) menuGroup.alpha = alpha;
            
            yield return null;
        }
        
        // Ensure fully visible
        if (nextGroup != null) nextGroup.alpha = 1f;
        if (menuGroup != null) menuGroup.alpha = 1f;
    }

    // Elastic ease-out for bouncy effect
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

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Call this from a button or after a delay
    public void ProceedToNextScreen()
    {
        // Load next scene, show results, etc.
        Debug.Log("Proceeding to next screen...");
    }
    
    public void ReturnToMenu()
    {
        Debug.Log("Returning to menu...");
        // Load menu scene
    }
}