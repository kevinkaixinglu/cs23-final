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
    private int winningScore = 18;

    private string victorySceneName = "LevelComplete";
    private string failureSceneName = "LevelFailed";
    private bool hasFinished = false;

    [Header("Song:")]
    public double bpm;
    private double startingBPM;
    public AudioSource musicSource;
    public AudioSource idleMusic;
    public float songDuration = 94f;

    [Header("Single Key Input:")]
    public KeyCode singleInputKey = KeyCode.A;

    [Header("Player Animation")]
    public GameObject playerIdleSprite;
    public GameObject playerActiveSprite;

    //judgy bird bools
    public bool judgyBirds = false;
    private int judgyBirdsEndTick = -1; // Track when to turn off judgy birds

    public Animator cloudPulse;
    public Animator linePulse;

    [Header("Good Input Animation")]
    public Animator goodInputAnimator;

    private int last_qNote = -1;

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
    private int inputWindowTicks = 1;
    private int inputWindowTicksBefore = 1;
    private int last_key_press_tick = -1;

    private double accumulatedTicks = 0;
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

        idleMusic.loop = true;
        idleMusic.Stop();
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
        lastBPMBeforeChange = startingBPM;
        last_key_press_tick = -999;
        hasFinished = false;
        currScore = 0;
        judgyBirds = false;
        judgyBirdsEndTick = -1;

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
            HandleKeyInput();
            UpdateTiming();
            
            if (curr_tick != last_tick && curr_qNote >= 0 && curr_sNote >= 0 && curr_meas >= 0)
            {
                HandleBeatAnimations();
                HandleNoteExpiration();
                HandleWindowMiss();
                CheckJudgyBirdsTimeout(); // Check if judgy birds should end
                CheckForNewNote();
                last_tick = curr_tick;
            }

            HandleInput();
            UpdateScoreDisplay();
        }
    }

    private void CheckJudgyBirdsTimeout()
    {
        // Turn off judgy birds after 3 ticks
        if (judgyBirds && judgyBirdsEndTick != -1 && curr_tick >= judgyBirdsEndTick)
        {
            judgyBirds = false;
            judgyBirdsEndTick = -1;
            Debug.Log($"Judgy birds deactivated at tick {curr_tick}");
        }
    }

    private void HandleKeyInput()
    {
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
    }

    private void UpdateTiming()
    {
        time_in_song = musicSource.time;
        double timeSinceLastBPMChange = time_in_song - lastBPMChangeTime;
        double ticksInCurrentPeriod = timeSinceLastBPMChange * (bpm / 60.0) * 4.0;
        double totalTicksDouble = accumulatedTicks + ticksInCurrentPeriod;
        
        curr_tick = (int)totalTicksDouble;
        curr_meas = curr_tick / 16;
        curr_qNote = ((curr_tick % 16) / 4);
        curr_sNote = curr_tick % 4;
    }

    private void HandleBeatAnimations()
    {
        if (curr_qNote != last_qNote)
        {
            last_qNote = curr_qNote;
            
            if (curr_qNote % 2 == 0)
            {
                linePulse.Play("slowPump", 0, 0f);
            }
            cloudPulse.Play("pump", 0, 0f);
        }
    }

    private void HandleNoteExpiration()
    {
        if (window_start_tick != -1 && curr_tick == window_start_tick + current_note_duration)
        {
            Debug.Log($"[{Time.time:F2}] Note duration ended at tick {curr_tick} (duration was {current_note_duration}) - forcing idle");
            force_idle_until_release = true;
            ShowPlayerIdle();
        }
    }

    private void HandleWindowMiss()
    {
        if (waiting_for_input && curr_tick == window_start_tick + inputWindowTicks + 1)
        {
            Debug.Log($"[{Time.time:F2}] BOO! Missed the window (ended at tick {window_start_tick + inputWindowTicks})");
            judgyBirds = true;
            judgyBirdsEndTick = curr_tick + 3; // Turn off after 3 ticks
            Debug.Log($"MISSED INPUT - Judgy birds activated until tick {judgyBirdsEndTick}!");
            waiting_for_input = false;
        }
    }

    private void CheckForNewNote()
    {
        if (beat_map == null || curr_meas >= beat_map.Length) return;
        
        int next_input = beat_map[curr_meas].qNotes[curr_qNote].sNotes[curr_sNote];
        
        if (next_input == 0) return;
        
        bool isNewNote = (curr_sNote == 0) || 
                         (beat_map[curr_meas].qNotes[curr_qNote].sNotes[curr_sNote - 1] != next_input);
        
        if (!isNewNote) return;
        
        OpenInputWindow(next_input);
    }

    private void OpenInputWindow(int noteValue)
    {
        window_start_tick = curr_tick;
        current_note_duration = CalculateNoteDuration(curr_meas, curr_qNote, curr_sNote, noteValue);
        force_idle_until_release = false;
        
        Debug.Log($"[{Time.time:F2}] WINDOW OPENING at tick {curr_tick} (measure {curr_meas}, qNote {curr_qNote}, sNote {curr_sNote}) - Input window: {inputWindowTicksBefore} tick(s) before + {inputWindowTicks} tick(s) after, Note duration: {current_note_duration} sixteenths");
        
        // Check if key is currently being held
        if (key_pressed)
        {
            int ticksSincePress = curr_tick - last_key_press_tick;
            
            // Check if the press was within the valid early window
            if (ticksSincePress >= 0 && ticksSincePress <= inputWindowTicksBefore)
            {
                Debug.Log($"[{Time.time:F2}] YAY! Good timing (pressed {ticksSincePress} tick(s) early)! Score: {currScore}");
                AddScore();
                TriggerGoodInputAnimation();
                waiting_for_input = false;
            }
            else
            {
                Debug.Log($"[{Time.time:F2}] WINDOW OPEN! (but key held too long - pressed {ticksSincePress} ticks ago, limit is {inputWindowTicksBefore})");
                waiting_for_input = false;
            }
        }
        else
        {
            Debug.Log($"[{Time.time:F2}] WINDOW OPEN! (valid for {inputWindowTicks} more tick(s))");
            waiting_for_input = true;
        }
    }

    private void HandleInput()
    {
        if (!waiting_for_input || !key_just_pressed_this_tick) return;
        
        int ticksFromWindowStart = curr_tick - window_start_tick;
        
        // Check if within valid window (exact or late)
        bool isExactInput = (ticksFromWindowStart == 0);
        bool isLateInput = (ticksFromWindowStart > 0 && ticksFromWindowStart <= inputWindowTicks);
        
        if (isExactInput || isLateInput)
        {
            AddScore();
            TriggerGoodInputAnimation();
            
            if (isExactInput)
                Debug.Log($"[{Time.time:F2}] YAY! Perfect timing! Score: {currScore}");
            else
                Debug.Log($"[{Time.time:F2}] YAY! Good timing (pressed {ticksFromWindowStart} tick(s) late)! Score: {currScore}");
            
            waiting_for_input = false;
        }
    }

    private void TriggerGoodInputAnimation()
    {
        if (goodInputAnimator != null)
        {
            goodInputAnimator.Play("Good_Input", 0, 0f);
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.SetText($"SCORE: {currScore}");
        }
    }

    void CheckBPMChanges()
    {
        double currentTime = musicSource.time;
        
        while (currentBPMChangeIndex < bpmChanges.Length && 
               currentTime >= bpmChanges[currentBPMChangeIndex].timeInSeconds)
        {
            double timeSinceLastChange = bpmChanges[currentBPMChangeIndex].timeInSeconds - lastBPMChangeTime;
            double ticksInPeriod = timeSinceLastChange * (lastBPMBeforeChange / 60.0) * 4.0;
            accumulatedTicks += ticksInPeriod;
            
            Debug.Log($"[BPM CHANGE] At {bpmChanges[currentBPMChangeIndex].timeInSeconds}s: " +
                      $"Added {ticksInPeriod:F2} ticks from previous period (BPM {lastBPMBeforeChange}). " +
                      $"Total accumulated: {accumulatedTicks:F2} (measure {(int)(accumulatedTicks / 16)})");
            
            lastBPMChangeTime = bpmChanges[currentBPMChangeIndex].timeInSeconds;
            lastBPMBeforeChange = bpmChanges[currentBPMChangeIndex].newBPM;
            bpm = bpmChanges[currentBPMChangeIndex].newBPM;
            
            Debug.Log($"[{Time.time:F2}] BPM changed to {bpm}");
            
            if (bpm >= 160)
            {
                inputWindowTicks = 0;
                inputWindowTicksBefore = 2;
                Debug.Log($"[INPUT WINDOW] Changed to: {inputWindowTicksBefore} tick(s) before + {inputWindowTicks} tick(s) after");
            }
            else
            {
                inputWindowTicks = 1;
                inputWindowTicksBefore = 0;
            }
            
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
                PlayerPrefs.SetInt("Level1Passed", 1); 
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
            idleMusic.Play();
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
            idleMusic.Stop();
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