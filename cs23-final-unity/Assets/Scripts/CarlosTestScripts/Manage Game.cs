using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class ManageGame : MonoBehaviour
{

    [Header("Score Keeping")]
    public GameObject score;
    public TextMeshProUGUI scoreText;
    public int currScore = 0;

    [Header("Song and Expected Player Input")]
    public double bpm;
    public AudioSource musicSource;
    public Measure[] beat_map;

    private double last_pause; // Track the dsp time we last paused at
    private double total_time_paused = 0; // To follow how behind dspTime we are
    private bool isPlaying = false;
    private bool waiting_for_input = false;

    private int last_tick = -1; // Used to record note changes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        double startTime = AudioSettings.dspTime;
        musicSource.PlayScheduled(startTime);
        total_time_paused = AudioSettings.dspTime;
        isPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if(Input.GetKey(KeyCode.Space) && waiting_for_input)
            {
                Debug.Log("YAY!"); // Hit our window
                waiting_for_input = false;
            }

            double offset = 60 / bpm / 4; // So window opens slightly early
            double time_in_song = AudioSettings.dspTime - total_time_paused + offset;
            int curr_tick = ((int)(time_in_song * (bpm / 60) * 4)) - 4; // tick = note relative to whole song
            int curr_meas = (curr_tick) / 16;
            int curr_beat = ((curr_tick % 16) / 4);
            int curr_note = curr_tick % 4;
            if (curr_tick != last_tick && curr_note >= 0 && curr_beat >= 0 && curr_meas >= 0)
            {

                if (curr_beat == 0 && curr_note == 0)
                {
                    Debug.Log("Time: " + time_in_song + " Measure: " + curr_meas + ", Beat: " + curr_beat + ", Note: " + curr_note);
                }

                if (waiting_for_input)
                {
                    Debug.Log("BOO!"); // Missed our window
                    waiting_for_input = false;
                }

                if (beat_map[curr_meas].beats[curr_beat].notes[curr_note])
                {
                    waiting_for_input = true; // Make window
                }

                last_tick = curr_tick;
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
            last_pause = AudioSettings.dspTime;
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
            total_time_paused += AudioSettings.dspTime - last_pause;
            Time.timeScale = 1f;
        }
    }

    public void addScore()
    {
        currScore++;
        scoreText.text = "SCORE: " + currScore.ToString();
    }

}
