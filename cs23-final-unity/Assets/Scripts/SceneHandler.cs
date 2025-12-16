using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{


    public void to_LastScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(LastSceneDefiner.lastScene);
    }

    public void to_MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void to_intro_CutScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("IntroCutScene");
    
    }

    public void to_Settings()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Settings");
    }

    public void to_Credits()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Credits");
    }

    public void to_EndLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("EndLevelScreen");
    }

    public void to_Levels()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    public void restart_level()
    {
        Time.timeScale = 1f;
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
