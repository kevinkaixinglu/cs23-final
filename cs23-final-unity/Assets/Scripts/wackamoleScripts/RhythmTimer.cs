using UnityEngine;

public class RhythmTimer : MonoBehaviour
{
    [Header("Timing")]
    public double bpm = 120;
    public AudioSource musicSource;
    
    [Header("Current Position (Read Only)")]
    public double time_in_song;
    public int curr_tick;
    public int curr_meas;
    public int curr_beat;
    public int curr_note;
    public bool isPlaying = true;

    void Start()
    {
        if (musicSource != null)
        {
            double startTime = AudioSettings.dspTime;
            musicSource.PlayScheduled(startTime);
        }
    }

    void Update()
    {
        if (isPlaying && musicSource != null)
        {
            time_in_song = musicSource.time;
            curr_tick = ((int)(time_in_song * (bpm / 60) * 4)) - 1;
            curr_meas = (curr_tick) / 16;
            curr_beat = ((curr_tick % 16) / 4);
            curr_note = curr_tick % 4;
        }
    }

    public void Pause()
    {
        isPlaying = false;
        if (musicSource != null) musicSource.Pause();
    }

    public void Resume()
    {
        isPlaying = true;
        if (musicSource != null) musicSource.UnPause();
    }
}