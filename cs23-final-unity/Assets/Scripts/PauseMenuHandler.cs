using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenuHandler : MonoBehaviour
{

    public static bool GameisPaused = false;
    public GameObject pauseMenuUI;
    public AudioMixer mixer;
    public static float volumeLevel = 1.0f;
    private Slider sliderVolumeCtrl;

    void Awake()
    {
        SetLevel(volumeLevel);
        GameObject sliderTemp = GameObject.FindWithTag("PauseMenuSlider");
        if (sliderTemp != null)
        {
            sliderVolumeCtrl = sliderTemp.GetComponent<Slider>();
            sliderVolumeCtrl.value = volumeLevel;
            
            // Connect the slider to the SetLevel method
            sliderVolumeCtrl.onValueChanged.AddListener(SetLevel);
        }
        else
        {
            Debug.LogWarning("Volume slider not found! Make sure your slider has the tag 'PauseMenuSlider'");
        }
    }

    void OnDestroy()
    {
        // Clean up the listener when this object is destroyed
        if (sliderVolumeCtrl != null)
        {
            sliderVolumeCtrl.onValueChanged.RemoveListener(SetLevel);
        }
    }

    void Start()
    {
        pauseMenuUI.SetActive(false);
        GameisPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameisPaused) { Resume(); }
            else { Pause(); }
        }
    }

    public void Pause()
    {
        if (!GameisPaused)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameisPaused = true;
        }
        else { Resume(); }
        //NOTE: This function is for the pause button
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameisPaused = false;
    }

    public void SetLevel(float sliderValue)
    {
        if (mixer != null)
        {
            // Clamp the value to avoid Log10(0) which is undefined
            float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
            mixer.SetFloat("MusicVolume", Mathf.Log10(clampedValue) * 20);
            volumeLevel = sliderValue;
        }
        else
        {
            Debug.LogWarning("AudioMixer is not assigned in PauseMenuHandler! Please assign it in the Inspector.");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        // Please also reset all static variables here, for new games!
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}