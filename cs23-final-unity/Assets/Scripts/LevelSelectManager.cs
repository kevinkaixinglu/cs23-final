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

    [Header("Level Cleared UI (Green Check/Score)")]
    [SerializeField] private GameObject level1ClearedObj;
    [SerializeField] private GameObject level2ClearedObj;
    [SerializeField] private GameObject level3ClearedObj;
    [SerializeField] private GameObject level4ClearedObj;
    [SerializeField] private GameObject level5ClearedObj;

    // ===================== LEVEL LOCKED UI =====================
    // Logic: Level 1 is always open. 
    // Level 2 is locked until Level 1 is beaten, etc.
    
    [Header("Level Locked UI (Red Locked Text)")]
    // Note: No level1LockedObj because Level 1 is always open!
    [SerializeField] private GameObject level2LockedObj;
    [SerializeField] private GameObject level3LockedObj;
    [SerializeField] private GameObject level4LockedObj;
    [SerializeField] private GameObject level5LockedObj;

    // ===============================================================

    void Start()
    {
        // 1. First, figure out what levels we have beaten
        CheckProgress(); 
        
        // 2. Set up the button clicks
        SetupButtonListeners(); 
        
        // 3. Update the visuals (Cleared checks and Locked text)
        UpdateLevelClearedUI();
        UpdateLockedState(); // <--- NEW FUNCTION
        UpdateFinalCutsceneUI();

        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);

        if (finalCutsceneButton != null)
            finalCutsceneButton.onClick.AddListener(LoadFinalCutscene);
    }

    private void CheckProgress()
    {
        // I moved the PlayerPrefs reading here so we can call it cleanly
        level1Cleared = PlayerPrefs.GetInt("Level1Passed", 0) == 1;
        level2Cleared = PlayerPrefs.GetInt("Level2Passed", 0) == 1;
        level3Cleared = PlayerPrefs.GetInt("Level3Passed", 0) == 1;
        level4Cleared = PlayerPrefs.GetInt("Level4Passed", 0) == 1;
        level5Cleared = PlayerPrefs.GetInt("Level5Passed", 0) == 1;
    }

    private void UpdateLevelClearedUI()
    {
        if (level1ClearedObj != null) level1ClearedObj.SetActive(level1Cleared);
        if (level2ClearedObj != null) level2ClearedObj.SetActive(level2Cleared);
        if (level3ClearedObj != null) level3ClearedObj.SetActive(level3Cleared);
        if (level4ClearedObj != null) level4ClearedObj.SetActive(level4Cleared);
        if (level5ClearedObj != null) level5ClearedObj.SetActive(level5Cleared);
    }

    // THIS IS THE NEW LOGIC FOR LOCKING LEVELS
    private void UpdateLockedState()
    {
        // LEVEL 1: Always unlocked
        UnlockLevel(0, null); 

        // LEVEL 2: Unlocked only if Level 1 is cleared
        if (level1Cleared) UnlockLevel(1, level2LockedObj);
        else LockLevel(1, level2LockedObj);

        // LEVEL 3: Unlocked only if Level 2 is cleared
        if (level2Cleared) UnlockLevel(2, level3LockedObj);
        else LockLevel(2, level3LockedObj);

        // LEVEL 4: Unlocked only if Level 3 is cleared
        if (level3Cleared) UnlockLevel(3, level4LockedObj);
        else LockLevel(3, level4LockedObj);

        // LEVEL 5: Unlocked only if Level 4 is cleared
        if (level4Cleared) UnlockLevel(4, level5LockedObj);
        else LockLevel(4, level5LockedObj);
    }

    // Helper function to make the button clickable and hide the "Locked" text
    private void UnlockLevel(int buttonIndex, GameObject lockedUI)
    {
        if (buttonIndex < levelButtons.Length && levelButtons[buttonIndex] != null)
        {
            levelButtons[buttonIndex].interactable = true;
        }
        if (lockedUI != null)
        {
            lockedUI.SetActive(false);
        }
    }

    // Helper function to make button unclickable and show the "Locked" text
    private void LockLevel(int buttonIndex, GameObject lockedUI)
    {
        if (buttonIndex < levelButtons.Length && levelButtons[buttonIndex] != null)
        {
            levelButtons[buttonIndex].interactable = false;
        }
        if (lockedUI != null)
        {
            lockedUI.SetActive(true);
        }
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

    private void SetupButtonListeners()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i;
            if (levelButtons[i] != null)
            {
                // Remove existing listeners first to prevent duplicates if Start runs twice (rare but safe)
                levelButtons[i].onClick.RemoveAllListeners(); 
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
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