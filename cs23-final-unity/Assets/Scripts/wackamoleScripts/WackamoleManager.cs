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

    [Header("Tutorial Text")]
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI goodLuckText;
    public string instructions = "Use ARROW KEYS to hit moles\nPress Z + ARROW for snakes!";

    [Header("Debug")]
    public bool showDebugMessages = true;

    private float quarterNoteTime = 0.5f;
    private int lastBouncedTick = -1;
    
    // Input tracking
    private int currentActiveNote = -1;
    private bool isWormActive = false;
    private int activeHoleIndex = -1;
    private bool inputWindowOpen = false;
    private GameObject activeSprite;
    private Coroutine inputWindowCoroutine;

    // Tutorial state
    private bool tutorialActive = true;

    void Start()
    {
        if (showDebugMessages) Debug.Log("WackamoleManager Start called");
        
        // Initialize text elements
        InitializeTextElements();
        
        if (npcBeatMap == null || npcBeatMap.Length == 0) 
        {
            if (showDebugMessages) Debug.Log("Building simple test beatmap");
            
            int totalMeasures = 62;
            beatmapBuilder builder = new beatmapBuilder(totalMeasures);
            
            // Measures 1-2: Empty (4-second delay)
            
            // Measures 3-62: Simple quarter note patterns
            int measure = 3;
            
            // Pattern 1: Single notes with large gaps (measures 3-10)
            for (int i = 0; i < 8; i++)
            {
                builder.PlaceQuarterNote(measure, 1, 1); // Top Snake on beat 1
                measure++;
            }
            
            // Pattern 2: Alternating top and bottom (measures 11-18)
            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                    builder.PlaceQuarterNote(measure, 1, 1); // Top Snake
                else
                    builder.PlaceQuarterNote(measure, 1, 2); // Bottom Snake
                measure++;
            }
            
            // Pattern 3: All holes in sequence (measures 19-26)
            for (int i = 0; i < 8; i++)
            {
                int hole = (i % 4) + 1; // Cycles through 1,2,3,4
                builder.PlaceQuarterNote(measure, 1, hole);
                measure++;
            }
            
            // Pattern 4: Simple worms (measures 27-34)
            for (int i = 0; i < 8; i++)
            {
                builder.PlaceQuarterNote(measure, 1, 5); // Top Worm
                measure++;
            }
            
            // Pattern 5: Alternating snakes and worms (measures 35-42)
            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                    builder.PlaceQuarterNote(measure, 1, 1); // Top Snake
                else
                    builder.PlaceQuarterNote(measure, 1, 5); // Top Worm
                measure++;
            }
            
            // Pattern 6: Two notes per measure with gap (measures 43-50)
            for (int i = 0; i < 8; i++)
            {
                builder.PlaceQuarterNote(measure, 1, 1); // Top Snake on beat 1
                builder.PlaceQuarterNote(measure, 3, 2); // Bottom Snake on beat 3
                measure++;
            }
            
            // Pattern 7: All holes with worms (measures 51-58)
            for (int i = 0; i < 8; i++)
            {
                int hole = (i % 4) + 5; // Cycles through 5,6,7,8 (worms)
                builder.PlaceQuarterNote(measure, 1, hole);
                measure++;
            }
            
            // Pattern 8: Final simple pattern (measures 59-62)
            for (int i = 0; i < 4; i++)
            {
                builder.PlaceQuarterNote(measure, 1, 1); // Top Snake
                measure++;
            }
            
            npcBeatMap = builder.GetBeatMap();
            if (showDebugMessages) Debug.Log($"Simple test beatmap created with {npcBeatMap.Length} measures");
        }
        
        InitializeSprites();
        lastBouncedTick = -1;
        
        if (flashPanel != null)
        {
            flashPanel.SetActive(false);
        }
        
        // Start tutorial sequence
        StartCoroutine(TutorialSequence());
        
        if (showDebugMessages) Debug.Log("WackamoleManager initialized successfully");
    }

    private void InitializeTextElements()
    {
        // Hide both text elements initially
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
            instructionsText.text = instructions;
        }
        
        if (goodLuckText != null)
        {
            goodLuckText.gameObject.SetActive(false);
            goodLuckText.text = "GOOD LUCK!";
        }
    }

    private IEnumerator TutorialSequence()
    {
        if (showDebugMessages) Debug.Log("Starting tutorial sequence");
        
        // Wait 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Show instructions for 8 seconds
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(true);
            if (showDebugMessages) Debug.Log("Showing instructions");
        }
        
        yield return new WaitForSeconds(8f);
        
        // Hide instructions
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
            if (showDebugMessages) Debug.Log("Hiding instructions");
        }
        
        // Wait 1 second
        yield return new WaitForSeconds(1f);
        
        // Show "Good Luck!" for 0.5 seconds
        if (goodLuckText != null)
        {
            goodLuckText.gameObject.SetActive(true);
            if (showDebugMessages) Debug.Log("Showing Good Luck message");
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Hide "Good Luck!"
        if (goodLuckText != null)
        {
            goodLuckText.gameObject.SetActive(false);
            if (showDebugMessages) Debug.Log("Hiding Good Luck message");
        }
        
        // Wait 0.5 seconds
        yield return new WaitForSeconds(0.5f);
        
        // End tutorial and enable gameplay
        tutorialActive = false;
        if (showDebugMessages) Debug.Log("Tutorial complete - gameplay starting");
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
                hole.snakeSprite.SetActive(true);
            }
            
            if (hole.wormSprite != null)
            {
                hole.wormStartPos = hole.wormSprite.transform.localPosition;
                hole.wormSprite.SetActive(true);
            }
            
            if (hole.idleSprite != null)
            {
                hole.idleSprite.SetActive(true);
            }
        }
        
        if (showDebugMessages) Debug.Log("All sprites initialized");
    }

    protected override void Update()
    {
        base.Update();
        
        // Only check for input if tutorial is complete and window is open
        if (!tutorialActive && inputWindowOpen)
        {
            CheckForInput();
        }
    }

    protected override void OnBeatTriggered(int noteValue)
    {
        // Skip beat processing during tutorial
        if (tutorialActive) return;
        
        if (showDebugMessages) Debug.Log($"OnBeatTriggered: {noteValue} at measure {rhythmTimer.curr_meas}, beat {rhythmTimer.curr_qNote}");
        
        if (rhythmTimer.curr_tick % 4 == 0 && rhythmTimer.curr_tick != lastBouncedTick)
        {
            BounceAllHoles();
            lastBouncedTick = rhythmTimer.curr_tick;
        }
        
        if (noteValue == 0) 
        {
            return;
        }

        int holeIndex = (noteValue <= 4) ? (noteValue - 1) : (noteValue - 5);
        bool isWorm = noteValue >= 5;
        
        if (holeIndex >= 0 && holeIndex < holeSprites.Count)
        {
            currentActiveNote = noteValue;
            isWormActive = isWorm;
            activeHoleIndex = holeIndex;
            
            if (showDebugMessages) Debug.Log($"Animation starting for {(isWorm ? "Worm" : "Snake")} at hole {holeIndex}");
            
            StartPopUpAnimation(holeIndex, isWorm);
        }
        else
        {
            if (showDebugMessages) Debug.LogWarning($"Invalid hole index {holeIndex} for note value {noteValue}");
        }
    }

    private void CheckForInput()
    {
        if (currentActiveNote == -1) return;
        
        for (int i = 0; i < holeKeys.Length; i++)
        {
            if (Input.GetKeyDown(holeKeys[i]))
            {
                if (i == activeHoleIndex)
                {
                    if (isWormActive)
                    {
                        if (showDebugMessages) Debug.Log("SUCCESS! Correct worm input for hole " + i);
                        FlashScreen(Color.green);
                        PlayHitSound();
                        CloseInputWindow();
                        return;
                    }
                    else
                    {
                        if (Input.GetKey(snakeActionKey))
                        {
                            if (showDebugMessages) Debug.Log("SUCCESS! Correct snake input for hole " + i + " with Z key");
                            FlashScreen(Color.green);
                            PlayHitSound();
                            CloseInputWindow();
                            return;
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
                    if (showDebugMessages) Debug.Log("WRONG! Pressed key for hole " + i + " but expected hole " + activeHoleIndex);
                    FlashScreen(Color.red);
                    PlayMissSound();
                }
            }
        }
        
        if (!isWormActive && Input.GetKeyDown(snakeActionKey))
        {
            if (showDebugMessages) Debug.Log("WRONG! Pressed Z key but no arrow key for snake");
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
            flashImage.color = new Color(color.r, color.g, color.b, 0.5f);
        }
        
        yield return new WaitForSeconds(flashDuration);
        
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
        yield return new WaitForSeconds(0.05f);
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
            StartCoroutine(PopUpAnimation(hole, targetSprite, startPos));
        }
        else
        {
            if (showDebugMessages) Debug.LogError($"Target sprite is null for hole {holeIndex}, isWorm: {isWorm}");
        }
    }

    private IEnumerator PopUpAnimation(HoleSprites hole, GameObject sprite, Vector3 startPos)
    {
        hole.isAnimating = true;
        
        Vector3 targetPos = startPos + Vector3.up * popUpHeight;

        // Move up to target position
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            sprite.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sprite.transform.localPosition = targetPos;

        // Wait at apex for one beat duration (0.5 seconds at 120 BPM)
        float apexDuration = quarterNoteTime - animationDuration;
        yield return new WaitForSeconds(apexDuration);

        // Start the input window 0.5 seconds after reaching apex
        // This creates a window from 0.25s before to 0.25s after the target beat
        yield return new WaitForSeconds(0.25f);
        
        // Open input window
        if (showDebugMessages) Debug.Log($"INPUT WINDOW OPEN for {(isWormActive ? "Worm" : "Snake")} at hole {activeHoleIndex}. Press {holeKeys[activeHoleIndex]} {(isWormActive ? "" : "+ " + snakeActionKey)}");
        inputWindowOpen = true;
        
        // Store the coroutine so we can stop it if needed
        inputWindowCoroutine = StartCoroutine(InputWindowTimer());

        // Move back down to start position
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            sprite.transform.localPosition = Vector3.Lerp(targetPos, startPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sprite.transform.localPosition = startPos;

        // Mark this hole as no longer animating
        hole.isAnimating = false;
    }

    private IEnumerator InputWindowTimer()
    {
        // Wait for the input window duration (0.5 seconds total)
        yield return new WaitForSeconds(0.5f);
        
        // Close input window and check for missed input
        if (inputWindowOpen && currentActiveNote != -1)
        {
            if (showDebugMessages) Debug.Log($"MISSED! No input for {(isWormActive ? "Worm" : "Snake")} at hole {activeHoleIndex}");
            FlashScreen(Color.red);
            PlayMissSound();
            CloseInputWindow();
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
                hole.snakeSprite.transform.localPosition = hole.snakeStartPos;
            if (hole.wormSprite != null)
                hole.wormSprite.transform.localPosition = hole.wormStartPos;
        }
        
        lastBouncedTick = -1;
        currentActiveNote = -1;
        inputWindowOpen = false;
        activeSprite = null;
        inputWindowCoroutine = null;
        
        if (flashPanel != null)
        {
            flashPanel.SetActive(false);
        }
        
        // Reset tutorial state
        tutorialActive = true;
        InitializeTextElements();
        
        // Restart tutorial sequence
        StartCoroutine(TutorialSequence());
        
        if (showDebugMessages) Debug.Log("All positions reset");
    }
}