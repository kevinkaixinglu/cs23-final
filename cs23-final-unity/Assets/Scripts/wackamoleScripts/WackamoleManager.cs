using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class WackamoleManager : BeatmapVisualizerSimple
{
    [System.Serializable]
    public class HoleSprites
    {
        public string holeName;
        public GameObject idleSprite;
        public GameObject snakeSprite;
        public GameObject wormSprite;
        [HideInInspector] public Vector3 snakeStartPos;
        [HideInInspector] public Vector3 wormStartPos;
        [HideInInspector] public Vector3 idleStartPos;
        [HideInInspector] public bool isAnimating = false;
    }

    [Header("Hole Sprites")]
    public List<HoleSprites> holeSprites = new List<HoleSprites>();

    [Header("Animation Settings")]
    public float popUpHeight = 1f;
    public float animationDuration = 0.25f;
    public float holeBounceHeight = 0.5f;

    [Header("Input Settings")]
    public KeyCode[] holeKeys = new KeyCode[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
    public KeyCode snakeActionKey = KeyCode.Z;

    [Header("Screen Flash")]
    public GameObject flashPanel;
    public float flashDuration = 0.2f;

    [Header("Audio")]
    public AudioSource hitSound;
    public AudioSource missSound;

    [Header("Debug")]
    public bool showDebugMessages = true;

    [Header("Timing Settings")]
    public float timingOffset = 0f; // Adjust this to fix timing
    public float inputWindowSize = 0.2f; // Total window size in seconds (centered on beat)

    [Header("Beatmap Settings")]
    [Tooltip("Beat interval for notes (1 = every beat, 2 = every other beat, etc.)")]
    public int noteBeatInterval = 2; // Set to 2 for every other beat

    // This will be calculated based on BPM
    private float quarterNoteTime;
    private int lastBouncedTick = -1;
    
    // Input tracking
    private int currentActiveNote = -1;
    private bool isWormActive = false;
    private int activeHoleIndex = -1;
    private bool inputWindowOpen = false;
    private GameObject activeSprite;
    private Coroutine inputWindowCoroutine;

    // Game state
    private bool gameActive = false;
    private bool waitingForFirstBeat = true;
    private int beatsSinceStart = 0;
    private float beatTime = 0f; // The exact time the beat should happen

    void Start()
    {
        if (showDebugMessages) Debug.Log("WackamoleManager Start called");
        
        // Calculate quarter note time based on BPM
        if (rhythmTimer != null)
        {
            quarterNoteTime = 60f / (float)rhythmTimer.bpm; // 60 seconds / BPM
            if (showDebugMessages) Debug.Log($"Quarter note time: {quarterNoteTime} seconds (BPM: {rhythmTimer.bpm})");
        }
        else
        {
            quarterNoteTime = 0.4f; // Default for 150 BPM
            if (showDebugMessages) Debug.LogWarning("RhythmTimer not found, using default quarter note time: 0.4s (150 BPM)");
        }
        
        // Create a simple beatmap with notes every other beat starting at beat 8
        CreateSimpleBeatmap();
        
        InitializeSprites();
        lastBouncedTick = -1;
        
        if (flashPanel != null)
        {
            flashPanel.SetActive(false);
        }
        
        if (showDebugMessages) Debug.Log("WackamoleManager initialized. Waiting for game to start...");
    }

    private void CreateSimpleBeatmap()
    {
        if (showDebugMessages) Debug.Log("Creating BPM-aware beatmap (every other beat)");
        
        // Get current BPM - NOW 150 BPM!
        float currentBPM = (rhythmTimer != null) ? (float)rhythmTimer.bpm : 150f;
        float secondsPerBeat = 60f / currentBPM;
        
        if (showDebugMessages) 
        {
            Debug.Log($"Current BPM: {currentBPM}");
            Debug.Log($"Seconds per beat: {secondsPerBeat:F3}");
        }
        
        // MUSICAL TIMING (in beats, not seconds!)
        float startBeat = 8f;        // Start at beat 8 (3.2 seconds at 150 BPM)
        float beatInterval = noteBeatInterval; // Use the configurable interval (2 = every other beat)
        int totalNotes = 32;         // Total number of notes
        float endBeat = startBeat + (totalNotes * beatInterval);
        
        // Calculate total measures needed
        int totalMeasures = Mathf.CeilToInt(endBeat / 4);
        beatmapBuilder builder = new beatmapBuilder(totalMeasures);
        
        // Create notes
        for (int noteCounter = 0; noteCounter < totalNotes; noteCounter++)
        {
            float beatPosition = startBeat + (noteCounter * beatInterval);
            
            // Convert beat position to measure and beat
            int measure = Mathf.FloorToInt(beatPosition / 4) + 1;
            int beatInMeasure = Mathf.FloorToInt(beatPosition % 4);
            int beat = beatInMeasure + 1;
            
            // Determine hole type (cycle through 1-8: snakes 1-4, worms 5-8)
            int holeType = (noteCounter % 8) + 1;
            
            // Place the note
            builder.PlaceQuarterNote(measure, beat, holeType);
            
            if (showDebugMessages && noteCounter < 10)
            {
                float timeInSeconds = beatPosition * secondsPerBeat;
                string creatureType = (holeType >= 5) ? "Worm" : "Snake";
                int holeIndex = (holeType <= 4) ? holeType : holeType - 4;
                string[] holeNames = {"Top", "Bottom", "Left", "Right"};
                Debug.Log($"Note {noteCounter + 1}: {creatureType} at {holeNames[holeIndex - 1]} hole");
                Debug.Log($"  Beat Position: {beatPosition:F1} -> Measure {measure}, Beat {beat}");
                Debug.Log($"  Time: {timeInSeconds:F2}s (every {beatInterval} beats)");
            }
        }
        
        npcBeatMap = builder.GetBeatMap();
        
        if (showDebugMessages) 
        {
            Debug.Log($"Beatmap created for BPM: {currentBPM}");
            Debug.Log($"Beat interval: {beatInterval} beats");
            Debug.Log($"Time interval: {beatInterval * secondsPerBeat:F3}s");
            Debug.Log($"First note at: {startBeat * secondsPerBeat:F2}s");
            Debug.Log($"Total notes: {totalNotes}");
        }
    }

    private void InitializeSprites()
    {
        foreach (var hole in holeSprites)
        {
            if (hole.idleSprite != null)
            {
                hole.idleStartPos = hole.idleSprite.transform.localPosition;
            }
            
            if (hole.snakeSprite != null)
            {
                hole.snakeStartPos = hole.snakeSprite.transform.localPosition;
                hole.snakeSprite.SetActive(false); // Start hidden
            }
            
            if (hole.wormSprite != null)
            {
                hole.wormStartPos = hole.wormSprite.transform.localPosition;
                hole.wormSprite.SetActive(false); // Start hidden
            }
            
            if (hole.idleSprite != null)
            {
                hole.idleSprite.SetActive(true);
            }
        }
        
        if (showDebugMessages) Debug.Log("All sprites initialized");
    }

    // Call this from GameManager when game starts
    public void StartGame()
    {
        gameActive = true;
        waitingForFirstBeat = true;
        beatsSinceStart = 0;
        beatTime = 0f;
        
        if (showDebugMessages) Debug.Log("WackamoleManager: Game started!");
    }

    protected override void Update()
    {
        if (!gameActive) return;
        
        base.Update();
        
        // Only check for input if game is active and window is open
        if (inputWindowOpen)
        {
            CheckForInput();
        }
    }

    protected override void OnBeatTriggered(int noteValue)
    {
        // Skip if game isn't active
        if (!gameActive) return;
        
        if (showDebugMessages) Debug.Log($"OnBeatTriggered: {noteValue} at measure {rhythmTimer.curr_meas}, beat {rhythmTimer.curr_qNote}, tick {rhythmTimer.curr_tick}");
        
        // Count beats since start (for debugging)
        beatsSinceStart++;
        
        // Bounce holes on every quarter note (every beat)
        if (rhythmTimer.curr_tick % 4 == 0 && rhythmTimer.curr_tick != lastBouncedTick)
        {
            BounceAllHoles();
            lastBouncedTick = rhythmTimer.curr_tick;
        }
        
        // Skip if noteValue is 0 (no note)
        if (noteValue == 0) 
        {
            if (waitingForFirstBeat)
            {
                if (showDebugMessages) Debug.Log($"Waiting... beat {beatsSinceStart}");
            }
            return;
        }

        // Note values 1-4: Snakes (Top, Bottom, Left, Right)
        // Note values 5-8: Worms (Top, Bottom, Left, Right)
        int holeIndex = (noteValue <= 4) ? (noteValue - 1) : (noteValue - 5);
        bool isWorm = noteValue >= 5;
        
        if (holeIndex >= 0 && holeIndex < holeSprites.Count)
        {
            currentActiveNote = noteValue;
            isWormActive = isWorm;
            activeHoleIndex = holeIndex;
            
            if (showDebugMessages) Debug.Log($"{(isWorm ? "Worm" : "Snake")} appearing at hole {holeIndex} (Key: {holeKeys[holeIndex]})");
            
            // Calculate when the beat should happen
            // This is when the creature should be at the apex
            beatTime = Time.unscaledTime + animationDuration;
            
            StartPopUpAnimation(holeIndex, isWorm);
            waitingForFirstBeat = false;
        }
        else
        {
            if (showDebugMessages) Debug.LogWarning($"Invalid hole index {holeIndex} for note value {noteValue}");
        }
    }

    private void CheckForInput()
    {
        if (currentActiveNote == -1) return;
        
        // Check each arrow key
        for (int i = 0; i < holeKeys.Length; i++)
        {
            if (Input.GetKeyDown(holeKeys[i]))
            {
                HandleArrowKeyPress(i);
                return; // Exit after handling one key press
            }
        }
        
        // Check for Z key press without arrow (only matters for snakes)
        if (!isWormActive && Input.GetKeyDown(snakeActionKey))
        {
            if (showDebugMessages) Debug.Log("WRONG! Pressed Z key but no arrow key for snake");
            FlashScreen(Color.red);
            PlayMissSound();
        }
    }

    private void HandleArrowKeyPress(int pressedHoleIndex)
    {
        if (pressedHoleIndex == activeHoleIndex)
        {
            // Correct hole pressed
            if (isWormActive)
            {
                // Worm: just arrow key, NO Z key allowed!
                if (!Input.GetKey(snakeActionKey)) // Make sure Z is NOT pressed
                {
                    if (showDebugMessages) Debug.Log("SUCCESS! Correct worm input for hole " + pressedHoleIndex);
                    FlashScreen(Color.green);
                    PlayHitSound();
                    CloseInputWindow();
                }
                else
                {
                    if (showDebugMessages) Debug.Log("WRONG! Should NOT press Z for worm");
                    FlashScreen(Color.red);
                    PlayMissSound();
                }
            }
            else
            {
                // Snake: need Z + arrow key
                if (Input.GetKey(snakeActionKey))
                {
                    if (showDebugMessages) Debug.Log("SUCCESS! Correct snake input for hole " + pressedHoleIndex + " with Z key");
                    FlashScreen(Color.green);
                    PlayHitSound();
                    CloseInputWindow();
                }
                else
                {
                    if (showDebugMessages) Debug.Log("WRONG! Correct hole for snake but missing Z key");
                    FlashScreen(Color.red);
                    PlayMissSound();
                }
            }
        }
        else
        {
            // Wrong hole pressed
            if (showDebugMessages) Debug.Log("WRONG! Pressed key for hole " + pressedHoleIndex + " but expected hole " + activeHoleIndex);
            FlashScreen(Color.red);
            PlayMissSound();
        }
    }

    private void CloseInputWindow()
    {
        inputWindowOpen = false;
        currentActiveNote = -1;
        
        // Stop the input window coroutine if it's running
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
            StartCoroutine(DoFlashScreen(color));
        }
    }

    private IEnumerator DoFlashScreen(Color color)
    {
        flashPanel.SetActive(true);
        
        UnityEngine.UI.Image flashImage = flashPanel.GetComponent<UnityEngine.UI.Image>();
        if (flashImage != null)
        {
            flashImage.color = new Color(color.r, color.g, color.b, 0.3f); // Lower alpha for less intense flash
        }
        
        yield return new WaitForSecondsRealtime(flashDuration);
        
        flashPanel.SetActive(false);
    }

    private void PlayHitSound()
    {
        if (hitSound != null)
        {
            hitSound.Play();
        }
    }

    private void PlayMissSound()
    {
        if (missSound != null)
        {
            missSound.Play();
        }
    }

    private void BounceAllHoles()
    {
        foreach (var hole in holeSprites)
        {
            if (hole.idleSprite != null)
            {
                StartCoroutine(BounceHole(hole));
            }
        }
    }

    private IEnumerator BounceHole(HoleSprites hole)
    {
        Vector3 startPos = hole.idleStartPos;
        Vector3 bouncePos = startPos + Vector3.up * holeBounceHeight;
        
        hole.idleSprite.transform.localPosition = bouncePos;
        yield return new WaitForSecondsRealtime(0.05f); // Use realtime for short animations
        hole.idleSprite.transform.localPosition = startPos;
    }

    private void StartPopUpAnimation(int holeIndex, bool isWorm)
    {
        var hole = holeSprites[holeIndex];
        
        if (hole.isAnimating) 
        {
            if (showDebugMessages) Debug.LogWarning($"Hole {holeIndex} is already animating, skipping new animation");
            return;
        }
            
        GameObject targetSprite = isWorm ? hole.wormSprite : hole.snakeSprite;
        Vector3 startPos = isWorm ? hole.wormStartPos : hole.snakeStartPos;

        if (targetSprite != null)
        {
            activeSprite = targetSprite;
            StartCoroutine(PopUpAnimation(hole, targetSprite, startPos, isWorm));
        }
        else
        {
            if (showDebugMessages) Debug.LogError($"Target sprite is null for hole {holeIndex}, isWorm: {isWorm}");
        }
    }

    private IEnumerator PopUpAnimation(HoleSprites hole, GameObject sprite, Vector3 startPos, bool isWorm)
    {
        hole.isAnimating = true;
        
        // Show the sprite
        sprite.SetActive(true);
        sprite.transform.localPosition = startPos;
        
        Vector3 targetPos = startPos + Vector3.up * popUpHeight;
        
        // Calculate exact timing
        float currentTime = Time.unscaledTime;
        float windowHalf = inputWindowSize / 2f;
        
        // When should the apex be? It should be at beatTime
        // But we need to adjust the animation start so apex is at beatTime
        float timeUntilBeat = beatTime - currentTime;
        
        if (showDebugMessages) 
        {
            Debug.Log($"Animation starting at: {currentTime:F3}");
            Debug.Log($"Target beat/apex at: {beatTime:F3}");
            Debug.Log($"Time until apex: {timeUntilBeat:F3}s");
            Debug.Log($"Animation duration: {animationDuration}s");
        }
        
        // If we have more time than needed for animation, wait before starting
        if (timeUntilBeat > animationDuration)
        {
            float waitTime = timeUntilBeat - animationDuration;
            yield return new WaitForSecondsRealtime(waitTime);
        }
        
        // Now start the animation - it will take animationDuration to reach apex
        // Should reach apex very close to beatTime
        
        // Move up to apex
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            sprite.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        sprite.transform.localPosition = targetPos;
        
        // We should now be at apex
        float apexTime = Time.unscaledTime;
        
        // Calculate input window times (centered on beatTime)
        float windowStartTime = beatTime - windowHalf;
        float windowEndTime = beatTime + windowHalf;
        
        if (showDebugMessages) 
        {
            Debug.Log($"Reached apex at: {apexTime:F3}");
            Debug.Log($"Window: {windowStartTime:F3} to {windowEndTime:F3}");
            Debug.Log($"Window center (beatTime): {beatTime:F3}");
        }
        
        // Check if we're already in or past the window
        if (apexTime < windowStartTime)
        {
            // Wait for window to start
            float waitTime = windowStartTime - apexTime;
            if (showDebugMessages) Debug.Log($"Waiting {waitTime:F3}s for window to start");
            yield return new WaitForSecondsRealtime(waitTime);
            
            // Open window for full duration
            if (showDebugMessages) Debug.Log($"INPUT WINDOW OPEN (0.2s) for {(isWorm ? "Worm" : "Snake")} at hole {activeHoleIndex}");
            inputWindowOpen = true;
            inputWindowCoroutine = StartCoroutine(InputWindowTimer(inputWindowSize));
        }
        else if (apexTime < windowEndTime)
        {
            // We're already in the window
            float remainingWindow = windowEndTime - apexTime;
            if (showDebugMessages) Debug.Log($"Already in window! Opening for remaining {remainingWindow:F3}s");
            inputWindowOpen = true;
            inputWindowCoroutine = StartCoroutine(InputWindowTimer(remainingWindow));
        }
        else
        {
            // Already past the window
            if (showDebugMessages) Debug.Log("Missed window entirely!");
            FlashScreen(Color.red);
            PlayMissSound();
            CloseInputWindow();
        }
        
        // Stay at apex for the full beat duration
        float timeSpentSoFar = Time.unscaledTime - (beatTime - animationDuration);
        float remainingApexTime = quarterNoteTime - timeSpentSoFar;
        
        if (remainingApexTime > 0)
        {
            if (showDebugMessages) Debug.Log($"Staying at apex for {remainingApexTime:F3}s");
            yield return new WaitForSecondsRealtime(remainingApexTime);
        }
        
        // Close window if still open
        CloseInputWindow();
        
        // Move back down
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            sprite.transform.localPosition = Vector3.Lerp(targetPos, startPos, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        sprite.transform.localPosition = startPos;
        
        // Hide the sprite
        sprite.SetActive(false);

        hole.isAnimating = false;
    }

    private IEnumerator InputWindowTimer(float windowDuration)
    {
        // Use unscaled time for input window
        float startTime = Time.unscaledTime;
        
        if (showDebugMessages) Debug.Log($"Input window open for {windowDuration} seconds (centered on beat)");
        
        while (Time.unscaledTime - startTime < windowDuration && inputWindowOpen)
        {
            yield return null;
        }
        
        // Close input window and check for missed input
        if (inputWindowOpen && currentActiveNote != -1)
        {
            if (showDebugMessages) Debug.Log($"MISSED! No input for {(isWormActive ? "Worm" : "Snake")} at hole {activeHoleIndex}");
            FlashScreen(Color.red);
            PlayMissSound();
            CloseInputWindow();
        }
    }

    // For debugging timing - call this method or add to OnGUI
    void OnGUI()
    {
        if (showDebugMessages && rhythmTimer != null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 350, 220));
            GUILayout.Label($"Music Time: {rhythmTimer.musicSource.time:F3}");
            GUILayout.Label($"Unscaled Time: {Time.unscaledTime:F3}");
            GUILayout.Label($"Current Tick: {rhythmTimer.curr_tick}");
            GUILayout.Label($"Quarter Note Time: {quarterNoteTime:F3}s");
            GUILayout.Label($"Input Window Size: {inputWindowSize:F3}s");
            GUILayout.Label($"Beats Since Start: {beatsSinceStart}");
            
            if (currentActiveNote != -1)
            {
                float timeToBeat = beatTime - Time.unscaledTime;
                GUILayout.Label($"Active Note: {currentActiveNote}");
                GUILayout.Label($"Active Hole: {activeHoleIndex}");
                GUILayout.Label($"Is Worm: {isWormActive}");
                GUILayout.Label($"Input Window: {inputWindowOpen}");
                GUILayout.Label($"Time to Beat: {timeToBeat:F3}s");
                GUILayout.Label($"Window Center: {beatTime:F3}");
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
            if (hole.idleSprite != null)
                hole.idleSprite.transform.localPosition = hole.idleStartPos;
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
        }
        
        lastBouncedTick = -1;
        currentActiveNote = -1;
        inputWindowOpen = false;
        activeSprite = null;
        inputWindowCoroutine = null;
        gameActive = false;
        beatsSinceStart = 0;
        beatTime = 0f;
        
        if (flashPanel != null)
        {
            flashPanel.SetActive(false);
        }
        
        if (showDebugMessages) Debug.Log("All positions reset");
    }
    
    // Call this when pausing
    public void Pause()
    {
        gameActive = false;
        // Also pause any running animations
        if (inputWindowCoroutine != null)
        {
            StopCoroutine(inputWindowCoroutine);
            inputWindowCoroutine = null;
        }
    }
    
    // Call this when resuming
    public void Resume()
    {
        gameActive = true;
    }
    
    // Test method for timing adjustment
    public void AdjustTiming(float offset)
    {
        timingOffset = offset;
        if (showDebugMessages) Debug.Log($"Timing offset set to: {timingOffset}");
    }
    
    // Method to adjust input window size
    public void AdjustWindowSize(float size)
    {
        inputWindowSize = size;
        if (showDebugMessages) Debug.Log($"Input window size set to: {inputWindowSize}");
    }
    
    // Method to get timing information for debugging
    public void LogTimingInfo()
    {
        if (rhythmTimer != null)
        {
            Debug.Log($"BPM: {rhythmTimer.bpm}");
            Debug.Log($"Quarter Note Time: {quarterNoteTime}");
            Debug.Log($"Music Time: {rhythmTimer.musicSource.time}");
            Debug.Log($"Current Measure: {rhythmTimer.curr_meas}");
            Debug.Log($"Current Beat: {rhythmTimer.curr_qNote}");
        }
    }
}