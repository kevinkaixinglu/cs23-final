using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelCompleteCinematic : MonoBehaviour
{
    [Header("References")]
    public Image darkOverlay;       // black semi-transparent panel
    public Image spotlight;         // soft circle spotlight
    public RectTransform banner;    // LEVEL COMPLETE banner
    public RectTransform femaleBird;
    public RectTransform raven;

    [Header("Timings (slower, cinematic)")]
    public float overlayFadeTime = 1.0f;      // overlay fade-in duration
    public float spotlightFadeTime = 1.0f;    // spotlight fade-in
    public float bannerDropTime = 1.0f;       // banner drop duration
    public float birdBounceHeight = 20f;      // pixels
    public float birdBounceTime = 0.7f;       // bird bounce duration
    public float stepDelay = 0.3f;            // delay between steps

    void Start()
    {
        // start invisible
        darkOverlay.color = new Color(0, 0, 0, 0);
        SetAlpha(spotlight, 0f);
        banner.gameObject.SetActive(false); // banner hidden at first

        // start the sequence
        StartCoroutine(LevelCompleteSequence());
    }

    IEnumerator LevelCompleteSequence()
    {
        // 1. Fade in dark overlay
        yield return StartCoroutine(FadeImageAlpha(darkOverlay, 0f, 0.6f, overlayFadeTime));
        yield return new WaitForSeconds(stepDelay);

        // 2. Fade in spotlight
        yield return StartCoroutine(FadeImageAlpha(spotlight, 0f, 1f, spotlightFadeTime));
        yield return new WaitForSeconds(stepDelay);

        // 3. Show banner
        banner.gameObject.SetActive(true);

        // Drop from above with smooth bounce
        Vector3 startPos = banner.anchoredPosition;
        startPos.y += 300; // start off-screen above
        Vector3 endPos = banner.anchoredPosition;
        banner.anchoredPosition = startPos;

        float t = 0f;
        while(t < bannerDropTime)
        {
            t += Time.deltaTime;
            float p = t / bannerDropTime;
            // floaty ease-out + slight overshoot
            float bounce = Mathf.Sin(p * Mathf.PI * 0.5f) + 0.05f * Mathf.Sin(p * Mathf.PI * 2f);
            banner.anchoredPosition = Vector3.Lerp(startPos, endPos, bounce);
            yield return null;
        }
        banner.anchoredPosition = endPos;

        yield return new WaitForSeconds(stepDelay);

        // 4. Bounce birds
        StartCoroutine(BirdBounce(femaleBird));
        StartCoroutine(BirdBounce(raven));
    }

    IEnumerator BirdBounce(RectTransform bird)
    {
        Vector3 original = bird.anchoredPosition;
        Vector3 up = original + new Vector3(0, birdBounceHeight, 0);
        float t = 0f;
        while(t < birdBounceTime)
        {
            t += Time.deltaTime;
            float p = t / birdBounceTime;
            // simple up-down motion with sine curve
            bird.anchoredPosition = Vector3.Lerp(original, up, Mathf.Sin(p * Mathf.PI));
            yield return null;
        }
        bird.anchoredPosition = original;
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
        while(t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            img.color = c;
            yield return null;
        }
        c.a = to;
        img.color = c;
    }
}
