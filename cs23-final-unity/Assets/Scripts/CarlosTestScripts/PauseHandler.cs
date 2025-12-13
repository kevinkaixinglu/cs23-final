using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PauseHandler : MonoBehaviour
{

    public GameObject pauseMenuUI;
    public Slider volumeSlider;
    public AudioMixer mixer;

    private bool paused;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volumeSlider.value = VolumeDefiner.vol;
        setVolume();
        Debug.Log("Stating Game...");
        pauseMenuUI.SetActive(false);
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (paused)
        {
            pauseMenuUI.SetActive(false);
            paused = false;
        }
        else
        {
            pauseMenuUI.SetActive(true);
            paused = true;
        }
    }

    public void setVolume()
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
