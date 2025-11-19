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

    [Header("Player Animation")]
    public GameObject playerIdleSprite;
    public GameObject playerActiveSprite;

    [Header("Song Status (FOR ACCESS ONLY)")]
    public double time_in_song;
    public int curr_tick;
    public int curr_meas;
    public int curr_qNote;
    public int curr_sNote;
    public bool isPlaying = false;

    private bool key_pressed = false;
    private bool key_just_pressed_this_tick = false; // NEW: Track if key was pressed THIS tick
    private bool waiting_for_input = false;
    private bool force_idle_until_release = false;
    private int window_start_tick = -1;
    private int current_note_duration = 4;
    private int last_tick = -1;

    void Start()
    {
        ShowPlayerIdle();
    }

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
        
        ShowPlayerIdle();
    }

    void Update()
    {
        if (isPlaying)
        {
            // Reset the "just pressed this tick" flag at the start of each frame
            key_just_pressed_this_tick = false;

            // Single key input handling
            if (Input.GetKey(singleInputKey))
            {
                // Show active sprite UNLESS we're forcing idle
                if (force_idle_until_release)
                {
                    ShowPlayerIdle();
                }
                else
                {
                    ShowPlayerActive();
                }
                
                if (!key_pressed) // First press
                {
                    key_just_pressed_this_tick = true; // Mark that key was pressed THIS tick
                    Debug.Log($"[{Time.time:F2}] Key {singleInputKey} pressed (tick: {curr_tick})");
                    key_pressed = true;
                }
            }
            else
            {
                // Key is released
                if (key_pressed) // Just released
                {
                    Debug.Log($"[{Time.time:F2}] Key {singleInputKey} released (tick: {curr_tick})");
                    ShowPlayerIdle();
                    force_idle_until_release = false;
                }
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
                // Check if animation duration has ended - force sprite to idle
                if (window_start_tick != -1 && curr_tick == window_start_tick + current_note_duration)
                {
                    Debug.Log($"[{Time.time:F2}] Note duration ended at tick {curr_tick} (duration was {current_note_duration}) - forcing idle");
                    force_idle_until_release = true;
                    ShowPlayerIdle();
                }

                // Check if we missed the window - only on the tick right after window opened
                if (waiting_for_input && curr_tick == window_start_tick + 1)
                {
                    currScore--;
                    Debug.Log($"[{Time.time:F2}] BOO! Missed the window");
                    waiting_for_input = false;
                }

                // Check for new notes - ONLY on the first sixteenth note of each beat
                if (beat_map != null && curr_meas < beat_map.Length && curr_sNote == 0)
                {
                    int next_input = beat_map[curr_meas].qNotes[curr_qNote].sNotes[curr_sNote];
                    
                    if (next_input != 0)
                    {
                        // New window is opening
                        window_start_tick = curr_tick;
                        
                        // Calculate note duration
                        current_note_duration = CalculateNoteDuration(curr_meas, curr_qNote, curr_sNote, next_input);
                        
                        Debug.Log($"[{Time.time:F2}] WINDOW OPENING at tick {curr_tick} - Note duration: {current_note_duration} sixteenths");
                        
                        // Check if key was pressed EXACTLY on this tick (the window opening tick)
                        if (key_just_pressed_this_tick)
                        {
                            currScore++;
                            Debug.Log($"[{Time.time:F2}] YAY! Correct input!");
                            waiting_for_input = false;
                        }
                        else if (key_pressed)
                        {
                            // Key was already held from before - no points
                            Debug.Log($"[{Time.time:F2}] WINDOW OPEN! (but key already held - won't score)");
                            waiting_for_input = false;
                        }
                        else
                        {
                            // No key pressed yet - window is open and waiting
                            Debug.Log($"[{Time.time:F2}] WINDOW OPEN!");
                            waiting_for_input = true;
                        }
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

    private int CalculateNoteDuration(int meas, int qNote, int sNote, int noteValue)
    {
        int duration = 0;
        int currentMeas = meas;
        int currentQNote = qNote;
        int currentSNote = sNote;

        while (currentMeas < beat_map.Length)
        {
            if (currentQNote >= beat_map[currentMeas].qNotes.Length)
                break;
            if (currentSNote >= beat_map[currentMeas].qNotes[currentQNote].sNotes.Length)
                break;

            int value = beat_map[currentMeas].qNotes[currentQNote].sNotes[currentSNote];
            
            if (value == noteValue)
            {
                duration++;
                currentSNote++;
                if (currentSNote >= 4)
                {
                    currentSNote = 0;
                    currentQNote++;
                    if (currentQNote >= 4)
                    {
                        currentQNote = 0;
                        currentMeas++;
                    }
                }
            }
            else
            {
                break;
            }
        }

        return duration;
    }

    private void ShowPlayerIdle()
    {
        if (playerIdleSprite != null) playerIdleSprite.SetActive(true);
        if (playerActiveSprite != null) playerActiveSprite.SetActive(false);
    }

    private void ShowPlayerActive()
    {
        if (playerIdleSprite != null) playerIdleSprite.SetActive(false);
        if (playerActiveSprite != null) playerActiveSprite.SetActive(true);
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