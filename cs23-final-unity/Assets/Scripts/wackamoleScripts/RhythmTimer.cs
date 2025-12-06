using UnityEngine;

public class RhythmTimer : MonoBehaviour
{
    [Header("Timing")]
    public double bpm = 150; // Changed to 150 BPM
    public AudioSource musicSource;
    
    [Header("Beat Sounds")]
    public AudioClip tambourineSound;
    [Range(0f, 1f)] public float tambourineVolume = 0.5f;
    public bool playTambourineOnBeat = true;
    
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
    private AudioSource tambourineSource;
    private int lastTambourineTick = -1;
    private bool isInitialized = false;

    void Start()
    {
        InitializeTambourine();
    }

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
                
                // Play tambourine on every quarter note (every beat)
                if (playTambourineOnBeat)
                {
                    PlayTambourineOnBeat();
                }
            }
        }
    }

    private void InitializeTambourine()
    {
        if (isInitialized) return;
        
        // Create or get AudioSource for tambourine
        tambourineSource = GetComponent<AudioSource>();
        if (tambourineSource == null)
        {
            tambourineSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure tambourine source
        tambourineSource.playOnAwake = false;
        tambourineSource.loop = false;
        tambourineSource.volume = tambourineVolume;
        
        // Load tambourine sound if not assigned in inspector
        if (tambourineSound == null)
        {
            // Try to load from Resources
            tambourineSound = Resources.Load<AudioClip>("tambourine");
            if (tambourineSound == null)
            {
                // Try common locations
                tambourineSound = Resources.Load<AudioClip>("Audio/tambourine");
                tambourineSound = Resources.Load<AudioClip>("Sounds/tambourine");
                if (tambourineSound == null)
                {
                    Debug.LogWarning("Tambourine sound not found in Resources. Assign it in the inspector or place it in a Resources folder.");
                }
            }
        }
        
        isInitialized = true;
    }

    private void PlayTambourineOnBeat()
    {
        // Check if we're on a new tick that's a quarter note (every 4 ticks)
        if (curr_tick != lastTambourineTick && curr_tick % 4 == 0)
        {
            PlayTambourine();
            lastTambourineTick = curr_tick;
        }
    }

    private void PlayTambourine()
    {
        if (tambourineSource != null && tambourineSound != null && tambourineSource.enabled)
        {
            tambourineSource.PlayOneShot(tambourineSound);
        }
    }

    public void StartMusic()
    {
        isPlaying = true;
        lastTambourineTick = -1; // Reset tick tracking
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
        lastTambourineTick = -1;
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
    
    // Toggle tambourine on/off
    public void ToggleTambourine(bool enable)
    {
        playTambourineOnBeat = enable;
    }
    
    // Adjust tambourine volume
    public void SetTambourineVolume(float volume)
    {
        tambourineVolume = Mathf.Clamp01(volume);
        if (tambourineSource != null)
        {
            tambourineSource.volume = tambourineVolume;
        }
    }
    
    // For debugging
    public void LogTambourineInfo()
    {
        Debug.Log($"Tambourine Sound: {(tambourineSound != null ? tambourineSound.name : "None")}");
        Debug.Log($"Tambourine Source: {(tambourineSource != null ? "Exists" : "Missing")}");
        Debug.Log($"Play on Beat: {playTambourineOnBeat}");
        Debug.Log($"Volume: {tambourineVolume}");
        Debug.Log($"BPM: {bpm}");
    }
}