using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class ManageGame : MonoBehaviour
{
    [Header("Expected Player Input Map:")]
    public Measure[] beat_map;
    public MakeBeatmap special_beatmap; // COMMENTED OUT - Special script pertaining to this level

    [Header("Score Keeping:")]
    public GameObject score;
    public TextMeshProUGUI scoreText;
    public int currScore = 0;

    [Header("Song:")]
    public double bpm;
    public AudioSource musicSource;

    [Header("Input Keys:")]
    public KeyCode[] key = new KeyCode[4];

    [Header("Song Status (FOR ACCESS ONLY)")]
    public double time_in_song;
    public int curr_tick;
    public int curr_meas;
    public int curr_qNote;
    public int curr_sNote;
    public bool isPlaying = false;

    private bool[] key_pressed = new bool[4]; // Used to stop player from holding down button
    private bool[] waiting_for_input = new bool[4];

    private int last_tick = -1; // Used to record note changes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        double startTime = AudioSettings.dspTime;
        musicSource.PlayScheduled(startTime);
        beat_map = special_beatmap.SpecialBeatMap(); // COMMENTED OUT
        isPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKey(key[i]))
                {
                    if (!key_pressed[i]) // Was this the first occurence of key 1 (No holding it down)
                    {
                        Debug.Log($"[{Time.time:F2}] Key " + (i + 1) + " hit");
                        if (waiting_for_input[i]) // Are we supposed to have hit the key?
                        {
                            currScore++;
                            Debug.Log($"[{Time.time:F2}] YAY!"); // We hit our window
                            waiting_for_input[i] = false;
                        }
                        else
                        {
                            Debug.Log($"[{Time.time:F2}] Incorrect input!"); // We hit when there wasn't a window
                        }
                        key_pressed[i] = true; // Track to not record future inputs when we hold it down
                        break;
                    }
                }
                else
                {
                    key_pressed[i] = false;
                }
            }

            double sec_per_tick = 60 / bpm / 4;
            time_in_song = musicSource.time - sec_per_tick / 4; // This subtraction allows for slightly early inputs
            curr_tick = ((int)(time_in_song * (bpm / 60) * 4)) - 1; // tick = sixteenthNote relative to whole song
            curr_meas = (curr_tick) / 16;
            curr_qNote = ((curr_tick % 16) / 4);
            curr_sNote = curr_tick % 4;

            if (curr_tick != last_tick && curr_qNote >= 0 && curr_sNote >= 0 && curr_meas >= 0)
            {
                for (int i = 0; i < 4; i++) //For all four input keys
                {
                    if (waiting_for_input[i])
                    {
                        currScore--;
                        Debug.Log($"[{Time.time:F2}] BOO!"); // Missed our window
                        waiting_for_input[i] = false;
                    }
                }


                int next_input = beat_map[curr_meas].qNotes[curr_qNote].sNotes[curr_sNote];
                if (next_input != 0)
                {
                    //Note the "%4". This allows user to record any positive number, which
                    //  can allow differenciation between numbers like 4 and 8, which will
                    //  map to the same input key, but may pertain to different animations.
                    waiting_for_input[(next_input - 1) % 4] = true; // Make window for input
                    Debug.Log($"[{Time.time:F2}] WINDOW OPEN!"); // Missed our window
                }
                

                last_tick = curr_tick; // Wait until we get to the next tick (tick defined above)
            }
            scoreText.SetText(currScore.ToString());
        }
    }

    public void Pause()
    {
        if (isPlaying)
        {
            Debug.Log("Pausing...");
            musicSource.Pause();
            score.SetActive(false);
            isPlaying = false;
            Time.timeScale = 0f;
        }
    }

    public void Resume()
    {
        if (!isPlaying)
        {
            Debug.Log("Resuming...");
            musicSource.UnPause();
            score.SetActive(true);
            isPlaying = true;
            Time.timeScale = 1f;
        }
    }

    public void addScore()
    {
        currScore++;
        scoreText.text = "SCORE: " + currScore.ToString();
    }

}