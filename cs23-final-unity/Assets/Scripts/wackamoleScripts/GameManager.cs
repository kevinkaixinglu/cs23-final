using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public GameObject infoPageUI;
    public AudioMixer mixer;
    public Slider volumeSlider;

    [Header("Game References")]
    public WackamoleManager wackamoleManager;
    public RhythmTimer rhythmTimer;
    public AudioSource musicSource;
    
    private bool gameIsPaused = false;
    private bool infoPageActive = true;
    private bool musicStarted = false;
    private bool gameStarted = false;

    void Start()
    {
        Debug.Log("GameManager Start");
        
        // Initialize UI states
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (infoPageUI != null) infoPageUI.SetActive(true);
        
        // Set initial game state
        gameIsPaused = true;
        infoPageActive = true;
        musicStarted = false;
        gameStarted = false;
        
        // Setup volume slider
        if (volumeSlider != null && mixer != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
            
            // Load saved volume
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }
        
        Debug.Log("Game paused for instructions. Press Enter to start.");
    }

    void Update()
    {
        // Handle info page (press Enter to start)
        if (infoPageActive && Input.GetKeyDown(KeyCode.Return))
        {
            CloseInfoPage();
        }
        
        // Handle pause menu (press Escape)
        if (!infoPageActive && Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void CloseInfoPage()
    {
        infoPageActive = false;
        if (infoPageUI != null) infoPageUI.SetActive(false);
        
        Debug.Log("Starting game...");
        
        // Start the game
        StartGame();
    }

    void StartGame()
    {
        gameStarted = true;
        
        // Start the Wackamole game
        if (wackamoleManager != null)
        {
            wackamoleManager.StartGame();
        }
        
        // Start the music
        StartMusic();
    }

    void StartMusic()
    {
        if (!musicStarted)
        {
            musicStarted = true;
            
            // Start music through RhythmTimer if available
            if (rhythmTimer != null)
            {
                rhythmTimer.StartMusic();
                Debug.Log("Music started via RhythmTimer");
            }
            // Or start it directly
            else if (musicSource != null)
            {
                musicSource.Play();
                Debug.Log("Music started directly");
            }
        }
    }

    public void PauseGame()
    {
        if (infoPageActive || !gameStarted) return;
        
        gameIsPaused = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        
        Debug.Log("Game Paused");
        
        // DON'T use Time.timeScale = 0 for rhythm games!
        // Instead, pause the audio and stop game logic
        
        if (rhythmTimer != null)
        {
            rhythmTimer.Pause();
        }
        
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
        
        // Notify WackamoleManager
        if (wackamoleManager != null)
        {
            wackamoleManager.Pause();
        }
    }

    public void ResumeGame()
    {
        gameIsPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        
        Debug.Log("Game Resumed");
        
        // Resume the audio
        if (rhythmTimer != null)
        {
            rhythmTimer.Resume();
        }
        
        if (musicStarted && musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
        
        // Notify WackamoleManager
        if (wackamoleManager != null)
        {
            wackamoleManager.Resume();
        }
    }

    public void SetVolume(float volume)
    {
        if (mixer != null)
        {
            float clampedValue = Mathf.Clamp(volume, 0.0001f, 1f);
            mixer.SetFloat("MusicVolume", Mathf.Log10(clampedValue) * 20);
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
            Debug.Log("Volume set to: " + volume);
        }
    }

    public void OnResumeClicked()
    {
        Debug.Log("Resume button clicked");
        ResumeGame();
    }
    
    public void OnRestartClicked()
    {
        Debug.Log("Restart button clicked");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void OnMenuClicked()
    {
        Debug.Log("Menu button clicked");
        SceneManager.LoadScene("MainMenu");
    }
}