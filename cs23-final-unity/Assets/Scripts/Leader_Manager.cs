using UnityEngine;
using System.Collections;

public class Leader_Manager : MonoBehaviour
{
    //Assign all Sprites + Gameobjects
    [Header("Arrow GameObjects")]
    public GameObject arrowUp;
    public GameObject arrowDown;
    public GameObject arrowLeft;
    public GameObject arrowRight;

    [Header("Arrow Sprites")]
    public Sprite arrowUpOff;
    public Sprite arrowUpOn;
    public Sprite arrowDownOff;
    public Sprite arrowDownOn;
    public Sprite arrowLeftOff;
    public Sprite arrowLeftOn;
    public Sprite arrowRightOff;
    public Sprite arrowRightOn;

    // --- Game Timing Controls (Set by Sequence Map) ---
    [Header("Game Timing Controls")]
    // Time the player gets to prepare after the leader finishes
    public float playerPrepTime = 0.0f; 
    // Time the game waits after the player finishes before the leader starts the next sequence
    public float cycleRestTime = 4.0f; 
    
    public bool startNextSequence = false;
    public bool playerTurn = false;
    private int i = 0; // The sequence indexvg 

    // Song duration tracking
    private float songStartTime = 0f;
    private const float SONG_DURATION = 50f; // Song lasts 55 seconds
    private bool songEnded = false;

    // Expose current sequence for Player to read when leader hands off
    public int currentSequenceIndex = 0;

    // Store the current random arrow sequence (0=Up, 1=Down, 2=Left, 3=Right)
    public int[] currentArrowSequence = new int[4];

    // Define the base beat duration for 120 BPM (0.5 seconds per beat)
    private const float beatDuration = 0.5f; 

    void Start()
    {
        ResetArrows();
        
        // Start a coroutine to handle the initial 8-second delay
        StartCoroutine(InitialDelayAndStart());
    }

    // Coroutine for Initial Delay
    IEnumerator InitialDelayAndStart()
    {
        // Wait for 8.0 seconds before setting startNextSequence to true
        Debug.Log($"[{Time.time:F2}] Waiting 8.0 seconds before starting game loop...");
        yield return new WaitForSeconds(7.75f);
        
        songStartTime = Time.time; // Record when the song/game actually starts
        startNextSequence = true;
        Debug.Log($"[{Time.time:F2}] Initial delay finished. Starting game loop (sequence index {i}). Song will end at {songStartTime + SONG_DURATION:F2}s");
    }
    
    void Update()
    {
        // Check if song has ended
        if (!songEnded && songStartTime > 0 && Time.time >= songStartTime + SONG_DURATION)
        {
            songEnded = true;
            Debug.Log($"[{Time.time:F2}] SONG ENDED! Game loop stopping after {i} sequences.");
        }

        // If it is not the player's turn and the sequence should be started (and song hasn't ended)
        if (startNextSequence == true && playerTurn == false && !songEnded)
        {
            // Always use Sequence 0 timing - loop indefinitely
            playerPrepTime = 0.0f; // player starts immediately after leader
            cycleRestTime = 4.0f;  // 4 second gap between sequences
            
            Debug.Log($"[{Time.time:F2}] Set timing for Sequence {i}. Prep: {playerPrepTime}s, Rest: {cycleRestTime}s");

            // Start the sequence
            startNextSequence = false;
            ResetArrows();
            
            // Always run SequenceZero with random arrows
            StartCoroutine(StartSequence(0));

            // Increment counter for logging purposes
            i++;
        }
    }

    // Turn all arrows off
    void ResetArrows()
    {
        arrowUp.GetComponentInChildren<SpriteRenderer>().sprite = arrowUpOff;
        arrowDown.GetComponentInChildren<SpriteRenderer>().sprite = arrowDownOff;
        arrowLeft.GetComponentInChildren<SpriteRenderer>().sprite = arrowLeftOff;
        arrowRight.GetComponentInChildren<SpriteRenderer>().sprite = arrowRightOff;
    }

    IEnumerator StartSequence(int sequenceIndex)
    {
        // Always run SequenceZero with random arrows
        Debug.Log($"[{Time.time:F2}] Leader: starting SequenceZero (iteration {i}).");
        yield return StartCoroutine(SequenceZero());
        
        // After Leader Demo, hand off to Player
        currentSequenceIndex = 0;
        playerTurn = true;

        // Reset arrows after the leader sequence before handing off (visual cleanup)
        ResetArrows();
    }

    // ----------------------
    // Leader sequences
    // ----------------------

    IEnumerator SequenceZero()
    {
        // Generate 4 random arrows (0=Up, 1=Down, 2=Left, 3=Right)
        for (int j = 0; j < 4; j++)
        {
            currentArrowSequence[j] = Random.Range(0, 4);
        }
        
        Debug.Log($"[{Time.time:F2}] Leader Seq0: Random sequence generated: " +
                  $"[{currentArrowSequence[0]}, {currentArrowSequence[1]}, {currentArrowSequence[2]}, {currentArrowSequence[3]}]");
        
        SpriteRenderer sr;
        
        // Flash duration: shorter than beat so arrows visibly turn off between beats
        const float FLASH_DURATION = 0.3f; // Arrow stays on for 0.3s
        const float GAP_DURATION = beatDuration - FLASH_DURATION; // 0.2s gap before next beat
        
        // Play each of the 4 random arrows
        for (int beatIndex = 0; beatIndex < 4; beatIndex++)
        {
            ResetArrows();
            
            // Select the arrow based on random choice
            switch (currentArrowSequence[beatIndex])
            {
                case 0: // UP
                    sr = arrowUp.GetComponentInChildren<SpriteRenderer>();
                    sr.sprite = arrowUpOn;
                    Debug.Log($"[{Time.time:F2}] Leader Seq0 Beat{beatIndex + 1} (UP) ON");
                    break;
                case 1: // DOWN
                    sr = arrowDown.GetComponentInChildren<SpriteRenderer>();
                    sr.sprite = arrowDownOn;
                    Debug.Log($"[{Time.time:F2}] Leader Seq0 Beat{beatIndex + 1} (DOWN) ON");
                    break;
                case 2: // LEFT
                    sr = arrowLeft.GetComponentInChildren<SpriteRenderer>();
                    sr.sprite = arrowLeftOn;
                    Debug.Log($"[{Time.time:F2}] Leader Seq0 Beat{beatIndex + 1} (LEFT) ON");
                    break;
                case 3: // RIGHT
                    sr = arrowRight.GetComponentInChildren<SpriteRenderer>();
                    sr.sprite = arrowRightOn;
                    Debug.Log($"[{Time.time:F2}] Leader Seq0 Beat{beatIndex + 1} (RIGHT) ON");
                    break;
            }
            
            // Wait for flash duration
            yield return new WaitForSeconds(FLASH_DURATION);
            
            // Turn off the arrow to create visible gap
            ResetArrows();
            
            // Wait for the gap before next beat (if not the last beat)
            if (beatIndex < 3)
            {
                yield return new WaitForSeconds(GAP_DURATION);
            }
            else
            {
                // On the last beat, still wait the gap so timing is consistent
                yield return new WaitForSeconds(GAP_DURATION);
            }
        }

        Debug.Log($"[{Time.time:F2}] Leader Seq0 complete.");
    }
}