using UnityEngine;
using System.Collections;

public class TEST_PLAYER_MANAGER : MonoBehaviour
{
    private const float beatDuration = 0.5f;

    public GameHandler gameHandler;
    public Leader_Manager leaderManager;

    [Header("Direction GameObjects (all at same position)")]
    public GameObject idleSprite;
    public GameObject arrowUp;
    public GameObject arrowDown;
    public GameObject arrowLeft;
    public GameObject arrowRight;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip missSound;
    private AudioSource audioSource;

    void Start()
    {
        ShowIdle();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        HandleVisibility();
    }

    public IEnumerator StartSequence(float bpm, int beats, int[] arr)
    {
        float timer = 0f;
        bool[] inputScored = new bool[4];
        bool[] missPlayed = new bool[4];
        int score = 0;

        float SEQUENCE_DURATION = (60 / bpm) * beats;

        float[] windowStarts = new float[] { -0.05f, beatDuration, beatDuration * 2, beatDuration * 3 };
        float[] windowEnds = new float[] { beatDuration, beatDuration * 2, beatDuration * 3, SEQUENCE_DURATION };

        while (timer < SEQUENCE_DURATION)
        {
            for (int i = 0; i < 4; i++)
            {
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

                    switch (arr[i])
                    {
                        case 0:
                            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                            {
                                correctInput = true;
                                arrowName = "UP";
                            }
                            break;
                        case 1:
                            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                            {
                                correctInput = true;
                                arrowName = "DOWN";
                            }
                            break;
                        case 2:
                            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                            {
                                correctInput = true;
                                arrowName = "LEFT";
                            }
                            break;
                        case 3:
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

            yield return null;
            timer += Time.deltaTime;
        }
        
        Debug.Log($"[{Time.time:F2}] PLAYER SEQUENCE 0 ENDED. FINAL SCORE: {score}\n");
    }

    void HandleVisibility()
    {
        // Check keys with priority: Up > Down > Left > Right
        // Only ONE direction shows at a time
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            ShowOnlyOneArrow(arrowUp);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            ShowOnlyOneArrow(arrowDown);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            ShowOnlyOneArrow(arrowLeft);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            ShowOnlyOneArrow(arrowRight);
        }
        else
        {
            // No keys pressed - show idle
            ShowIdle();
        }
    }

    void ShowOnlyOneArrow(GameObject arrowToShow)
    {
        // Hide all first (including idle)
        HideAll();
        
        // Show only the specified direction
        if (arrowToShow != null)
        {
            arrowToShow.SetActive(true);
        }
    }

    void ShowIdle()
    {
        // Hide all directions
        HideAll();
        
        // Show only idle
        if (idleSprite != null)
        {
            idleSprite.SetActive(true);
        }
    }

    void HideAll()
    {
        if (idleSprite != null) idleSprite.SetActive(false);
        if (arrowUp != null) arrowUp.SetActive(false);
        if (arrowDown != null) arrowDown.SetActive(false);
        if (arrowLeft != null) arrowLeft.SetActive(false);
        if (arrowRight != null) arrowRight.SetActive(false);
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}