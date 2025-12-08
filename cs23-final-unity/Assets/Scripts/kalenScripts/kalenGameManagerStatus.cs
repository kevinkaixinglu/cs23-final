using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class kalenGameManagerStatus : MonoBehaviour
{
    [Header("Pausing the Game")]
    public GameObject pauseMenuUI;
    public GameObject pauseButton;  // NEW: Add pause button reference
    public GameObject infoPageUI;
    public AudioMixer mixer;
    public Slider volumeSlider;
    public static float volumeLevel = 1.0f;

    private kalenGameManager gameHandler;
    private static bool GameisPaused = false;

    private bool InfoPage = false;

    public void Start()
    {
        gameHandler = GetComponent<kalenGameManager>();

        if (gameHandler == null)
        {
            Debug.LogError("KalenGameHandler component not found on this GameObject!");
            return;
        }

        SetVolume();
        Debug.Log("Starting Game...");
        pauseMenuUI.SetActive(false);
        
        // NEW: Hide pause button at start
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }

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
                
                // NEW: Show pause button when game starts
                if (pauseButton != null)
                {
                    pauseButton.SetActive(true);
                }
                
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
            pauseMenuUI.SetActive(false);
            GameisPaused = false;
            gameHandler.Resume();
        }
        else
        {
            pauseMenuUI.SetActive(true);
            GameisPaused = true;
            gameHandler.Pause();
        }
    }

    public void SetVolume()
    {
        if (mixer != null)
        {
            float value = volumeSlider.value;
            float clampedValue = Mathf.Clamp(value, 0.0001f, 1f);
            mixer.SetFloat("MusicVolume", Mathf.Log10(clampedValue) * 20);
            volumeLevel = value;
        }
        else
        {
            Debug.LogWarning("AudioMixer is not assigned in PauseMenuHandler! Please assign it in the Inspector.");
        }
    }
}