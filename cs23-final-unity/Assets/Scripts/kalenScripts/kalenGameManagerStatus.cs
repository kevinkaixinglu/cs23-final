using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class kalenGameManagerStatus : MonoBehaviour
{
    [Header("Pausing the Game")]
    public GameObject pauseMenuUI;  
    public GameObject infoPageUI;
    public AudioMixer mixer;
    public Slider volumeSlider;

    private kalenGameManager gameHandler;
    private static bool GameisPaused = false;

    private bool InfoPage = false;
    private bool tutorialWasActiveWhenPaused = false; // Track if tutorial was showing when paused

    public void Start()
    {
        gameHandler = GetComponent<kalenGameManager>();

        if (gameHandler == null)
        {
            Debug.LogError("KalenGameHandler component not found on this GameObject!");
            return;
        }

        volumeSlider.value = VolumeDefiner.vol;
        SetVolume();
        Debug.Log("Starting Game...");
        pauseMenuUI.SetActive(false);

        InfoPage = true;
        GameisPaused = false;
        infoPageUI.SetActive(true);
    }

    void OnDestroy()
    {

    }

    void Update()
    {
        if (InfoPage)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                InfoPage = false;
                GameisPaused = false;
                infoPageUI.SetActive(false);
                
                gameHandler.StartGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (GameisPaused)
        {
            // Resuming
            pauseMenuUI.SetActive(false);
            GameisPaused = false;
            
            // If tutorial was active when we paused, go back to tutorial
            if (tutorialWasActiveWhenPaused)
            {
                infoPageUI.SetActive(true);
                InfoPage = true;
                tutorialWasActiveWhenPaused = false;
                // Stop the idle music
                if (gameHandler.idleMusic != null)
                {
                    gameHandler.idleMusic.Stop();
                }
            }
            else
            {
                // Tutorial wasn't active, so resume the game normally
                gameHandler.Resume();
            }
        }
        else
        {
            // Pausing
            pauseMenuUI.SetActive(true);
            GameisPaused = true;
            
            // Check if tutorial is currently active
            if (InfoPage)
            {
                // Tutorial is active, hide it but remember it was showing
                tutorialWasActiveWhenPaused = true;
                infoPageUI.SetActive(false);
                // Play idle music during tutorial pause
                if (gameHandler.idleMusic != null)
                {
                    gameHandler.idleMusic.Play();
                }
            }
            else
            {
                // Tutorial not active, pause the game normally
                tutorialWasActiveWhenPaused = false;
                gameHandler.Pause();
            }
        }
    }

    public void SetVolume()
    {
        if (mixer != null)
        {
            float value = volumeSlider.value;
            // Clamp the value to avoid Log10(0) which is undefined
            float clampedValue = Mathf.Clamp(value, 0.0001f, 1f);
            VolumeDefiner.vol = clampedValue;
            mixer.SetFloat("MusicVolume", Mathf.Log10(VolumeDefiner.vol) * 20);
        }
        else
        {
            Debug.LogWarning("AudioMixer is not assigned in PauseMenuHandler! Please assign it in the Inspector.");
        }
    }
}