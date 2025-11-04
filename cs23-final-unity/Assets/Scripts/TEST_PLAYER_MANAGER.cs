using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class TEST_PLAYER_MANAGER : MonoBehaviour
{

    private const float beatDuration = 0.5f;

    public GameHandler gameHandler;
    public Leader_Manager leaderManager;

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

    public AudioClip hitSound;
    public AudioClip missSound;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        activateKeys();
    }

    public IEnumerator StartSequence(float bpm, int beats, int[] arr)
    {
        float timer = 0f;
        bool[] inputScored = new bool[4]; // Track which inputs have been scored
        bool[] missPlayed = new bool[4];  // Track which misses have been played
        int score = 0;

        //Debug.Log($"[{Time.time:F2}] Player: Starting Player Sequence (Seq 0). Expected: " +
        //          $"[{expectedSequence[0]}, {expectedSequence[1]}, {expectedSequence[2]}, {expectedSequence[3]}]\n");


        // 2. Player's action time is 4 beats (4 * 0.5s = 2.0s)
        float SEQUENCE_DURATION = (60 / bpm) * beats; // 2.0s

        // Define 4 CONSECUTIVE input windows
        float[] windowStarts = new float[] { -0.05f, beatDuration, beatDuration * 2, beatDuration * 3 };
        float[] windowEnds = new float[] { beatDuration, beatDuration * 2, beatDuration * 3, SEQUENCE_DURATION };

        leaderManager.playerCue.SetActive(true);
        // Run the input loop for the duration of the sequence (2.0s)
        while (timer < SEQUENCE_DURATION)
        {
            // Check each of the 4 input windows
            for (int i = 0; i < 4; i++)
            {
                //play miss sound if missed
                if (timer > windowEnds[i] && !inputScored[i] && !missPlayed[i])
                {
                    PlaySound(missSound);
                    missPlayed[i] = true;
                    Debug.Log($"[{Time.time:F2}] Player Seq0: MISSED Input {i + 1} at timer {timer:F3}s");
                }

                if (timer >= windowStarts[i] && timer <= windowEnds[i] && !inputScored[i])
                {
                    bool correctInput = false;
                    string arrowName = "";

                    // Check if the correct arrow for this beat was pressed
                    switch (arr[i])
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
                        gameHandler.addScore();
                        score++;
                        PlaySound(hitSound);
                        Debug.Log($"[{Time.time:F2}] Player Seq0: Scored Input {i + 1} ({arrowName}) at timer {timer:F3}s");
                        inputScored[i] = true;
                    }
                }
            }

            yield return null; // wait for next frame

            // Increment time AFTER all checks have run. This guarantees the 0.0s window is checked.
            timer += Time.deltaTime;
        }
        leaderManager.playerCue.SetActive(false);
        // 3. Score is counted immediately after the 2.0s window closes.
        Debug.Log($"[{Time.time:F2}] PLAYER SEQUENCE 0 ENDED. FINAL SCORE: {score}\n");
    }

    void ResetArrows()
    {
        arrowUp.GetComponentInChildren<SpriteRenderer>().sprite = arrowUpOff;
        arrowDown.GetComponentInChildren<SpriteRenderer>().sprite = arrowDownOff;
        arrowLeft.GetComponentInChildren<SpriteRenderer>().sprite = arrowLeftOff;
        arrowRight.GetComponentInChildren<SpriteRenderer>().sprite = arrowRightOff;
    }

    void activateKeys()
    {
        SpriteRenderer sr;

        // Activate/deactivate up key (Up Arrow or W)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            sr = arrowUp.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowUpOn;
        }
        else
        {
            sr = arrowUp.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowUpOff;
        }

        // Activate/deactivate down key (Down Arrow or S)
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            sr = arrowDown.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowDownOn;
        }
        else
        {
            sr = arrowDown.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowDownOff;
        }

        // Activate/deactivate left key (Left Arrow or A)
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            sr = arrowLeft.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowLeftOn;
        }
        else
        {
            sr = arrowLeft.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowLeftOff;
        }

        // Activate/deactivate right key (Right Arrow or D)
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            sr = arrowRight.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowRightOn;
        }
        else
        {
            sr = arrowRight.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowRightOff;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

}
