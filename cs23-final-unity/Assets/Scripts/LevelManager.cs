using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelManager : MonoBehaviour
{

    public AudioSource musicSource;
    public double bpm;
    public double beats_per_seq;
    public double init_delay;

    private double musicStartTime;
    private double nextSeqTime;
    private bool isPlaying = false;
    private double pausedTimeOffset = 0;

    public Leader_Manager leaderManager;
    public Player_Manager playerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        musicSource.Play();
        nextSeqTime = AudioSettings.dspTime + init_delay; // slight delay to be safe
        isPlaying = true;

    }

    // Update is called once per frame
    void Update()
    {

        if (!isPlaying) return;

        double dspTime = AudioSettings.dspTime;

        // SEQUENCE TRIGGERED
        if (dspTime >= nextSeqTime)
        {
            Debug.Log($"[{Time.time:F2}] Sequence trigger");
            nextSeqTime += (60 / bpm) * beats_per_seq;

            // CALL LEADER/PLAYER FUNCTIONS HERE. Perhaps we can call leader,
            //  retreive the sequence, then call player afterwards, passing
            //  the sequence over.

        }

    }
    public void PauseMusic()
    {
        if (!isPlaying) return;

        musicSource.Pause();
        isPlaying = false;

        pausedTimeOffset = nextSeqTime - AudioSettings.dspTime;
    }

    public void UnPauseMusic()
    {
        if (isPlaying) return;

        // Schedule music to start 1 second from now
        //musicStartTime = AudioSettings.dspTime;
        //musicSource.PlayScheduled(musicStartTime);

        Debug.Log("Music unpaused");
        musicSource.UnPause();

        // Schedule first beat at the same time the music begins
        //nextSeqTime = musicStartTime;
        nextSeqTime = AudioSettings.dspTime + pausedTimeOffset;
        isPlaying = true;
    }

    public void Pause()
    {
        PauseMusic();
        Time.timeScale = 0f;
        return;
    }

    public void Resume()
    {
        UnPauseMusic();
        Time.timeScale = 1f;
        return;
    }

}
