using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndHandler : MonoBehaviour
{
    public void LoadWinScene()
    {
        SceneManager.LoadScene("WinCutscene");
    }

    public void LoadLoseScene()
    {
        SceneManager.LoadScene("LoseCutscene");
    }
}