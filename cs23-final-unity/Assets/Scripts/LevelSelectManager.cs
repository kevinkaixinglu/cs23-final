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
    [SerializeField] private string finalCutsceneSceneName = "FinalCutscene";

    // ===================== LEVEL CLEARED FLAGS =====================

    [Header("Level Cleared Flags")]
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
        UpdateFinalCutsceneButton();
        UpdateLevelClearedUI();
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMainMenu);
        }
        
        if (finalCutsceneButton != null)
        {
            finalCutsceneButton.onClick.AddListener(LoadFinalCutscene);
        }
    }

    private void UpdateLevelClearedUI()
    {
        level1Cleared = PlayerPrefs.GetInt("Level_0_Completed", 0) == 1;
        level2Cleared = PlayerPrefs.GetInt("Level_1_Completed", 0) == 1;
        level3Cleared = PlayerPrefs.GetInt("Level_2_Completed", 0) == 1;
        level4Cleared = PlayerPrefs.GetInt("Level_3_Completed", 0) == 1;
        level5Cleared = PlayerPrefs.GetInt("Level_4_Completed", 0) == 1;

        if (level1ClearedObj != null) level1ClearedObj.SetActive(level1Cleared);
        if (level2ClearedObj != null) level2ClearedObj.SetActive(level2Cleared);
        if (level3ClearedObj != null) level3ClearedObj.SetActive(level3Cleared);
        if (level4ClearedObj != null) level4ClearedObj.SetActive(level4Cleared);
        if (level5ClearedObj != null) level5ClearedObj.SetActive(level5Cleared);
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
    
    private void UpdateFinalCutsceneButton()
    {
        if (finalCutsceneButton != null)
        {
            bool level5Completed = PlayerPrefs.GetInt("Level_4_Completed", 0) == 1;
            finalCutsceneButton.interactable = level5Completed;
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelSceneNames.Length)
        {
            SceneManager.LoadScene(levelSceneNames[levelIndex]);
        }
    }
    
    public void LoadFinalCutscene()
    {
        SceneManager.LoadScene(finalCutsceneSceneName);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public static void MarkLevelCompleted(int levelIndex)
    {
        PlayerPrefs.SetInt("Level_" + levelIndex + "_Completed", 1);
        PlayerPrefs.Save();
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
