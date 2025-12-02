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
                
                // All levels are now unlocked
                levelButtons[i].interactable = true;
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

    // Optional: Call this to track level completion (for stats, stars, etc.)
    public static void MarkLevelCompleted(int levelIndex)
    {
        PlayerPrefs.SetInt("Level_" + levelIndex + "_Completed", 1);
        PlayerPrefs.Save();
        Debug.Log("Level " + levelIndex + " completed!");
    }

    // Optional: Reset completion tracking
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene
    }
}