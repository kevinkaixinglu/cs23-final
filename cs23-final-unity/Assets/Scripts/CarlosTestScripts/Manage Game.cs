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

    [Header("Score Keeping:")]
    public GameObject score;
    public TextMeshProUGUI scoreText;
    public int currScore = 0;

    [Header("Song:")]
    public double bpm;
    public AudioSource musicSource;

    [Header("Input Keys:")]
    public KeyCode[] key = new KeyCode[4];

    private bool isPlaying = false;
    private bool[] key_pressed = new bool[4]; // Used to stop player from holding down button
    private bool[] waiting_for_input = new bool[4];

    private int last_tick = -1; // Used to record note changes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        double startTime = AudioSettings.dspTime;
        musicSource.PlayScheduled(startTime);
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
                            Debug.Log($"[{Time.time:F2}] YAY!"); // We hit our window
                            waiting_for_input[i] = false;
                        }
                        key_pressed[i] = true; // Track to not record future inputs when we hold it down
                    }
                }
                else
                {
                    key_pressed[i] = false;
                }
            }

            double time_in_song = musicSource.time;
            int curr_tick = ((int)(time_in_song * (bpm / 60) * 4)) - 1; // tick = note relative to whole song
            int curr_meas = (curr_tick) / 16;
            int curr_beat = ((curr_tick % 16) / 4);
            int curr_note = curr_tick % 4;

            if (curr_tick != last_tick && curr_note >= 0 && curr_beat >= 0 && curr_meas >= 0)
            {

                //if (curr_note == 0) // Testing
                //{
                //    Debug.Log("Time: " + time_in_song + " Measure: " + curr_meas + ", Beat: " + curr_beat + ", Note: " + curr_note);
                //}

                for (int i = 0; i < 4; i++) //For all four input keys
                {
                    if (waiting_for_input[i])
                    {
                        Debug.Log($"[{Time.time:F2}] BOO!"); // Missed our window
                        waiting_for_input[i] = false;
                    }
                }

                int next_input = beat_map[curr_meas].beats[curr_beat].notes[curr_note];
                if (next_input != 0)
                {
                    waiting_for_input[next_input - 1] = true; // Make window for input
                }

                last_tick = curr_tick; // Wait until we get to the next tick (tick defined above)
            }
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
