using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class ManageGameCopy : MonoBehaviour
{
    [Header("Puasing the Game")]
    public GameObject pauseMenuUI;
    public AudioMixer mixer;
    public Slider volumeSlider;

    private GameHandlerCopy gameHandler;
    private static bool GameisPaused = false;

    [Header("Info Page")]
    public GameObject infoPageUI;

    private bool InfoPage = false;

    public void Start()
    {
        gameHandler = GetComponent<GameHandlerCopy>();

        volumeSlider.value = VolumeDefiner.vol;
        SetVolume();
        Debug.Log("Stating Game...");
        InfoPage = true;
        GameisPaused = false;
        infoPageUI.SetActive(true);
        pauseMenuUI.SetActive(false);
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
                pauseMenuUI.SetActive(false);

                gameHandler.StartGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
            Debug.Log("P1...");
        }
    }

    public void Pause()
    {
        Debug.Log("P2...");
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


