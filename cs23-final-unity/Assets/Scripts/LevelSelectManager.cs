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
    
    [Header("Lock Overlays")]
    [SerializeField] private GameObject[] lockOverlays; // Assign the dark overlay for each level button
    
    [Header("Final Cutscene")]
    [SerializeField] private Button finalCutsceneButton;
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
                levelButtons[i].onClick.RemoveAllListeners();
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
                
                // Check if level is unlocked
                bool isUnlocked = IsLevelUnlocked(levelIndex);
                levelButtons[i].interactable = isUnlocked;
                
                // Show/hide lock overlay
                if (lockOverlays != null && levelIndex < lockOverlays.Length && lockOverlays[levelIndex] != null)
                {
                    lockOverlays[levelIndex].SetActive(!isUnlocked); // Hide if unlocked, show if locked
                }
            }
        }
    }
    
    private void UpdateFinalCutsceneButton()
    {
        if (finalCutsceneButton != null)
        {
            // Check if level 5 (index 4) is completed
            bool level5Completed = PlayerPrefs.GetInt("Level_4_Completed", 0) == 1;
            
            // Show button only if level 5 is completed
            finalCutsceneButton.gameObject.SetActive(level5Completed);
        }
    }

    private bool IsLevelUnlocked(int levelIndex)
    {
        // First level is always unlocked
        if (levelIndex == 0)
        {
            return true;
        }
        
        // Check if previous level is completed
        int previousLevelCompleted = PlayerPrefs.GetInt("Level_" + (levelIndex - 1) + "_Completed", 0);
        return previousLevelCompleted == 1;
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelSceneNames.Length)
        {
            if (IsLevelUnlocked(levelIndex))
            {
                Debug.Log("Loading level: " + levelSceneNames[levelIndex]);
                SceneManager.LoadScene(levelSceneNames[levelIndex]);
            }
            else
            {
                Debug.Log("Level " + levelIndex + " is locked!");
            }
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