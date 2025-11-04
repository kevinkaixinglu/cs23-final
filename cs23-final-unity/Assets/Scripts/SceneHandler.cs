using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void to_MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void to_Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void to_Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void to_EndLevel()
    {
        SceneManager.LoadScene("EndLevelScreen");
    }

    public void to_Levels()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void restart_level()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
