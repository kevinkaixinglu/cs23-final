using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField] private Button[] levelButtons;

    [Header("Level Names")]
    [SerializeField] private string[] levelSceneNames;

    [Header("UI Elements")]
    [SerializeField] private Button backButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Final Cutscene")]
    [SerializeField] private Button finalCutsceneButton;
    [SerializeField] private GameObject finalCutsceneUI;
    [SerializeField] private string finalScene = "finalScene";

    // ===================== LEVEL CLEARED FLAGS =====================

    private bool level1Cleared;
    private bool level2Cleared;
    private bool level3Cleared;
    private bool level4Cleared;
    private bool level5Cleared;

    [Header("Level Cleared UI")]
    [SerializeField] private GameObject level1ClearedObj;
    [SerializeField] private GameObject level2ClearedObj;
    [SerializeField] private GameObject level3ClearedObj;
    [SerializeField] private GameObject level4ClearedObj;
    [SerializeField] private GameObject level5ClearedObj;

    // ===============================================================

    void Start()
    {
        UpdateLevelButtons();
        UpdateLevelClearedUI();
        UpdateFinalCutsceneUI();

        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);

        if (finalCutsceneButton != null)
            finalCutsceneButton.onClick.AddListener(LoadFinalCutscene);
    }

    private void UpdateLevelClearedUI()
    {
        level1Cleared = PlayerPrefs.GetInt("Level1Passed", 0) == 1;
        level2Cleared = PlayerPrefs.GetInt("Level2Passed", 0) == 1;
        level3Cleared = PlayerPrefs.GetInt("Level3Passed", 0) == 1;
        level4Cleared = PlayerPrefs.GetInt("Level4Passed", 0) == 1;
        level5Cleared = PlayerPrefs.GetInt("Level5Passed", 0) == 1;

        if (level1ClearedObj != null) level1ClearedObj.SetActive(level1Cleared);
        if (level2ClearedObj != null) level2ClearedObj.SetActive(level2Cleared);
        if (level3ClearedObj != null) level3ClearedObj.SetActive(level3Cleared);
        if (level4ClearedObj != null) level4ClearedObj.SetActive(level4Cleared);
        if (level5ClearedObj != null) level5ClearedObj.SetActive(level5Cleared);
    }

    private void UpdateFinalCutsceneUI()
    {
        bool allLevelsPassed =
            level1Cleared &&
            level2Cleared &&
            level3Cleared &&
            level4Cleared &&
            level5Cleared;

        if (finalCutsceneUI != null)
            finalCutsceneUI.SetActive(allLevelsPassed);

        if (finalCutsceneButton != null)
            finalCutsceneButton.interactable = allLevelsPassed;
    }

    private void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i;
            if (levelButtons[i] != null)
            {
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
                levelButtons[i].interactable = true;
            }
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelSceneNames.Length)
        {
            LastSceneDefiner.lastScene = levelSceneNames[levelIndex];
            SceneManager.LoadScene(levelSceneNames[levelIndex]);
        }
    }

    public void LoadFinalCutscene()
    {
        SceneManager.LoadScene(finalScene);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
