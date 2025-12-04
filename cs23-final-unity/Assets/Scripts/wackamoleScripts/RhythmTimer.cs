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
    public int curr_qNote;
    public int curr_sNote;
    public bool isPlaying = false;

    // For timing accuracy
    private double startDspTime;
    private double songStartTime;

    void Update()
    {
        if (isPlaying && musicSource != null)
        {
            if (musicSource.isPlaying)
            {
                // Use both musicSource.time and dspTime for accuracy
                time_in_song = musicSource.time;
                
                // Calculate based on actual elapsed time
                curr_tick = ((int)(time_in_song * (bpm / 60) * 4));
                curr_meas = curr_tick / 16;
                curr_qNote = (curr_tick % 16) / 4;
                curr_sNote = curr_tick % 4;
            }
        }
    }

    public void StartMusic()
    {
        isPlaying = true;
        if (musicSource != null && !musicSource.isPlaying)
        {
            startDspTime = AudioSettings.dspTime;
            songStartTime = startDspTime + 0.1f; // Small buffer
            
            // Schedule playback for accurate timing
            musicSource.PlayScheduled(songStartTime);
            Debug.Log($"Music scheduled to start at DSP time: {songStartTime}");
        }
    }

    public void Pause()
    {
        isPlaying = false;
        if (musicSource != null) 
        {
            musicSource.Pause();
        }
    }

    public void Resume()
    {
        isPlaying = true;
        if (musicSource != null) 
        {
            musicSource.UnPause();
        }
    }

    public void Stop()
    {
        isPlaying = false;
        if (musicSource != null) musicSource.Stop();
    }
    
    // Get exact time since song start (more accurate than musicSource.time)
    public double GetExactSongTime()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            return musicSource.time;
        }
        return 0;
    }
}