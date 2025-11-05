using System.Collections;
using Unity.Burst.Intrinsics;
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

    public TEST_LEADER_MANAGER leaderManager;
    public TEST_PLAYER_MANAGER playerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        double startTime = AudioSettings.dspTime;
        //musicSource.Play();
        musicSource.PlayScheduled(startTime);
        nextSeqTime = startTime + init_delay; // slight delay to be safe
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

            //TESTING NEW LEADER MANAGER
            int beats = 4;
            int[] seq = leaderManager.StartSequence((float)(bpm), beats);
            StartCoroutine(Wait_and_Call_Player((float)((60 / bpm) * beats), beats, seq));

        }
    }

    IEnumerator Wait_and_Call_Player(float wait_time, int beats, int[] seq)
    {
        yield return new WaitForSeconds(wait_time);
        StartCoroutine(playerManager.StartSequence((float)bpm, beats, seq));
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
