using UnityEngine;
using System.Collections;


public class Player_Manager : MonoBehaviour
{
    //Assign sprites/gameobjects
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

    // Define the base beat duration for 120 BPM (0.5 seconds per beat)
    private const float beatDuration = 0.5f;

    // Must be assigned in the Inspector to the Leader_Manager object
    public Leader_Manager leaderManager;

    void Start()
    {
        ResetArrows();
    }

    void Update()
    {
        // If leader manager tells us it's player turn, start
        if (leaderManager.playerTurn == true)
        {
            // Capture the sequence index to run the appropriate player sequence
            int seqIndex = leaderManager.currentSequenceIndex;

            // Turn off the playerTurn flag immediately and start the sequence coroutine
            leaderManager.playerTurn = false;
            
            ResetArrows();
            
            // Start the player sequence corresponding to the leader's sequence index.
            StartCoroutine(StartSequence(seqIndex)); 
        }
    }

    void ResetArrows()
    {
        arrowUp.GetComponentInChildren<SpriteRenderer>().sprite = arrowUpOff;
        arrowDown.GetComponentInChildren<SpriteRenderer>().sprite = arrowDownOff;
        arrowLeft.GetComponentInChildren<SpriteRenderer>().sprite = arrowLeftOff;
        arrowRight.GetComponentInChildren<SpriteRenderer>().sprite = arrowRightOff;
    }

    IEnumerator StartSequence(int sequenceIndex)
    {
        // Always run SequenceZero
        Debug.Log($"[{Time.time:F2}] Player: Starting Sequence 0 reaction.");
        yield return StartCoroutine(SequenceZero());
        
        // After the Player sequence is complete (including the post-sequence gap), 
        // signal the Leader to start the next sequence.
        Debug.Log($"[{Time.time:F2}] Player: completed sequence {sequenceIndex}. Waiting rest (leader cycleRestTime = {leaderManager.cycleRestTime}s) before signaling leader.");
        yield return new WaitForSeconds(leaderManager.cycleRestTime);
        leaderManager.startNextSequence = true;
    }

    IEnumerator SequenceZero()
    {
        float timer = 0f;
        bool[] inputScored = new bool[4]; // Track which inputs have been scored
        int score = 0;
        
        // Get the random sequence from the leader (0=Up, 1=Down, 2=Left, 3=Right)
        int[] expectedSequence = leaderManager.currentArrowSequence;
        
        Debug.Log($"[{Time.time:F2}] Player: Starting Player Sequence (Seq 0). Expected: " +
                  $"[{expectedSequence[0]}, {expectedSequence[1]}, {expectedSequence[2]}, {expectedSequence[3]}]\n");

        // 1. Wait for the prep time set in Leader_Manager
        yield return new WaitForSeconds(leaderManager.playerPrepTime); 

        // 2. Player's action time is 4 beats (4 * 0.5s = 2.0s)
        const float SEQUENCE_DURATION = beatDuration * 4; // 2.0s
        
        // Define 4 CONSECUTIVE input windows
        float[] windowStarts = new float[] { -0.05f, beatDuration, beatDuration * 2, beatDuration * 3 };
        float[] windowEnds = new float[] { beatDuration, beatDuration * 2, beatDuration * 3, SEQUENCE_DURATION };

        // Run the input loop for the duration of the sequence (2.0s)
        while (timer < SEQUENCE_DURATION) 
        {
            // turn on any keys being pressed (including WASD)
            activateKeys();

            // Check each of the 4 input windows
            for (int i = 0; i < 4; i++)
            {
                if (timer >= windowStarts[i] && timer <= windowEnds[i] && !inputScored[i])
                {
                    bool correctInput = false;
                    string arrowName = "";
                    
                    // Check if the correct arrow for this beat was pressed
                    switch (expectedSequence[i])
                    {
                        case 0: // Expected UP
                            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                            {
                                correctInput = true;
                                arrowName = "UP";
                            }
                            break;
                        case 1: // Expected DOWN
                            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                            {
                                correctInput = true;
                                arrowName = "DOWN";
                            }
                            break;
                        case 2: // Expected LEFT
                            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                            {
                                correctInput = true;
                                arrowName = "LEFT";
                            }
                            break;
                        case 3: // Expected RIGHT
                            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                            {
                                correctInput = true;
                                arrowName = "RIGHT";
                            }
                            break;
                    }
                    
                    if (correctInput)
                    {
                        score++;
                        Debug.Log($"[{Time.time:F2}] Player Seq0: Scored Input {i + 1} ({arrowName}) at timer {timer:F3}s");
                        inputScored[i] = true;
                    }
                }
            }
            
            yield return null; // wait for next frame
            
            // Increment time AFTER all checks have run. This guarantees the 0.0s window is checked.
            timer += Time.deltaTime; 
        }
        
        // 3. Score is counted immediately after the 2.0s window closes.
        Debug.Log($"[{Time.time:F2}] PLAYER SEQUENCE 0 ENDED. FINAL SCORE: {score}\n");
    }

    void activateKeys() {
        SpriteRenderer sr;

        // Activate/deactivate up key (Up Arrow or W)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            sr = arrowUp.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowUpOn;
        } else {
            sr = arrowUp.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowUpOff;
        }

        // Activate/deactivate down key (Down Arrow or S)
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            sr = arrowDown.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowDownOn;
        } else {
            sr = arrowDown.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowDownOff;
        }

        // Activate/deactivate left key (Left Arrow or A)
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            sr = arrowLeft.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowLeftOn;
        } else {
            sr = arrowLeft.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowLeftOff;
        }

        // Activate/deactivate right key (Right Arrow or D)
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            sr = arrowRight.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowRightOn;
        } else {
            sr = arrowRight.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowRightOff;
        }
    }
}