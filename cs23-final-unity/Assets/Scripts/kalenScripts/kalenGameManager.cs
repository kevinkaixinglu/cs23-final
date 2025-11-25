using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class kalenGameManager : MonoBehaviour
{
    [Header("Expected Player Input Map:")]
    public Measure[] beat_map;
    public MakeBeatmap special_beatmap;

    [Header("Score Keeping:")]
    public GameObject score;
    public TextMeshProUGUI scoreText;
    public int currScore = 0;
    public int winningScore = 5;

    private string victorySceneName = "LevelCompleteScene";
    private string failureSceneName = "LevelFailed";
    private bool hasFinished = false;

    [Header("Song:")]
    public double bpm;
    private double startingBPM;
    public AudioSource musicSource;
    public float songDuration = 90f;

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
    private bool key_just_pressed_this_tick = false;
    private bool waiting_for_input = false;
    private bool force_idle_until_release = false;
    private int window_start_tick = -1;
    private int current_note_duration = 4;
    private int last_tick = -1;
    private int inputWindowTicks = 2;
    private int inputWindowTicksBefore = 1;
    private int last_key_press_tick = -1;

    // BPM tracking across changes
    private int accumulatedTicks = 0;
    private double lastBPMChangeTime = 0;
    private double lastBPMBeforeChange = 0;

    [System.Serializable]
    public class BPMChange
    {
        public double timeInSeconds;
        public double newBPM;
    }

    private BPMChange[] bpmChanges;
    private int currentBPMChangeIndex = 0;

    void Start()
    {
        ShowPlayerIdle();
        InitializeBPMChanges();
    }

    void InitializeBPMChanges()
    {
        bpmChanges = new BPMChange[]
        {
            new BPMChange { timeInSeconds = 48.0, newBPM = 150 },
            new BPMChange { timeInSeconds = 49.6, newBPM = 155 },
            new BPMChange { timeInSeconds = 51.148, newBPM = 160 },
            new BPMChange { timeInSeconds = 52.648, newBPM = 165 },
            new BPMChange { timeInSeconds = 54.103, newBPM = 170 },
            new BPMChange { timeInSeconds = 72.632, newBPM = 165 },
            new BPMChange { timeInSeconds = 73.905, newBPM = 160 },
            new BPMChange { timeInSeconds = 75.405, newBPM = 155 },
            new BPMChange { timeInSeconds = 76.954, newBPM = 150 }
        };
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

        startingBPM = bpm;
        currentBPMChangeIndex = 0;
        accumulatedTicks = 0;
        lastBPMChangeTime = 0;
        lastBPMBeforeChange = 0;
        last_key_press_tick = -999;
        hasFinished = false;
        currScore = 0;

        double startTime = AudioSettings.dspTime;
        musicSource.PlayScheduled(startTime);
        isPlaying = true;
        
        ShowPlayerIdle();
        
        StartCoroutine(CheckSongEnd());
    }

    void Update()
    {
        if (isPlaying)
        {
            CheckBPMChanges();

            key_just_pressed_this_tick = false;

            if (Input.GetKey(singleInputKey))
            {
                if (force_idle_until_release)
                {
                    ShowPlayerIdle();
                }
                else
                {
                    ShowPlayerActive();
                }
                
                if (!key_pressed)
                {
                    key_just_pressed_this_tick = true;
                    last_key_press_tick = curr_tick;
                    Debug.Log($"[{Time.time:F2}] Key {singleInputKey} pressed (tick: {curr_tick})");
                    key_pressed = true;
                }
            }
            else
            {
                if (key_pressed)
                {
                    Debug.Log($"[{Time.time:F2}] Key {singleInputKey} released (tick: {curr_tick})");
                    ShowPlayerIdle();
                    force_idle_until_release = false;
                }
                key_pressed = false;
            }

            // Calculate current time in song
            time_in_song = musicSource.time;

            // Calculate ticks in current BPM period
            double timeSinceLastBPMChange = time_in_song - lastBPMChangeTime;
            int ticksInCurrentPeriod = (int)(timeSinceLastBPMChange * (bpm / 60) * 4);

            // Total ticks = accumulated from previous periods + current period
            curr_tick = accumulatedTicks + ticksInCurrentPeriod;

            curr_meas = curr_tick / 16;
            curr_qNote = ((curr_tick % 16) / 4);
            curr_sNote = curr_tick % 4;

            if (curr_tick != last_tick && curr_qNote >= 0 && curr_sNote >= 0 && curr_meas >= 0)
            {
                if (window_start_tick != -1 && curr_tick == window_start_tick + current_note_duration)
                {
                    Debug.Log($"[{Time.time:F2}] Note duration ended at tick {curr_tick} (duration was {current_note_duration}) - forcing idle");
                    force_idle_until_release = true;
                    ShowPlayerIdle();
                }

                if (waiting_for_input && curr_tick == window_start_tick + inputWindowTicks + 1)
                {
                    Debug.Log($"[{Time.time:F2}] BOO! Missed the window (ended at tick {window_start_tick + inputWindowTicks})");
                    waiting_for_input = false;
                }

                if (beat_map != null && curr_meas < beat_map.Length)
                {
                    int next_input = beat_map[curr_meas].qNotes[curr_qNote].sNotes[curr_sNote];
                    
                    bool isNewNote = false;
                    
                    if (next_input != 0)
                    {
                        if (curr_sNote == 0)
                        {
                            isNewNote = true;
                        }
                        else
                        {
                            int prevValue = beat_map[curr_meas].qNotes[curr_qNote].sNotes[curr_sNote - 1];
                            isNewNote = (prevValue != next_input);
                        }
                    }
                    
                    if (isNewNote)
                    {
                        window_start_tick = curr_tick;
                        current_note_duration = CalculateNoteDuration(curr_meas, curr_qNote, curr_sNote, next_input);
                        force_idle_until_release = false;  
                        
                        Debug.Log($"[{Time.time:F2}] WINDOW OPENING at tick {curr_tick} - Input window: {inputWindowTicksBefore} tick(s) before + {inputWindowTicks} tick(s) after, Note duration: {current_note_duration} sixteenths");
                        
                        int ticksSinceLastPress = curr_tick - last_key_press_tick;
                        
                        if (key_just_pressed_this_tick)
                        {
                            AddScore();
                            Debug.Log($"[{Time.time:F2}] YAY! Perfect timing (pressed exactly on opening tick)! Score: {currScore}");
                            waiting_for_input = false;
                        }
                        else if (ticksSinceLastPress > 0 && ticksSinceLastPress <= inputWindowTicksBefore)
                        {
                            AddScore();
                            Debug.Log($"[{Time.time:F2}] YAY! Good timing (pressed {ticksSinceLastPress} tick(s) early)! Score: {currScore}");
                            waiting_for_input = false;
                        }
                        else if (key_pressed)
                        {
                            Debug.Log($"[{Time.time:F2}] WINDOW OPEN! (but key held too long - won't score)");
                            waiting_for_input = false;
                        }
                        else
                        {
                            Debug.Log($"[{Time.time:F2}] WINDOW OPEN! (valid for {inputWindowTicks} more tick(s))");
                            waiting_for_input = true;
                        }
                    }
                }

                last_tick = curr_tick;
            }

            if (waiting_for_input && key_just_pressed_this_tick)
            {
                int ticksFromStart = curr_tick - window_start_tick;
                
                if (ticksFromStart <= inputWindowTicks)
                {
                    AddScore();
                    Debug.Log($"[{Time.time:F2}] YAY! Correct input! (pressed {ticksFromStart} tick(s) after window opened) Score: {currScore}");
                    waiting_for_input = false;
                }
            }
                            
            if (scoreText != null)
            {
                scoreText.SetText($"SCORE: {currScore}");
            }
        }
    }

    void CheckBPMChanges()
    {
        double currentTime = musicSource.time;
        
        while (currentBPMChangeIndex < bpmChanges.Length && 
               currentTime >= bpmChanges[currentBPMChangeIndex].timeInSeconds)
        {
            // Calculate ticks accumulated during the previous BPM period
            double timeSinceLastChange = bpmChanges[currentBPMChangeIndex].timeInSeconds - lastBPMChangeTime;
            double bpmToUse = (lastBPMBeforeChange > 0) ? lastBPMBeforeChange : startingBPM;
            int ticksInPeriod = (int)(timeSinceLastChange * (bpmToUse / 60) * 4);
            accumulatedTicks += ticksInPeriod;
            
            Debug.Log($"[BPM CHANGE] At {bpmChanges[currentBPMChangeIndex].timeInSeconds}s: " +
                      $"Added {ticksInPeriod} ticks from previous period (BPM {bpmToUse}). " +
                      $"Total accumulated: {accumulatedTicks}");
            
            lastBPMChangeTime = bpmChanges[currentBPMChangeIndex].timeInSeconds;
            lastBPMBeforeChange = bpm;
            bpm = bpmChanges[currentBPMChangeIndex].newBPM;
            
            Debug.Log($"[{Time.time:F2}] BPM changed to {bpm}");
            currentBPMChangeIndex++;
        }
    }

    private IEnumerator CheckSongEnd()
    {
        Debug.Log($"[SONG] Waiting for {songDuration} seconds...");
        yield return new WaitForSeconds(songDuration);
        
        if (!hasFinished)
        {
            hasFinished = true;
            isPlaying = false;
            
            Debug.Log($"[SONG END] Song finished! Final score: {currScore}");
            
            if (currScore >= winningScore)
            {
                Debug.Log($"[VICTORY] Player won with {currScore} points!");
                ShowVictoryCinematic();
            }
            else
            {
                Debug.Log($"[FAILURE] Player failed with {currScore} points (needed {winningScore})");
                ShowFailureCinematic();
            }
        }
    }

    private void AddScore()
    {
        currScore++;
        Debug.Log($"[SCORE] Score increased to: {currScore}");
    }

    private void ShowVictoryCinematic()
    {
        Debug.Log($"[VICTORY] Loading scene: {victorySceneName}");
        SceneManager.LoadScene(victorySceneName);
    }

    private void ShowFailureCinematic()
    {
        Debug.Log($"[FAILURE] Loading scene: {failureSceneName}");
        SceneManager.LoadScene(failureSceneName);
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
        AddScore();
    }
}