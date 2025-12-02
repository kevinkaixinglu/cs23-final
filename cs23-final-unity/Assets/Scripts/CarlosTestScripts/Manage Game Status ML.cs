using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Manage_Game_Status_ML : MonoBehaviour
{
    [Header("Puasing the Game")]
    public GameObject pauseMenuUI;
    public GameObject infoPageUI;
    public AudioMixer mixer;
    public Slider volumeSlider;
    public static float volumeLevel = 1.0f;

    private ManageGameML gameHandler;
    private bool GameisPaused = false;

    private bool InfoPage = false;

    public void Start()
    {
        gameHandler = GetComponent<ManageGameML>();

        SetVolume();
        Debug.Log("Stating Game...");
        pauseMenuUI.SetActive(false);
    
        InfoPage = true;
        GameisPaused = true;
        infoPageUI.SetActive(true);
    }

    void OnDestroy()
    {

    }

    // Update is called once per frame
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
        if (!InfoPage)
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

}
