using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class LevelFailedCinematic : MonoBehaviour
{
    [Header("References")]
    public Image darkOverlay;
    public RectTransform banner;    // "TRY AGAIN" banner
    public RectTransform raven;
    public RectTransform rainCloud; // sad rain cloud sprite
    public ParticleSystem rainParticles; // rain effect
    public Transform canvasRoot; // for screen shake effect (can be Canvas or any parent)

    private Image[] ravenImages;

    [Header("Audio (Optional)")]
    public AudioClip sadSound;
    public AudioClip bannerThudSound;
    public AudioClip birdSighSound;
    public AudioClip melancholyMusic;
    private AudioSource audioSource;

    [Header("Timings")]
    public float overlayFadeTime = 1.5f;
    public float bannerDropTime = 0.8f;
    public float birdSlumpAmount = 15f;
    public float birdSlumpTime = 1.0f;
    public float stepDelay = 0.5f;
    public float idleSadSwayAmount = 3f;
    public float idleSadSwaySpeed = 0.5f;
    public float idleRockAngle = 8f; // side-to-side rocking angle
    public float idleRockSpeed = 1.2f; // rocking speed
    public float cloudYOffset = 80f;    // cloud position above bird
    
    [Header("Effects")]
    public bool useScreenShake = true;
    public bool useDesaturation = true;
    public float screenShakeIntensity = 10f;
    public float screenShakeDuration = 0.3f;
    public float desaturationDuration = 2.0f;

    [Header("Events")]
    public UnityEvent onSequenceComplete;

    private bool sequenceComplete = false;
    private Vector2 cloudBasePosition;
    private Vector3 bannerOriginalScale;

    void Start()
    {
        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // Get all images from raven (including children)
        ravenImages = raven.GetComponentsInChildren<Image>();
        
        // Start bird as black silhouette
        SetBirdToBlack(ravenImages);
        
        // Stop rain initially
        if (rainParticles != null) rainParticles.Stop();
        
        // Hide cloud initially
        if (rainCloud != null)
        {
            SetAlphaForRectTransform(rainCloud, 0f);
            // Position cloud above the raven
            cloudBasePosition = raven.anchoredPosition + new Vector2(0, cloudYOffset);
            rainCloud.anchoredPosition = cloudBasePosition;
        }
        
        // Start invisible
        darkOverlay.color = new Color(0, 0, 0, 0);
        banner.gameObject.SetActive(false);
        bannerOriginalScale = banner.localScale; // Store original scale

        StartCoroutine(LevelFailedSequence());
    }

    void SetBirdToBlack(Image[] images)
    {
        foreach (Image img in images)
        {
            img.color = Color.black;
        }
    }

    IEnumerator RestoreBirdColorsDesaturated(Image[] images, float duration)
    {
        // Restore to desaturated, grayish colors for sad mood
        Color[] targetColors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            // Desaturated gray-blue for sad appearance
            targetColors[i] = new Color(0.55f, 0.55f, 0.6f, 1f);
        }
        
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = Color.Lerp(Color.black, targetColors[i], p);
            }
            
            yield return null;
        }
        
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = targetColors[i];
        }
    }

    IEnumerator LevelFailedSequence()
    {
        // Play melancholy music
        if (melancholyMusic != null)
        {
            audioSource.clip = melancholyMusic;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }

        // 1. Fade in dark overlay
        PlaySound(sadSound);
        yield return StartCoroutine(FadeImageAlpha(darkOverlay, 0f, 0.75f, overlayFadeTime));
        yield return new WaitForSeconds(stepDelay);

        // 2. Reveal bird and cloud together
        StartCoroutine(RestoreBirdColorsDesaturated(ravenImages, 1.0f));
        if (rainCloud != null)
        {
            StartCoroutine(FadeRectTransform(rainCloud, 0f, 1f, 1.0f));
        }
        yield return new WaitForSeconds(1.0f); // Wait for both to complete
        yield return new WaitForSeconds(0.2f); // Shorter delay before banner

        // 3. Banner drops heavily (no bounce, just thuds down)
        banner.gameObject.SetActive(true);
        banner.localScale = bannerOriginalScale; // Ensure correct scale
        PlaySound(bannerThudSound);
        
        Vector2 originalPos = banner.anchoredPosition;
        Vector2 startPos = originalPos + new Vector2(0, 450);
        banner.anchoredPosition = startPos;

        float t = 0f;
        while (t < bannerDropTime)
        {
            t += Time.deltaTime;
            float p = t / bannerDropTime;
            
            // Dramatic heavy drop - fast fall with hard stop
            float ease = Mathf.Pow(p, 3f); // accelerates downward
            banner.anchoredPosition = Vector2.Lerp(startPos, originalPos, ease);
            
            yield return null;
        }
        
        banner.anchoredPosition = originalPos;
        
        // Screen shake on impact
        if (useScreenShake && canvasRoot != null)
        {
            StartCoroutine(ScreenShake());
        }
        
        // Start rain when banner lands
        if (rainParticles != null) rainParticles.Play();
        
        // Start desaturation effect
        if (useDesaturation)
        {
            StartCoroutine(DesaturateScene());
        }
        
        yield return new WaitForSeconds(stepDelay);

        // 4. Bird slumps down disappointedly
        PlaySound(birdSighSound);
        StartCoroutine(BirdSlump(raven, 0f));
        
        yield return new WaitForSeconds(birdSlumpTime + 0.5f);

        // 5. Idle sad sway with cloud gently moving
        StartCoroutine(IdleSadSway(raven));
        StartCoroutine(CloudIdleDrift());

        sequenceComplete = true;
        onSequenceComplete?.Invoke();
    }

    IEnumerator DesaturateScene()
    {
        // Get all images in the scene to desaturate
        Image[] allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        Color[] originalColors = new Color[allImages.Length];
        
        // Store original colors
        for (int i = 0; i < allImages.Length; i++)
        {
            originalColors[i] = allImages[i].color;
        }
        
        float t = 0f;
        while (t < desaturationDuration)
        {
            t += Time.deltaTime;
            float p = t / desaturationDuration;
            
            for (int i = 0; i < allImages.Length; i++)
            {
                // Skip the dark overlay
                if (allImages[i] == darkOverlay) continue;
                
                Color original = originalColors[i];
                // Convert to grayscale
                float gray = original.r * 0.3f + original.g * 0.59f + original.b * 0.11f;
                Color desaturated = new Color(gray, gray, gray, original.a);
                
                allImages[i].color = Color.Lerp(original, desaturated, p * 0.7f); // 70% desaturated
            }
            
            yield return null;
        }
    }

    IEnumerator BirdSlump(RectTransform bird, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Vector2 originalPos = bird.anchoredPosition;
        Quaternion originalRot = bird.localRotation;
        Vector3 originalScale = bird.localScale;
        
        // Slump animation - drop and tilt
        float t = 0f;
        while (t < birdSlumpTime)
        {
            t += Time.deltaTime;
            float p = t / birdSlumpTime;
            
            // Ease out with heavy feeling
            float ease = 1f - Mathf.Pow(1f - p, 2f);
            
            // Drop down
            Vector2 offset = new Vector2(0, -birdSlumpAmount * ease);
            bird.anchoredPosition = originalPos + offset;
            
            // Tilt head down sadly
            float tilt = -12f * ease;
            bird.localRotation = Quaternion.Euler(0, 0, tilt);
            
            // Shrink slightly (defeated posture)
            float scale = 1f - 0.08f * ease;
            bird.localScale = originalScale * scale;
            
            yield return null;
        }
        
        // Hold slumped position
        bird.anchoredPosition = originalPos + new Vector2(0, -birdSlumpAmount);
        bird.localRotation = Quaternion.Euler(0, 0, -12f);
        bird.localScale = originalScale * 0.92f;
    }

    IEnumerator IdleSadSway(RectTransform bird)
    {
        Vector2 basePos = bird.anchoredPosition;
        Quaternion baseRot = bird.localRotation;
        float randomOffset = Random.Range(0f, Mathf.PI * 2f);
        
        while (sequenceComplete)
        {
            float time = Time.time * idleSadSwaySpeed + randomOffset;
            float rockTime = Time.time * idleRockSpeed + randomOffset;
            
            // Very gentle, slow swaying position
            float swayX = Mathf.Sin(time) * idleSadSwayAmount * 0.3f;
            float swayY = Mathf.Sin(time * 0.7f) * idleSadSwayAmount * 0.5f;
            Vector2 swayOffset = new Vector2(swayX, swayY);
            bird.anchoredPosition = basePos + swayOffset;
            
            // Prominent side-to-side rocking motion
            float rockAngle = Mathf.Sin(rockTime) * idleRockAngle;
            bird.localRotation = baseRot * Quaternion.Euler(0, 0, rockAngle);
            
            yield return null;
        }
    }

    IEnumerator CloudIdleDrift()
    {
        if (rainCloud == null) yield break;
        
        float randomOffset = Random.Range(0f, Mathf.PI * 2f);
        
        while (sequenceComplete)
        {
            float time = Time.time * 0.3f + randomOffset;
            
            // Very slow, gentle drift
            float driftX = Mathf.Sin(time) * 8f;
            float driftY = Mathf.Sin(time * 0.5f) * 4f;
            Vector2 driftOffset = new Vector2(driftX, driftY);
            rainCloud.anchoredPosition = cloudBasePosition + driftOffset;
            
            yield return null;
        }
    }

    IEnumerator ScreenShake()
    {
        if (canvasRoot == null) yield break;
        
        Vector3 originalPos = canvasRoot.localPosition;
        float elapsed = 0f;
        
        while (elapsed < screenShakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = screenShakeIntensity * (1f - elapsed / screenShakeDuration);
            
            float offsetX = Random.Range(-strength, strength);
            float offsetY = Random.Range(-strength, strength);
            
            canvasRoot.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            
            yield return null;
        }
        
        canvasRoot.localPosition = originalPos;
    }

    void SetAlphaForRectTransform(RectTransform rectTransform, float alpha)
    {
        Image[] images = rectTransform.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
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

    IEnumerator FadeRectTransform(RectTransform rectTransform, float from, float to, float duration)
    {
        Image[] images = rectTransform.GetComponentsInChildren<Image>();
        
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            
            foreach (Image img in images)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
            
            yield return null;
        }
        
        foreach (Image img in images)
        {
            Color c = img.color;
            c.a = to;
            img.color = c;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Call this from a button
    public void RetryLevel()
    {
        Debug.Log("Retrying level...");
        // Reload scene or reset game state
    }

    public void ReturnToMenu()
    {
        Debug.Log("Returning to menu...");
        // Load menu scene
    }
}