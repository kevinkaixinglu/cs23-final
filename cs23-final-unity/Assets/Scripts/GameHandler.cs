using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


public class GameHandler : MonoBehaviour
{

    public GameObject pauseMenuUI;
    public static bool GameisPaused = false;
    public AudioMixer mixer;
    public static float volumeLevel = 1.0f;
    public Slider volumeSlider;

    public GameObject score;
    public TextMeshProUGUI scoreText;
    public int currScore = 0;

    public LevelManager levelManager;

    public void Awake()
    {
        SetVolume();
        Resume();
        score.SetActive(true);
    }

    void OnDestroy()
    {

    }

    public void Pause()
    {
        if (!GameisPaused)
        {
            pauseMenuUI.SetActive(true);
            //Time.timeScale = 0f;
            GameisPaused = true;
            //score.SetActive(true);
            Debug.Log("Pausing...");
            levelManager.Pause();
        }
        else { Resume(); }
    }

    public void Resume()
    {
        Debug.Log("Resuming...");
        levelManager.Resume();
        pauseMenuUI.SetActive(false);
        //Time.timeScale = 1f;
        GameisPaused = false;
        //score.SetActive(false);
        //levelHandler.Resume();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameisPaused) { Resume(); }
            else { Pause(); }
        }
    }

    public void SetVolume()
    {
        if (mixer != null)
        {
            float value = volumeSlider.value;
            // Clamp the value to avoid Log10(0) which is undefined
            float clampedValue = Mathf.Clamp(value, 0.0001f, 1f);
            mixer.SetFloat("MusicVolume", Mathf.Log10(clampedValue) * 20);
            volumeLevel = value;
        }
        else
        {
            Debug.LogWarning("AudioMixer is not assigned in PauseMenuHandler! Please assign it in the Inspector.");
        }
    }

    public void addScore()
    {
        currScore++;
        scoreText.text = "SCORE: " + currScore.ToString();
    }
}

