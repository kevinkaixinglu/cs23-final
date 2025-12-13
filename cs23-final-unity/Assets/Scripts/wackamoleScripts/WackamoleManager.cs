using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro; 
using UnityEngine.UI;

public class WackamoleManager : BeatmapVisualizerSimple
{
    [System.Serializable]
    public class HoleSprites
    {
        public string holeName;
        public GameObject idleSprite;
        public GameObject snakeSprite;
        public GameObject wormSprite;
        public GameObject smileWormSprite; 
        public GameObject ghostSprite;     
        
        [HideInInspector] public Vector3 snakeStartPos;
        [HideInInspector] public Vector3 wormStartPos;
        [HideInInspector] public Vector3 smileWormStartPos;
        [HideInInspector] public Vector3 ghostStartPos;
        [HideInInspector] public Vector3 idleStartPos;
        
        // --- NEW: Per-Hole State Tracking ---
        [HideInInspector] public bool isAnimating = false;
        [HideInInspector] public bool isHittable = false; // Replaces global inputWindowOpen
        [HideInInspector] public bool isWormType = false; // Replaces global isWormActive
        [HideInInspector] public Coroutine activeCoroutine; // Replaces global currentPopUpCoroutine
    }

    [Header("Hole Sprites")]
    public List<HoleSprites> holeSprites = new List<HoleSprites>();

    [Header("UI Prompts")]
    public TextMeshProUGUI promptTextUp;         
    public TextMeshProUGUI promptTextDownSpace;  
    public float promptDuration = 0.5f;          
    public float promptFadeDuration = 0.5f;      

    [Header("Animation Settings")]
    public float popUpHeight = 1f;
    public float animationDuration = 0.25f;
    public float holeBounceHeight = 0.5f;

    [Header("Ghost Settings (Success)")]
    public float ghostFloatHeight = 3.5f; 
    public float ghostFloatDuration = 0.5f;
    public float ghostFadeDuration = 0.3f; 

    [Header("Input Settings")]
    public KeyCode[] holeKeys = new KeyCode[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
    public KeyCode actionKey = KeyCode.Space; 

    [Header("Screen Flash (Red/Green)")]
    public GameObject flashPanel;
    public float flashDuration = 0.1f; 
    public float flashFadeOutDuration = 0.3f; 
    private float maxFlashAlpha = 0.3f;

    [Header("Worm Hit Effect Details")]
    public GameObject wormHitEffectObject;
    public float wormSlamDuration = 0.15f;
    public float wormStartScaleMultiplier = 2f;

    [Header("Audio")]
    public AudioSource hitSound;      
    public AudioSource missSound;     
    public AudioSource successSound;  
    public AudioSource wormMunchSound;

    [Header("Animation Sounds")]
    public AudioSource snakeUpSound;    
    public AudioSource snakeDownSound;
    public AudioSource snakeHissSound;   
    
    public AudioSource wormUpSound;     
    public AudioSource wormDownSound;   
    public AudioSource holeBounceSound; 

    [Header("Debug")]
    public bool showDebugMessages = true;

    [Header("Timing Settings")]
    public float timingOffset = 0f; 
    public float inputWindowSize = 0.2f; 

    [Header("Beatmap Settings")]
    public int noteBeatInterval = 2; 

    private float quarterNoteTime;
    private int lastBouncedTick = -1;
    
    // Tutorial flags
    private bool tutorialUpShown = false;
    private bool tutorialDownShown = false;
    
    private Coroutine flashCoroutine; 
    private Coroutine wormFlashCoroutine;
    
    private Vector3 wormTargetScale;

    private bool gameActive = false;
    private int beatsSinceStart = 0;
    
    // NOTE: Removed global "activeHoleIndex", "currentActiveNote", "inputWindowOpen"
    // effectively decoupling the holes from a single state machine.

    void Start()
    {
        if (showDebugMessages) Debug.Log("WackamoleManager Start called");
        
        if (rhythmTimer != null)
        {
            quarterNoteTime = 60f / (float)rhythmTimer.bpm; 
        }
        else
        {
            quarterNoteTime = 0.4444f; 
        }
        
        CreateSimpleBeatmap();
        InitializeSprites();
        lastBouncedTick = -1;
        
        if (flashPanel != null)
        {
            flashPanel.SetActive(false);
            if (flashPanel.GetComponent<Image>() == null)
                Debug.LogError("Flash Panel needs an Image component!");
        }

        if (promptTextUp != null) promptTextUp.gameObject.SetActive(false);
        if (promptTextDownSpace != null) promptTextDownSpace.gameObject.SetActive(false);
        
        if (wormHitEffectObject != null)
        {
            wormTargetScale = wormHitEffectObject.transform.localScale;
            wormHitEffectObject.SetActive(false);
            
            if (wormHitEffectObject.GetComponent<Image>() == null && wormHitEffectObject.GetComponent<SpriteRenderer>() == null)
                Debug.LogWarning("Worm Hit Object needs an Image or SpriteRenderer to fade properly!");
        }
        
        if (showDebugMessages) Debug.Log("WackamoleManager initialized. Waiting for game to start...");
    }

    private void CreateSimpleBeatmap()
    {
        beatmapBuilder builder = new beatmapBuilder(16);
        
        // --- NOTE ID KEY ---
        // 1 = Snake Top (Up)
        // 2 = Snake Bottom (Down)
        // 3 = Snake Left
        // 4 = Snake Right
        // 5 = Worm Top (Up)
        // 6 = Worm Bottom (Down)
        // 7 = Worm Left
        // 8 = Worm Right

        // Tutorial
        builder.PlaceQuarterNote(3, 1, 1); 
        builder.PlaceQuarterNote(4, 1, 6);

        // Sequence
        builder.PlaceQuarterNote(5, 1, 3); 
        builder.PlaceQuarterNote(5, 3, 8); 
        builder.PlaceQuarterNote(6, 1, 2); 
        builder.PlaceQuarterNote(6, 3, 5); 
        builder.PlaceQuarterNote(7, 1, 6); 

        // Fast Section (The part that was glitching)
        builder.PlaceQuarterNote(8, 1, 7); // Worm Left
        //builder.Pl  // Snake Left
        builder.PlaceQuarterNote(9, 2, 4); // Snake Right
        builder.PlaceQuarterNote(9, 3, 1); // Snake Top
        builder.PlaceQuarterNote(10, 1, 3); // Snake Left

        npcBeatMap = builder.GetBeatMap();
    }

    private void InitializeSprites()
    {
        foreach (var hole in holeSprites)
        {
            if (hole.idleSprite != null) hole.idleStartPos = hole.idleSprite.transform.localPosition;
            
            if (hole.snakeSprite != null)
            {
                hole.snakeStartPos = hole.snakeSprite.transform.localPosition;
                hole.snakeSprite.SetActive(false); 
            }
            
            if (hole.wormSprite != null)
            {
                hole.wormStartPos = hole.wormSprite.transform.localPosition;
                hole.wormSprite.SetActive(false); 
            }

            if (hole.smileWormSprite != null)
            {
                hole.smileWormStartPos = hole.smileWormSprite.transform.localPosition;
                hole.smileWormSprite.SetActive(false);
            }

            if (hole.ghostSprite != null)
            {
                hole.ghostStartPos = hole.ghostSprite.transform.localPosition;
                hole.ghostSprite.SetActive(false);
            }
            
            if (hole.idleSprite != null) hole.idleSprite.SetActive(true);
            
            // Reset state
            hole.isAnimating = false;
            hole.isHittable = false;
            hole.activeCoroutine = null;
        }
    }

    // Audio Wrappers
    private void PlaySnakeUpSound() { if (snakeUpSound != null && snakeUpSound.enabled) snakeUpSound.Play(); }
    private void PlaySnakeDownSound() { if (snakeDownSound != null && snakeDownSound.enabled) snakeDownSound.Play(); }
    private void PlaySnakeHissSound() { if (snakeHissSound != null && snakeHissSound.enabled) snakeHissSound.Play(); }
    private void PlayWormUpSound() { if (wormUpSound != null && wormUpSound.enabled) wormUpSound.Play(); }
    private void PlayWormDownSound() { if (wormDownSound != null && wormDownSound.enabled) wormDownSound.Play(); }
    private void PlayHoleBounceSound() { if (holeBounceSound != null && holeBounceSound.enabled) holeBounceSound.Play(); }
    private void PlaySuccessSound() { if (successSound != null && successSound.enabled) successSound.Play(); }
    private void PlayWormMunchSound() { if (wormMunchSound != null && wormMunchSound.enabled) wormMunchSound.Play(); }

    public void StartGame()
    {
        gameActive = true;
        beatsSinceStart = 0;
    }

    protected override void Update()
    {
        if (!gameActive) return;
        base.Update();
        CheckForInput(); // Always check for input
    }

    protected override void OnBeatTriggered(int noteValue)
    {
        if (!gameActive) return;
        
        beatsSinceStart++;
        
        if (rhythmTimer.curr_tick % 4 == 0 && rhythmTimer.curr_tick != lastBouncedTick)
        {
            BounceAllHoles();
            lastBouncedTick = rhythmTimer.curr_tick;
        }
        
        if (noteValue == 0) return;

        // --- HOLE LOGIC ---
        // 1-4 are holes 0-3 (Snake)
        // 5-8 are holes 0-3 (Worm)
        int holeIndex = (noteValue <= 4) ? (noteValue - 1) : (noteValue - 5);
        bool isWorm = noteValue >= 5;
        
        if (holeIndex >= 0 && holeIndex < holeSprites.Count)
        {
            // --- TUTORIAL TEXT TRIGGER ---
            if (noteValue == 1 && promptTextUp != null && !tutorialUpShown) 
            {
                tutorialUpShown = true;
                StartCoroutine(FlashPrompt(promptTextUp, "UP"));
            }
            if (noteValue == 6 && promptTextDownSpace != null && !tutorialDownShown)
            {
                tutorialDownShown = true;
                StartCoroutine(FlashPrompt(promptTextDownSpace, "DOWN + SPACEBAR"));
            }

            // --- ANIMATION START ---
            // We pass the beat time relative to *now* + animation duration
            float targetBeatTime = Time.unscaledTime + animationDuration;
            StartPopUpAnimation(holeIndex, isWorm, targetBeatTime);
        }
    }

    private IEnumerator FlashPrompt(TextMeshProUGUI tmpText, string text)
    {
        if (tmpText == null) yield break;
        tmpText.text = text;
        tmpText.gameObject.SetActive(true);
        tmpText.alpha = 1f; 
        yield return new WaitForSecondsRealtime(promptDuration); 
        float elapsed = 0f;
        while (elapsed < promptFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            tmpText.alpha = Mathf.Lerp(1f, 0f, elapsed / promptFadeDuration);
            yield return null;
        }
        tmpText.alpha = 0f;
        tmpText.gameObject.SetActive(false);
    }

    // --- REFACTORED INPUT LOGIC ---
    private void CheckForInput()
    {
        // Iterate through all possible hole keys
        for (int i = 0; i < holeKeys.Length; i++)
        {
            if (Input.GetKeyDown(holeKeys[i]))
            {
                // Verify bounds
                if (i < holeSprites.Count)
                {
                    HandleInputForHole(i);
                }
                return; // Only handle one hole press per frame to prevent spamming
            }
        }
    }

    private void HandleInputForHole(int index)
    {
        HoleSprites hole = holeSprites[index];

        // Only process if the hole is currently in its hittable window
        if (hole.isHittable)
        {
            bool isActionKeyPressed = Input.GetKey(actionKey);

            if (hole.isWormType)
            {
                // WORM: Requires Action Key (Space)
                if (isActionKeyPressed) 
                { 
                    FlashWormEffect(); 
                    SuccessInput(hole, index); 
                }
                else 
                { 
                    FailedInput(hole, index, "Worm requires Space key held"); 
                }
            }
            else
            {
                // SNAKE: Requires NO Action Key
                if (!isActionKeyPressed) 
                { 
                    SuccessInput(hole, index); 
                }
                else 
                { 
                    FailedInput(hole, index, "Snake requires NO Space key"); 
                }
            }
        }
        else
        {
            // Optional: You can uncomment this if you want a penalty for hitting empty holes,
            // but for 1-beat intervals, it's safer to just ignore stray clicks or log them.
            // FailedInput(hole, index, "Hole not active or missed window");
        }
    }

    private void SuccessInput(HoleSprites hole, int index)
    {
        if (showDebugMessages) Debug.Log($"SUCCESS INPUT on Hole {index}");
        
        // Immediately mark as handled so we don't hit it twice
        hole.isHittable = false;

        if (hole.isWormType)
        {
            TriggerGhostAnimation(hole);
            PlayWormMunchSound(); 
        }

        FlashScreen(Color.green);
        PlayHitSound(); 
        PlaySuccessSound(); 
    }

    private void FailedInput(HoleSprites hole, int index, string reason)
    {
        if (showDebugMessages) Debug.Log($"MISS INPUT on Hole {index}: {reason}");
        
        hole.isHittable = false; // Close window

        if (hole.isWormType)
        {
            SwapToSmileWorm(hole);
        }

        FlashScreen(Color.red);
        PlayMissSound();
    }

    private void TriggerGhostAnimation(HoleSprites hole)
    {
        if (hole.ghostSprite != null && hole.wormSprite != null)
        {
            // Stop the main animation coroutine for this specific hole
            if (hole.activeCoroutine != null) StopCoroutine(hole.activeCoroutine);

            hole.wormSprite.SetActive(false);
            hole.isAnimating = false; 

            hole.ghostSprite.transform.localPosition = hole.wormSprite.transform.localPosition;
            hole.ghostSprite.SetActive(true);

            StartCoroutine(GhostFloatAnimation(hole));
        }
    }

    private IEnumerator GhostFloatAnimation(HoleSprites hole)
    {
        Vector3 startPos = hole.ghostSprite.transform.localPosition;
        Vector3 targetPos = startPos + Vector3.up * ghostFloatHeight;
        
        SpriteRenderer spr = hole.ghostSprite.GetComponent<SpriteRenderer>();
        Image img = hole.ghostSprite.GetComponent<Image>();
        SetGhostAlpha(spr, img, 1f);

        float elapsed = 0f;
        while (elapsed < ghostFloatDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / ghostFloatDuration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            hole.ghostSprite.transform.localPosition = Vector3.Lerp(startPos, targetPos, easedT);
            yield return null;
        }

        float fadeElapsed = 0f;
        while (fadeElapsed < ghostFadeDuration)
        {
            fadeElapsed += Time.unscaledDeltaTime;
            float currentAlpha = Mathf.Lerp(1f, 0f, fadeElapsed / ghostFadeDuration);
            SetGhostAlpha(spr, img, currentAlpha);
            yield return null;
        }

        hole.ghostSprite.SetActive(false);
        hole.ghostSprite.transform.localPosition = hole.ghostStartPos;
        SetGhostAlpha(spr, img, 1f); 
    }

    private void SetGhostAlpha(SpriteRenderer spr, Image img, float alpha)
    {
        if (spr != null)
        {
            Color c = spr.color;
            c.a = alpha;
            spr.color = c;
        }
        else if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }

    private void SwapToSmileWorm(HoleSprites hole)
    {
        if (hole.wormSprite != null && hole.smileWormSprite != null)
        {
            hole.smileWormSprite.transform.localPosition = hole.wormSprite.transform.localPosition;
            hole.wormSprite.SetActive(false);
            hole.smileWormSprite.SetActive(true);
            // Note: The main PopUpAnimation coroutine will handle bringing this down
        }
    }

    private void FlashScreen(Color color)
    {
        if (flashPanel != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(DoFlashScreenFade(color));
        }
    }

    private IEnumerator DoFlashScreenFade(Color targetColor)
    {
        flashPanel.SetActive(true);
        Image flashImage = flashPanel.GetComponent<Image>();
        if (flashImage == null) yield break;

        flashImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, maxFlashAlpha);
        yield return new WaitForSecondsRealtime(flashDuration);

        float elapsed = 0f;
        while (elapsed < flashFadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float currentAlpha = Mathf.Lerp(maxFlashAlpha, 0f, elapsed / flashFadeOutDuration);
            flashImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, currentAlpha);
            yield return null;
        }

        flashImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
        flashPanel.SetActive(false);
        flashCoroutine = null;
    }

    private void FlashWormEffect()
    {
        if (wormHitEffectObject != null)
        {
            if (wormFlashCoroutine != null) StopCoroutine(wormFlashCoroutine);
            wormFlashCoroutine = StartCoroutine(DoWormFlashFade());
        }
    }

    private IEnumerator DoWormFlashFade()
    {
        wormHitEffectObject.SetActive(true);
        
        Image img = wormHitEffectObject.GetComponent<Image>();
        SpriteRenderer spr = wormHitEffectObject.GetComponent<SpriteRenderer>();

        Color baseColor = Color.white;
        if (img != null) baseColor = img.color;
        else if (spr != null) baseColor = spr.color;

        Vector3 startScale = wormTargetScale * wormStartScaleMultiplier;

        float elapsedSlam = 0f;
        while (elapsedSlam < wormSlamDuration)
        {
            elapsedSlam += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedSlam / wormSlamDuration);
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            wormHitEffectObject.transform.localScale = Vector3.Lerp(startScale, wormTargetScale, easedT);
            SetObjectAlpha(img, spr, baseColor, easedT);

            yield return null;
        }
        wormHitEffectObject.transform.localScale = wormTargetScale;
        SetObjectAlpha(img, spr, baseColor, 1f);

        yield return new WaitForSecondsRealtime(flashDuration);

        float elapsedFade = 0f;
        while (elapsedFade < flashFadeOutDuration)
        {
            elapsedFade += Time.unscaledDeltaTime;
            float currentAlpha = Mathf.Lerp(1f, 0f, elapsedFade / flashFadeOutDuration);
            SetObjectAlpha(img, spr, baseColor, currentAlpha);
            yield return null;
        }

        SetObjectAlpha(img, spr, baseColor, 0f);
        wormHitEffectObject.SetActive(false);
        wormHitEffectObject.transform.localScale = wormTargetScale;
        wormFlashCoroutine = null;
    }

    private void SetObjectAlpha(Image img, SpriteRenderer spr, Color baseColor, float alpha)
    {
        if (img != null) img.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        else if (spr != null) spr.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }

    private void PlayHitSound() { if (hitSound != null) hitSound.Play(); }
    private void PlayMissSound() { if (missSound != null) missSound.Play(); }

    private void BounceAllHoles()
    {
        foreach (var hole in holeSprites)
        {
            if (hole.idleSprite != null) StartCoroutine(BounceHole(hole));
        }
    }

    private IEnumerator BounceHole(HoleSprites hole)
    {
        PlayHoleBounceSound();
        Vector3 startPos = hole.idleStartPos;
        Vector3 bouncePos = startPos + Vector3.up * holeBounceHeight;
        hole.idleSprite.transform.localPosition = bouncePos;
        yield return new WaitForSecondsRealtime(0.05f); 
        hole.idleSprite.transform.localPosition = startPos;
    }

    // --- REFACTORED ANIMATION ---
    private void StartPopUpAnimation(int holeIndex, bool isWorm, float targetBeatTime)
    {
        var hole = holeSprites[holeIndex];
        
        // Safety check: Don't interrupt if it's already busy 
        // (unless you want to allow spamming the same hole faster than animation speed)
        if (hole.isAnimating) return; 

        GameObject targetSprite = isWorm ? hole.wormSprite : hole.snakeSprite;
        Vector3 startPos = isWorm ? hole.wormStartPos : hole.snakeStartPos;

        if (targetSprite != null)
        {
            // Set Per-Hole State
            hole.isWormType = isWorm;
            
            // Start independent coroutine
            hole.activeCoroutine = StartCoroutine(PopUpAnimation(hole, targetSprite, startPos, isWorm, targetBeatTime));
        }
    }

    private IEnumerator PopUpAnimation(HoleSprites hole, GameObject originalSprite, Vector3 startPos, bool isWorm, float targetBeatTime)
    {
        hole.isAnimating = true;
        originalSprite.SetActive(true);
        originalSprite.transform.localPosition = startPos;
        
        Vector3 targetPos = startPos + Vector3.up * popUpHeight;
        
        float currentTime = Time.unscaledTime;
        float windowHalf = inputWindowSize / 2f;
        float timeUntilBeat = targetBeatTime - currentTime;
        
        // Wait until we need to start moving up
        if (timeUntilBeat > animationDuration)
        {
            yield return new WaitForSecondsRealtime(timeUntilBeat - animationDuration);
        }
        
        if (isWorm) PlayWormUpSound(); 
        else { PlaySnakeUpSound(); PlaySnakeHissSound(); }
        
        // Go UP
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / animationDuration);
            originalSprite.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        originalSprite.transform.localPosition = targetPos;
        
        // --- INPUT WINDOW LOGIC (Inside Coroutine) ---
        // Calculate how long we wait at the top before the window closes
        // The window centers on 'targetBeatTime'.
        
        float now = Time.unscaledTime;
        float windowEndTime = targetBeatTime + windowHalf;
        float windowStartTime = targetBeatTime - windowHalf;

        // If we arrived early (due to frame rate), wait for window start
        if (now < windowStartTime)
        {
            yield return new WaitForSecondsRealtime(windowStartTime - now);
        }

        // OPEN WINDOW
        hole.isHittable = true;

        // Wait for Window Duration
        now = Time.unscaledTime;
        if (now < windowEndTime)
        {
            yield return new WaitForSecondsRealtime(windowEndTime - now);
        }

        // CLOSE WINDOW
        // If it was still hittable, that means the player missed the timing
        if (hole.isHittable)
        {
             FailedInput(hole, holeSprites.IndexOf(hole), "Timed out");
        }
        
        hole.isHittable = false;
        
        // Wait out the rest of the beat (if quarter note is long)
        // This makes sure it stays up for a moment before going down
        // Optional: you can remove this if you want it to go down immediately after window closes
        float timeSpentTotal = Time.unscaledTime - (targetBeatTime - animationDuration);
        if (timeSpentTotal < quarterNoteTime)
        {
            // yield return new WaitForSecondsRealtime(quarterNoteTime - timeSpentTotal);
        }

        if (isWorm) PlayWormDownSound(); else PlaySnakeDownSound();
        
        // Go DOWN
        // Note: Check what is currently active (could have swapped to SmileWorm)
        GameObject spriteMovingDown = originalSprite;
        if (hole.smileWormSprite != null && hole.smileWormSprite.activeSelf) 
            spriteMovingDown = hole.smileWormSprite;

        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / animationDuration);
            spriteMovingDown.transform.localPosition = Vector3.Lerp(targetPos, startPos, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        spriteMovingDown.transform.localPosition = startPos;
        spriteMovingDown.SetActive(false);
        
        // Reset Logic
        if (hole.smileWormSprite != null) hole.smileWormSprite.SetActive(false);
        
        hole.isAnimating = false;
        hole.activeCoroutine = null;
    }

    void OnGUI()
    {
        if (showDebugMessages && rhythmTimer != null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 350, 300));
            GUILayout.Label($"BPM: {rhythmTimer.bpm}");
            GUILayout.EndArea();
        }
    }

    public void ResetAllPositions()
    {
        StopAllCoroutines();

        foreach (var hole in holeSprites)
        {
            hole.isAnimating = false;
            hole.isHittable = false;
            hole.activeCoroutine = null;

            if (hole.idleSprite != null) hole.idleSprite.transform.localPosition = hole.idleStartPos;
            
            if (hole.snakeSprite != null)
            {
                hole.snakeSprite.transform.localPosition = hole.snakeStartPos;
                hole.snakeSprite.SetActive(false);
            }
            if (hole.wormSprite != null)
            {
                hole.wormSprite.transform.localPosition = hole.wormStartPos;
                hole.wormSprite.SetActive(false);
            }
            if (hole.smileWormSprite != null)
            {
                hole.smileWormSprite.transform.localPosition = hole.smileWormStartPos;
                hole.smileWormSprite.SetActive(false);
            }
            if (hole.ghostSprite != null)
            {
                hole.ghostSprite.transform.localPosition = hole.ghostStartPos;
                hole.ghostSprite.SetActive(false);
                SetGhostAlpha(hole.ghostSprite.GetComponent<SpriteRenderer>(), hole.ghostSprite.GetComponent<Image>(), 1f);
            }
        }
        
        lastBouncedTick = -1;
        gameActive = false;
        beatsSinceStart = 0;
        
        tutorialUpShown = false;
        tutorialDownShown = false;
        
        if (flashPanel != null) flashPanel.SetActive(false);
        if (wormHitEffectObject != null) 
        {
             wormHitEffectObject.transform.localScale = wormTargetScale;
             wormHitEffectObject.SetActive(false);
        }
        
        if (snakeUpSound != null) snakeUpSound.Stop();
        if (snakeDownSound != null) snakeDownSound.Stop();
        if (snakeHissSound != null) snakeHissSound.Stop();
        if (wormUpSound != null) wormUpSound.Stop();
        if (wormDownSound != null) wormDownSound.Stop();
        if (holeBounceSound != null) holeBounceSound.Stop();
        if (successSound != null) successSound.Stop();
        if (wormMunchSound != null) wormMunchSound.Stop();
        
        if (promptTextUp != null) promptTextUp.gameObject.SetActive(false);
        if (promptTextDownSpace != null) promptTextDownSpace.gameObject.SetActive(false);
    }
    
    public void Pause()
    {
        gameActive = false;
        StopAllCoroutines(); 
        if (snakeUpSound != null) snakeUpSound.Pause();
        if (snakeDownSound != null) snakeDownSound.Pause();
        if (snakeHissSound != null) snakeHissSound.Pause();
        if (wormUpSound != null) wormUpSound.Pause();
        if (wormDownSound != null) wormDownSound.Pause();
        if (holeBounceSound != null) holeBounceSound.Pause();
        if (successSound != null) successSound.Pause();
        if (wormMunchSound != null) wormMunchSound.Pause();
    }
    
    public void Resume()
    {
        gameActive = true;
        if (snakeUpSound != null) snakeUpSound.UnPause();
        if (snakeDownSound != null) snakeDownSound.UnPause();
        if (snakeHissSound != null) snakeHissSound.UnPause();
        if (wormUpSound != null) wormUpSound.UnPause();
        if (wormDownSound != null) wormDownSound.UnPause();
        if (holeBounceSound != null) holeBounceSound.UnPause();
        if (successSound != null) successSound.UnPause();
        if (wormMunchSound != null) wormMunchSound.UnPause();
    }
}