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
        [HideInInspector] public bool isAnimating = false;
    }

    [Header("Hole Sprites")]
    public List<HoleSprites> holeSprites = new List<HoleSprites>();

    [Header("Animation Settings")]
    public float popUpHeight = 1f;
    public float animationDuration = 0.25f;
    public float holeBounceHeight = 0.5f;

    [Header("Ghost Settings (Success)")]
    public float ghostFloatHeight = 3.5f; 
    public float ghostFloatDuration = 0.5f;
    // NEW: How long it takes for the ghost to fade out at the end
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
    
    [Header("Animation Sounds")]
    public AudioSource snakeUpSound;    
    public AudioSource snakeDownSound;  
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
    
    // Input tracking
    private int currentActiveNote = -1;
    private bool isWormActive = false;
    private int activeHoleIndex = -1;
    private bool inputWindowOpen = false;
    
    private GameObject activeSprite; 
    
    private Coroutine inputWindowCoroutine;
    private Coroutine flashCoroutine; 
    private Coroutine wormFlashCoroutine;
    
    // Track the main animation coroutine so we can stop it if the ghost takes over
    private Coroutine currentPopUpCoroutine; 
    
    // Internal variables for worm slam effect
    private Vector3 wormTargetScale;

    // Game state
    private bool gameActive = false;
    private int beatsSinceStart = 0;
    private float beatTime = 0f; 

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
        float startBeat = 8f;        
        float beatInterval = noteBeatInterval; 
        int totalNotes = 32;         
        
        int totalMeasures = Mathf.CeilToInt((startBeat + (totalNotes * beatInterval)) / 4);
        beatmapBuilder builder = new beatmapBuilder(totalMeasures);
        
        for (int noteCounter = 0; noteCounter < totalNotes; noteCounter++)
        {
            float beatPosition = startBeat + (noteCounter * beatInterval);
            int measure = Mathf.FloorToInt(beatPosition / 4) + 1;
            int beat = Mathf.FloorToInt(beatPosition % 4) + 1;
            int holeType = (noteCounter % 8) + 1;
            builder.PlaceQuarterNote(measure, beat, holeType);
        }
        
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

            // Initialize Ghost
            if (hole.ghostSprite != null)
            {
                hole.ghostStartPos = hole.ghostSprite.transform.localPosition;
                hole.ghostSprite.SetActive(false);
            }
            
            if (hole.idleSprite != null) hole.idleSprite.SetActive(true);
        }
    }

    private void PlaySnakeUpSound() { if (snakeUpSound != null && snakeUpSound.enabled) snakeUpSound.Play(); }
    private void PlaySnakeDownSound() { if (snakeDownSound != null && snakeDownSound.enabled) snakeDownSound.Play(); }
    private void PlayWormUpSound() { if (wormUpSound != null && wormUpSound.enabled) wormUpSound.Play(); }
    private void PlayWormDownSound() { if (wormDownSound != null && wormDownSound.enabled) wormDownSound.Play(); }
    private void PlayHoleBounceSound() { if (holeBounceSound != null && holeBounceSound.enabled) holeBounceSound.Play(); }

    public void StartGame()
    {
        gameActive = true;
        beatsSinceStart = 0;
        beatTime = 0f;
    }

    protected override void Update()
    {
        if (!gameActive) return;
        base.Update();
        if (inputWindowOpen) CheckForInput();
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

        int holeIndex = (noteValue <= 4) ? (noteValue - 1) : (noteValue - 5);
        bool isWorm = noteValue >= 5;
        
        if (holeIndex >= 0 && holeIndex < holeSprites.Count)
        {
            currentActiveNote = noteValue;
            isWormActive = isWorm;
            activeHoleIndex = holeIndex;
            beatTime = Time.unscaledTime + animationDuration;
            StartPopUpAnimation(holeIndex, isWorm);
        }
    }

    private void CheckForInput()
    {
        if (currentActiveNote == -1) return;
        
        for (int i = 0; i < holeKeys.Length; i++)
        {
            if (Input.GetKeyDown(holeKeys[i]))
            {
                HandleArrowKeyPress(i);
                return; 
            }
        }
    }

    private void HandleArrowKeyPress(int pressedHoleIndex)
    {
        if (pressedHoleIndex == activeHoleIndex)
        {
            bool isActionKeyPressed = Input.GetKey(actionKey);

            if (isWormActive)
            {
                // WORM: Arrow + Space
                if (isActionKeyPressed) 
                { 
                    FlashWormEffect(); 
                    SuccessInput(); 
                }
                else { FailedInput("Worm requires Space key held"); }
            }
            else
            {
                // SNAKE: Arrow Only
                if (!isActionKeyPressed) { SuccessInput(); }
                else { FailedInput("Snake requires NO Space key"); }
            }
        }
        else
        {
            FailedInput($"Wrong hole pressed");
        }
    }

    private void SuccessInput()
    {
        if (showDebugMessages) Debug.Log("SUCCESS INPUT");
        
        // If it is a worm, we trigger the Ghost logic
        if (isWormActive && activeHoleIndex != -1)
        {
            TriggerGhostAnimation(activeHoleIndex);
        }

        FlashScreen(Color.green);
        PlayHitSound();
        CloseInputWindow();
    }

    // Handles stopping the normal worm and starting the ghost
    private void TriggerGhostAnimation(int holeIndex)
    {
        HoleSprites hole = holeSprites[holeIndex];
        
        if (hole.ghostSprite != null && hole.wormSprite != null)
        {
            // 1. Stop the regular PopUp/Down animation immediately
            if (currentPopUpCoroutine != null) StopCoroutine(currentPopUpCoroutine);

            // 2. Hide the regular worm
            hole.wormSprite.SetActive(false);
            
            // 3. Mark hole as not animating (so it can be used again later if needed)
            hole.isAnimating = false; 

            // 4. Position ghost exactly where the worm was (at the peak)
            hole.ghostSprite.transform.localPosition = hole.wormSprite.transform.localPosition;
            hole.ghostSprite.SetActive(true);

            // 5. Start the float animation
            StartCoroutine(GhostFloatAnimation(hole));
        }
    }

    // UPDATED: The specific animation for the ghost (now includes fade)
    private IEnumerator GhostFloatAnimation(HoleSprites hole)
    {
        Vector3 startPos = hole.ghostSprite.transform.localPosition;
        Vector3 targetPos = startPos + Vector3.up * ghostFloatHeight;
        
        // --- Setup Renderers and ensure Alpha is reset to 1 ---
        SpriteRenderer spr = hole.ghostSprite.GetComponent<SpriteRenderer>();
        Image img = hole.ghostSprite.GetComponent<Image>();
        // Helper function to set alpha instantly
        SetGhostAlpha(spr, img, 1f);

        // --- Phase 1: Float Up ---
        float elapsed = 0f;
        while (elapsed < ghostFloatDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / ghostFloatDuration);
            // "Reverse Speed Ramp" = Deceleration = Ease Out Sine
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            hole.ghostSprite.transform.localPosition = Vector3.Lerp(startPos, targetPos, easedT);
            yield return null;
        }

        // --- Phase 2: Fade Out ---
        float fadeElapsed = 0f;
        while (fadeElapsed < ghostFadeDuration)
        {
            fadeElapsed += Time.unscaledDeltaTime;
            float currentAlpha = Mathf.Lerp(1f, 0f, fadeElapsed / ghostFadeDuration);
            SetGhostAlpha(spr, img, currentAlpha);
            yield return null;
        }

        // --- Cleanup ---
        hole.ghostSprite.SetActive(false);
        hole.ghostSprite.transform.localPosition = hole.ghostStartPos;
        // Reset alpha back to 1 for next time just in case
        SetGhostAlpha(spr, img, 1f); 
    }

    // Helper to set alpha on either SpriteRenderer or Image
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

    private void FailedInput(string reason)
    {
        if (showDebugMessages) Debug.Log($"MISS INPUT: {reason}");
        
        // If we failed on a worm, swap to smiling worm immediately
        if (isWormActive && activeHoleIndex != -1)
        {
            SwapToSmileWorm(activeHoleIndex);
        }

        FlashScreen(Color.red);
        PlayMissSound();
        CloseInputWindow();
    }

    private void SwapToSmileWorm(int holeIndex)
    {
        HoleSprites hole = holeSprites[holeIndex];
        
        if (hole.wormSprite != null && hole.smileWormSprite != null)
        {
            hole.smileWormSprite.transform.localPosition = hole.wormSprite.transform.localPosition;
            hole.wormSprite.SetActive(false);
            hole.smileWormSprite.SetActive(true);
            activeSprite = hole.smileWormSprite;
        }
    }

    private void CloseInputWindow()
    {
        inputWindowOpen = false;
        currentActiveNote = -1;
        
        if (inputWindowCoroutine != null)
        {
            StopCoroutine(inputWindowCoroutine);
            inputWindowCoroutine = null;
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

    private void StartPopUpAnimation(int holeIndex, bool isWorm)
    {
        var hole = holeSprites[holeIndex];
        if (hole.isAnimating) return;
        GameObject targetSprite = isWorm ? hole.wormSprite : hole.snakeSprite;
        Vector3 startPos = isWorm ? hole.wormStartPos : hole.snakeStartPos;

        if (targetSprite != null)
        {
            activeSprite = targetSprite;
            // Store the coroutine so we can stop it if the Ghost needs to take over
            currentPopUpCoroutine = StartCoroutine(PopUpAnimation(hole, targetSprite, startPos, isWorm));
        }
    }

    private IEnumerator PopUpAnimation(HoleSprites hole, GameObject originalSprite, Vector3 startPos, bool isWorm)
    {
        hole.isAnimating = true;
        originalSprite.SetActive(true);
        originalSprite.transform.localPosition = startPos;
        
        Vector3 targetPos = startPos + Vector3.up * popUpHeight;
        
        float currentTime = Time.unscaledTime;
        float windowHalf = inputWindowSize / 2f;
        float timeUntilBeat = beatTime - currentTime;
        
        if (timeUntilBeat > animationDuration)
        {
            yield return new WaitForSecondsRealtime(timeUntilBeat - animationDuration);
        }
        
        if (isWorm) PlayWormUpSound(); else PlaySnakeUpSound();
        
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
        
        // Apex
        float apexTime = Time.unscaledTime;
        float windowStartTime = beatTime - windowHalf;
        float windowEndTime = beatTime + windowHalf;
        
        if (apexTime < windowStartTime)
        {
            yield return new WaitForSecondsRealtime(windowStartTime - apexTime);
            inputWindowOpen = true;
            inputWindowCoroutine = StartCoroutine(InputWindowTimer(inputWindowSize));
        }
        else if (apexTime < windowEndTime)
        {
            inputWindowOpen = true;
            inputWindowCoroutine = StartCoroutine(InputWindowTimer(windowEndTime - apexTime));
        }
        else
        {
            FailedInput("Missed window entirely");
        }
        
        float timeSpentSoFar = Time.unscaledTime - (beatTime - animationDuration);
        float remainingApexTime = quarterNoteTime - timeSpentSoFar;
        
        if (remainingApexTime > 0)
        {
            yield return new WaitForSecondsRealtime(remainingApexTime);
        }
        
        CloseInputWindow();
        
        if (isWorm) PlayWormDownSound(); else PlaySnakeDownSound();
        
        // Go DOWN
        GameObject spriteMovingDown = activeSprite; 

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
        hole.isAnimating = false;
        currentPopUpCoroutine = null;
    }

    private IEnumerator InputWindowTimer(float windowDuration)
    {
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime - startTime < windowDuration && inputWindowOpen)
        {
            yield return null;
        }
        
        if (inputWindowOpen && currentActiveNote != -1)
        {
            FailedInput("Time out");
        }
    }

    void OnGUI()
    {
        if (showDebugMessages && rhythmTimer != null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 350, 300));
            GUILayout.Label($"BPM: {rhythmTimer.bpm}");
            if (currentActiveNote != -1)
            {
                GUILayout.Label($"Active Hole: {activeHoleIndex}");
                GUILayout.Label($"Is Worm: {isWormActive}");
            }
            GUILayout.EndArea();
        }
    }

    public void ResetAllPositions()
    {
        StopAllCoroutines();

        foreach (var hole in holeSprites)
        {
            hole.isAnimating = false;
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
                 // Ensure alpha is reset if we reset the game mid-fade
                SetGhostAlpha(hole.ghostSprite.GetComponent<SpriteRenderer>(), hole.ghostSprite.GetComponent<Image>(), 1f);
            }
        }
        
        lastBouncedTick = -1;
        currentActiveNote = -1;
        inputWindowOpen = false;
        activeSprite = null;
        inputWindowCoroutine = null;
        currentPopUpCoroutine = null;
        gameActive = false;
        beatsSinceStart = 0;
        beatTime = 0f;
        
        if (flashPanel != null) flashPanel.SetActive(false);
        if (wormHitEffectObject != null) 
        {
             wormHitEffectObject.transform.localScale = wormTargetScale;
             wormHitEffectObject.SetActive(false);
        }
        
        if (snakeUpSound != null) snakeUpSound.Stop();
        if (snakeDownSound != null) snakeDownSound.Stop();
        if (wormUpSound != null) wormUpSound.Stop();
        if (wormDownSound != null) wormDownSound.Stop();
        if (holeBounceSound != null) holeBounceSound.Stop();
    }
    
    public void Pause()
    {
        gameActive = false;
        StopAllCoroutines(); 
        if (snakeUpSound != null) snakeUpSound.Pause();
        if (snakeDownSound != null) snakeDownSound.Pause();
        if (wormUpSound != null) wormUpSound.Pause();
        if (wormDownSound != null) wormDownSound.Pause();
        if (holeBounceSound != null) holeBounceSound.Pause();
    }
    
    public void Resume()
    {
        gameActive = true;
        if (snakeUpSound != null) snakeUpSound.UnPause();
        if (snakeDownSound != null) snakeDownSound.UnPause();
        if (wormUpSound != null) wormUpSound.UnPause();
        if (wormDownSound != null) wormDownSound.UnPause();
        if (holeBounceSound != null) holeBounceSound.UnPause();
    }
}