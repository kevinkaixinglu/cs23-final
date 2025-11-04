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

    void Start()
    {
        // Setup level buttons
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i; // Capture variable for closure
            
            if (levelButtons[i] != null)
            {
                // Add click listener
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
                
                // Check if level is unlocked
                bool isUnlocked = IsLevelUnlocked(levelIndex);
                levelButtons[i].interactable = isUnlocked;
                
                // Optional: Change appearance for locked levels
                if (!isUnlocked)
                {
                    SetButtonLocked(levelButtons[i]);
                }
            }
        }
        
        // Setup back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMainMenu);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelSceneNames.Length)
        {
            Debug.Log("Loading level: " + levelSceneNames[levelIndex]);
            SceneManager.LoadScene(levelSceneNames[levelIndex]);
        }
        else
        {
            Debug.LogError("Invalid level index: " + levelIndex);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private bool IsLevelUnlocked(int levelIndex)
    {
        // Level 0 is always unlocked
        if (levelIndex == 0)
            return true;
        
        // Check if previous level is completed
        int previousLevel = levelIndex - 1;
        return PlayerPrefs.GetInt("Level_" + previousLevel + "_Completed", 0) == 1;
    }

    private void SetButtonLocked(Button button)
    {
        // Make locked buttons look different
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        button.colors = colors;
    }

    // Call this when a level is completed
    public static void UnlockNextLevel(int currentLevelIndex)
    {
        PlayerPrefs.SetInt("Level_" + currentLevelIndex + "_Completed", 1);
        PlayerPrefs.Save();
        Debug.Log("Level " + currentLevelIndex + " completed! Next level unlocked.");
    }

    // Optional: Reset all progress
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene
    }
}