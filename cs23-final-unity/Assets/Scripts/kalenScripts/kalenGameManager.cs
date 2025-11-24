using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class kalenGameManager : MonoBehaviour
{
    [Header("Expected Player Input Map:")]
    public Measure[] beat_map;
    public MakeBeatmap special_beatmap;

    [Header("Score Keeping:")]
    public GameObject score;
    public TextMeshProUGUI scoreText;
    public int currScore = 0;

    [Header("Song:")]
    public double bpm;
    public AudioSource musicSource;

    [Header("Single Key Input:")]
    public KeyCode singleInputKey = KeyCode.A;

    [Header("Song Status (FOR ACCESS ONLY)")]
    public double time_in_song;
    public int curr_tick;
    public int curr_meas;
    public int curr_qNote;
    public int curr_sNote;
    public bool isPlaying = false;

    private bool key_pressed = false; // Single key tracking
    private bool waiting_for_input = false;

    private int last_tick = -1;

    public void StartGame()
    {
        if (musicSource == null)
        {
            Debug.LogError("Music Source is not assigned!");
            return;
        }

        if (special_beatmap != null)
        {
            beat_map = special_beatmap.SpecialBeatMap();
            Debug.Log("Beatmap loaded from special_beatmap");
        }

        double startTime = AudioSettings.dspTime;
        musicSource.PlayScheduled(startTime);
        isPlaying = true;
    }

    void Update()
    {
        if (isPlaying)
        {
            // Single key input handling
            if (Input.GetKey(singleInputKey))
            {
                if (!key_pressed) // First press (no holding)
                {
                    Debug.Log($"[{Time.time:F2}] Key {singleInputKey} pressed");
                    
                    if (waiting_for_input) // Should we have hit the key?
                    {
                        currScore++;
                        Debug.Log($"[{Time.time:F2}] YAY! Correct input!");
                        waiting_for_input = false;
                    }
                    else
                    {
                        Debug.Log($"[{Time.time:F2}] Incorrect input! No window open");
                    }
                    
                    key_pressed = true; // Prevent holding
                }
            }
            else
            {
                key_pressed = false;
            }

            // Calculate timing
            double sec_per_tick = 60 / bpm / 4;
            time_in_song = musicSource.time - sec_per_tick / 4;
            curr_tick = ((int)(time_in_song * (bpm / 60) * 4)) - 1;
            curr_meas = (curr_tick) / 16;
            curr_qNote = ((curr_tick % 16) / 4);
            curr_sNote = curr_tick % 4;

            // Process beat changes
            if (curr_tick != last_tick && curr_qNote >= 0 && curr_sNote >= 0 && curr_meas >= 0)
            {
                // Check if we missed the window
                if (waiting_for_input)
                {
                    currScore--;
                    Debug.Log($"[{Time.time:F2}] BOO! Missed the window");
                    waiting_for_input = false;
                }

                // Check for new notes
                if (beat_map != null && curr_meas < beat_map.Length)
                {
                    int next_input = beat_map[curr_meas].qNotes[curr_qNote].sNotes[curr_sNote];
                    
                    if (next_input != 0) // Any non-zero value triggers input window
                    {
                        waiting_for_input = true;
                        Debug.Log($"[{Time.time:F2}] WINDOW OPEN!");
                    }
                }

                last_tick = curr_tick;
            }
            
            // Update score text
            if (scoreText != null)
            {
                scoreText.SetText(currScore.ToString());
            }
        }
    }

    public void Pause()
    {
        if (isPlaying)
        {
            Debug.Log("Pausing...");
            musicSource.Pause();
            if (score != null) score.SetActive(false);
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
            if (score != null) score.SetActive(true);
            isPlaying = true;
            Time.timeScale = 1f;
        }
    }

    public void addScore()
    {
        currScore++;
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + currScore.ToString();
        }
    }
}