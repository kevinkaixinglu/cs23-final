using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        [HideInInspector] public bool isInputWindowOpen = false;
        [HideInInspector] public bool currentIsWorm = false; 
        
        [HideInInspector] public Coroutine visualCoroutine;
        [HideInInspector] public Coroutine logicCoroutine;
    }

    [Header("Hole Sprites")]
    public List<HoleSprites> holeSprites = new List<HoleSprites>();

    [Header("UI Prompts")]
    public TextMeshProUGUI promptTextUp;         
    public TextMeshProUGUI promptTextDownSpace;  
    public float promptDuration = 0.5f;          
    public float promptFadeDuration = 0.5f;      


    [Header("Score Settings")]
    public TextMeshProUGUI scoreText; 
    private int currentScore = 0;
    private int maxScore = 30;

    // ======== KALEN IDK IF SUPPOSED TO DO THIS =========
    [Header("Win/Loss Settings")]
    public string winScene = "LevelComplete";  
    public string loseSceneName = "LevelFailed"; 
    public int scoreToWin = 30;              

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
    
    private bool tutorialUpShown = false;
    private bool tutorialDownShown = false;
    
    private Coroutine flashCoroutine; 
    private Coroutine wormFlashCoroutine;
    private Vector3 wormTargetScale;

    private bool gameActive = false;
    private int beatsSinceStart = 0;

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
        
        if (scoreText == null)
        {
            GameObject canvasObj = GameObject.Find("canvas");
            if (canvasObj != null)
            {
                Transform t = canvasObj.transform.Find("canvas_score/score_object/scoreText");
                if (t != null)
                {
                    scoreText = t.GetComponent<TextMeshProUGUI>();
                }
            }
        }
        UpdateScoreDisplay();
        
        if (flashPanel != null) flashPanel.SetActive(false);
        if (promptTextUp != null) promptTextUp.gameObject.SetActive(false);
        if (promptTextDownSpace != null) promptTextDownSpace.gameObject.SetActive(false);
        
        if (wormHitEffectObject != null)
        {
            wormTargetScale = wormHitEffectObject.transform.localScale;
            wormHitEffectObject.SetActive(false);
        }
        
        if (showDebugMessages) Debug.Log("WackamoleManager initialized. Waiting for game to start...");
    }

    // ... [CreateSimpleBeatmap() stays the same] ...
    private void CreateSimpleBeatmap()
    {
        beatmapBuilder builder = new beatmapBuilder(30); 
        // Tutorial
        builder.PlaceQuarterNote(3, 1, 1);
        builder.PlaceQuarterNote(4, 1, 6);
        // Sequence
        builder.PlaceQuarterNote(5, 1, 3); 
        builder.PlaceQuarterNote(6, 1, 2); 
        builder.PlaceQuarterNote(6, 3, 5); 
        builder.PlaceQuarterNote(7, 1, 6); 
        builder.PlaceQuarterNote(8, 1, 7);
        builder.PlaceQuarterNote(9, 1, 8);
        builder.PlaceQuarterNote(9, 3, 3);
        builder.PlaceQuarterNote(10, 1, 8);
        builder.PlaceQuarterNote(10, 3, 2);
        builder.PlaceQuarterNote(11, 1, 5);
        builder.PlaceQuarterNote(12, 1, 4);
        builder.PlaceQuarterNote(12, 3, 2);
        builder.PlaceQuarterNote(13, 1, 5);
        builder.PlaceQuarterNote(14, 1, 4);
        builder.PlaceQuarterNote(14, 3, 7);
        builder.PlaceQuarterNote(15, 1, 1);
        builder.PlaceQuarterNote(16, 1, 5);
        builder.PlaceQuarterNote(17, 1, 3);
        builder.PlaceQuarterNote(17, 3, 4);
        builder.PlaceQuarterNote(18, 1, 5);
        // Consecutive notes
        builder.PlaceQuarterNote(19, 1, 3);
        builder.PlaceQuarterNote(19, 2, 1);
        builder.PlaceQuarterNote(19, 3, 4);
        builder.PlaceQuarterNote(19, 4, 6);
        builder.PlaceQuarterNote(20, 3, 3);
        builder.PlaceQuarterNote(21, 1, 4);
        builder.PlaceQuarterNote(21, 2, 1);
        builder.PlaceQuarterNote(21, 3, 3);
        builder.PlaceQuarterNote(21, 4, 6);
        builder.PlaceQuarterNote(22, 3, 4);
        builder.PlaceQuarterNote(23, 1, 4);
        builder.PlaceQuarterNote(23, 3, 4);
        builder.PlaceQuarterNote(24, 1, 3);
        builder.PlaceQuarterNote(24, 3, 3);
        builder.PlaceQuarterNote(25, 1, 2);
        builder.PlaceQuarterNote(25, 2, 3);
        builder.PlaceQuarterNote(25, 3, 1);
        builder.PlaceQuarterNote(25, 4, 4);
        builder.PlaceQuarterNote(26, 1, 6);
        builder.PlaceQuarterNote(26, 3, 7);
        builder.PlaceQuarterNote(27, 1, 2);
        builder.PlaceQuarterNote(28, 1, 1);
        builder.PlaceQuarterNote(29, 1, 6);

        npcBeatMap = builder.GetBeatMap();
    }

    // ... [InitializeSprites() stays the same] ...
    private void InitializeSprites()
    {
        foreach (var hole in holeSprites)
        {
            hole.isAnimating = false;
            hole.isInputWindowOpen = false;

            if (hole.idleSprite != null) hole.idleStartPos = hole.idleSprite.transform.localPosition;
            if (hole.snakeSprite != null) { hole.snakeStartPos = hole.snakeSprite.transform.localPosition; hole.snakeSprite.SetActive(false); }
            if (hole.wormSprite != null) { hole.wormStartPos = hole.wormSprite.transform.localPosition; hole.wormSprite.SetActive(false); }
            if (hole.smileWormSprite != null) { hole.smileWormStartPos = hole.smileWormSprite.transform.localPosition; hole.smileWormSprite.SetActive(false); }
            if (hole.ghostSprite != null) { hole.ghostStartPos = hole.ghostSprite.transform.localPosition; hole.ghostSprite.SetActive(false); }
            if (hole.idleSprite != null) hole.idleSprite.SetActive(true);
        }
    }

    // ... [Audio Helpers stay the same] ...
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

    // ======== UPDATED UPDATE FUNCTION =========
    protected override void Update()
    {
        if (!gameActive) return;
        base.Update();
        CheckForInput();

        // gameActive must be true (meaning we haven't paused via menu)
        // we must have references to the timer and audio
        // The music is NOT playing anymore
        // We check 'beatsSinceStart > 0' to make sure the game didn't *just* start and music hasn't loaded yet.
        if (rhythmTimer != null && rhythmTimer.musicSource != null && !rhythmTimer.musicSource.isPlaying && beatsSinceStart > 1)
        {
            EndLevel();
        }
    }

    // ======== KALEN IDK IF SUPPOSED TO DO THIS =========
    private void EndLevel()
    {
        gameActive = false; 

        // Check if score meets win condition
        if (currentScore >= scoreToWin)
        {
            Debug.Log("YOU WIN!");
            // Check if scene name is valid before loading to avoid crash
            if (!string.IsNullOrEmpty(winScene))
            {
                PlayerPrefs.SetInt("Level3Passed", 1);
                SceneManager.LoadScene(winScene);
            }
        }
        else
        {
            Debug.Log("YOU LOSE!");
            if (!string.IsNullOrEmpty(loseSceneName))
            {
                SceneManager.LoadScene(loseSceneName);
            }
        }
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
            if (noteValue == 1 && promptTextUp != null && !tutorialUpShown) 
            {
                tutorialUpShown = true;
                StartCoroutine(FlashPrompt(promptTextUp, "UP"));
            }
            if (noteValue == 6 && promptTextDownSpace != null && !tutorialDownShown)
            {
                tutorialDownShown = true;
                StartCoroutine(FlashPrompt(promptTextDownSpace, "DOWN")); 
            }

            float targetBeatTime = Time.unscaledTime + animationDuration;
            StartPopUp(holeIndex, isWorm, targetBeatTime);
        }
    }

    // ... [CheckForInput, HandleArrowKeyPress, etc. stay the same] ...
    private void CheckForInput()
    {
        for (int i = 0; i < holeKeys.Length; i++) 
        {
            if (Input.GetKeyDown(holeKeys[i]))
            {
                if (i < holeSprites.Count && holeSprites[i].isInputWindowOpen)
                {
                    HandleArrowKeyPress(i, holeSprites[i]);
                    return; 
                }

                for (int j = 0; j < holeSprites.Count; j++)
                {
                    if (holeSprites[j].isInputWindowOpen)
                    {
                        FailedInput(j, "Wrong Key Pressed"); 
                        return; 
                    }
                }
                
                return;
            }
        }
    }

    private void HandleArrowKeyPress(int pressedHoleIndex, HoleSprites hole)
    {
        if (hole.currentIsWorm)
        {
            FlashWormEffect();
            TriggerGhostAnimation(pressedHoleIndex);
            PlayWormMunchSound();
        }
        SuccessInput(hole);
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {currentScore}/{maxScore}";
        }
        if (currentScore >= maxScore)
        {
            scoreText.color = Color.green;
        }
        else
        {
            scoreText.color = Color.black;
        }
    }

    private void SuccessInput(HoleSprites hole)
    {
        if (showDebugMessages) Debug.Log($"SUCCESS INPUT on {hole.holeName}");
        
        currentScore++;
        UpdateScoreDisplay();
        
        FlashScreen(Color.green);
        PlayHitSound(); 
        PlaySuccessSound(); 
        
        hole.isInputWindowOpen = false; 
    }

    private void FailedInput(int holeIndex, string reason)
    {
        if (showDebugMessages) Debug.Log($"MISS INPUT on Hole {holeIndex}: {reason}");
        
        currentScore--;
        if (currentScore < 0) currentScore = 0; 
        UpdateScoreDisplay();
        
        HoleSprites hole = holeSprites[holeIndex];

        if (hole.currentIsWorm)
        {
            SwapToSmileWorm(holeIndex);
        }

        FlashScreen(Color.red);
        PlayMissSound();
        
        hole.isInputWindowOpen = false;
    }

    // ... [Rest of the functions: TriggerGhostAnimation, StartPopUp, PopUpAnimation, InputFailureTimer, etc. stay the same] ...
    private void TriggerGhostAnimation(int holeIndex)
    {
        HoleSprites hole = holeSprites[holeIndex];
        
        if (hole.ghostSprite != null && hole.wormSprite != null)
        {
            if (hole.visualCoroutine != null) StopCoroutine(hole.visualCoroutine);

            hole.wormSprite.SetActive(false);
            hole.isAnimating = false; 
            hole.isInputWindowOpen = false;

            hole.ghostSprite.transform.localPosition = hole.wormSprite.transform.localPosition;
            hole.ghostSprite.SetActive(true);

            StartCoroutine(GhostFloatAnimation(hole));
        }
    }

    private void StartPopUp(int holeIndex, bool isWorm, float targetBeatTime)
    {
        var hole = holeSprites[holeIndex];
        if (hole.isAnimating) return; 

        GameObject targetSprite = isWorm ? hole.wormSprite : hole.snakeSprite;
        Vector3 startPos = isWorm ? hole.wormStartPos : hole.snakeStartPos;

        if (targetSprite != null)
        {
            hole.currentIsWorm = isWorm;
            hole.visualCoroutine = StartCoroutine(PopUpAnimation(holeIndex, hole, targetSprite, startPos, isWorm, targetBeatTime));
        }
    }

    private IEnumerator PopUpAnimation(int holeIndex, HoleSprites hole, GameObject spriteToAnimate, Vector3 startPos, bool isWorm, float targetBeatTime)
    {
        hole.isAnimating = true;
        hole.isInputWindowOpen = false; 
        
        spriteToAnimate.SetActive(true);
        spriteToAnimate.transform.localPosition = startPos;
        Vector3 targetPos = startPos + Vector3.up * popUpHeight;
        
        float timeUntilBeat = targetBeatTime - Time.unscaledTime;
        if (timeUntilBeat > animationDuration)
        {
            yield return new WaitForSecondsRealtime(timeUntilBeat - animationDuration);
        }
        
        if (isWorm) PlayWormUpSound(); 
        else { PlaySnakeUpSound(); PlaySnakeHissSound(); }
        
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / animationDuration);
            spriteToAnimate.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        spriteToAnimate.transform.localPosition = targetPos;
        
        hole.isInputWindowOpen = true;
        
        if (hole.logicCoroutine != null) StopCoroutine(hole.logicCoroutine);
        hole.logicCoroutine = StartCoroutine(InputFailureTimer(hole, holeIndex, quarterNoteTime));

        float timeSpentSoFar = Time.unscaledTime - (targetBeatTime - animationDuration);
        float visualHangTime = quarterNoteTime - timeSpentSoFar;
        
        if (visualHangTime > 0)
        {
            yield return new WaitForSecondsRealtime(visualHangTime);
        }

        if (isWorm) PlayWormDownSound(); else PlaySnakeDownSound();
        
        GameObject currentVisibleSprite = spriteToAnimate;
        if(hole.smileWormSprite != null && hole.smileWormSprite.activeSelf) 
            currentVisibleSprite = hole.smileWormSprite;

        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / animationDuration);
            currentVisibleSprite.transform.localPosition = Vector3.Lerp(targetPos, startPos, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        currentVisibleSprite.transform.localPosition = startPos;
        currentVisibleSprite.SetActive(false);
        
        hole.isAnimating = false;
        hole.visualCoroutine = null;
    }

    private IEnumerator InputFailureTimer(HoleSprites hole, int holeIndex, float duration)
    {
        float elapsed = 0f;
        while(elapsed < duration)
        {
            if (!hole.isInputWindowOpen) yield break;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (hole.isInputWindowOpen)
        {
            FailedInput(holeIndex, "Timed out");
        }

        hole.isInputWindowOpen = false;
        hole.logicCoroutine = null;
    }

    private void SwapToSmileWorm(int holeIndex)
    {
        HoleSprites hole = holeSprites[holeIndex];
        
        if (hole.wormSprite != null && hole.smileWormSprite != null)
        {
            hole.smileWormSprite.transform.localPosition = hole.wormSprite.transform.localPosition;
            hole.wormSprite.SetActive(false);
            hole.smileWormSprite.SetActive(true);
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
        if (spr != null) { Color c = spr.color; c.a = alpha; spr.color = c; }
        else if (img != null) { Color c = img.color; c.a = alpha; img.color = c; }
    }

    public void ResetAllPositions()
    {
        StopAllCoroutines();

        foreach (var hole in holeSprites)
        {
            hole.isAnimating = false;
            hole.isInputWindowOpen = false;
            hole.visualCoroutine = null;
            hole.logicCoroutine = null;

            if (hole.idleSprite != null) hole.idleSprite.transform.localPosition = hole.idleStartPos;
            if (hole.snakeSprite != null) { hole.snakeSprite.transform.localPosition = hole.snakeStartPos; hole.snakeSprite.SetActive(false); }
            if (hole.wormSprite != null) { hole.wormSprite.transform.localPosition = hole.wormStartPos; hole.wormSprite.SetActive(false); }
            if (hole.smileWormSprite != null) { hole.smileWormSprite.transform.localPosition = hole.smileWormStartPos; hole.smileWormSprite.SetActive(false); }
            if (hole.ghostSprite != null) { hole.ghostSprite.transform.localPosition = hole.ghostStartPos; hole.ghostSprite.SetActive(false); SetGhostAlpha(hole.ghostSprite.GetComponent<SpriteRenderer>(), hole.ghostSprite.GetComponent<Image>(), 1f); }
        }
        
        lastBouncedTick = -1;
        gameActive = false;
        beatsSinceStart = 0;
        
        currentScore = 0;
        UpdateScoreDisplay();
        
        tutorialUpShown = false;
        tutorialDownShown = false;
        
        if (flashPanel != null) flashPanel.SetActive(false);
        if (wormHitEffectObject != null) { wormHitEffectObject.transform.localScale = wormTargetScale; wormHitEffectObject.SetActive(false); }
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