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
    [SerializeField] private GameObject finalCutsceneLockOverlay; // Lock overlay for final cutscene button
    [SerializeField] private string finalCutsceneSceneName = "FinalCutscene";

    void Start()
    {
        UpdateLevelButtons();
        UpdateFinalCutsceneButton();
        
        // Setup back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMainMenu);
        }
        
        // Setup final cutscene button
        if (finalCutsceneButton != null)
        {
            finalCutsceneButton.onClick.AddListener(LoadFinalCutscene);
        }
    }

    private void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i;
            
            if (levelButtons[i] != null)
            {
                // Add click listener
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
                
                // All levels are unlocked
                levelButtons[i].interactable = true;
            }
        }
    }
    
    private void UpdateFinalCutsceneButton()
    {
        if (finalCutsceneButton != null)
        {
            // Check if level 5 (index 4) is completed
            bool level5Completed = PlayerPrefs.GetInt("Level_4_Completed", 0) == 1;
            
            // Make button interactable/non-interactable
            finalCutsceneButton.interactable = level5Completed;
            
            // Show/hide lock overlay
            if (finalCutsceneLockOverlay != null)
            {
                finalCutsceneLockOverlay.SetActive(!level5Completed);
            }
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
    
    public void LoadFinalCutscene()
    {
        Debug.Log("Loading final cutscene: " + finalCutsceneSceneName);
        SceneManager.LoadScene(finalCutsceneSceneName);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Call this when a level is completed
    public static void MarkLevelCompleted(int levelIndex)
    {
        PlayerPrefs.SetInt("Level_" + levelIndex + "_Completed", 1);
        PlayerPrefs.Save();
        Debug.Log("Level " + levelIndex + " completed!");
    }

    // Reset all progress
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}