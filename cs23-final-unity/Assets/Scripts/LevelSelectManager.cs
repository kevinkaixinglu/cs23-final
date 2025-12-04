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
    [SerializeField] public GameObject lvl1Clear;
    // [SerializeField] public GameObject lvl2Clear;
    // [SerializeField] public GameObject lvl3Clear;
    // [SerializeField] public GameObject lvl4Clear;
    // [SerializeField] public GameObject lvl5Clear;

    void Start()
    {
        // lvl1Clear.SetActive(false);
        // lvl2Clear.SetActive(false);
        // lvl3Clear.SetActive(false);
        // lvl14lear.SetActive(false);
        // lvl5Clear.SetActive(false);


        //check if level was cleared
        if (PlayerPrefs.GetInt("Level1Passed", 0) == 1) lvl1Clear.SetActive(true);
        // if (PlayerPrefs.GetInt("Level2Passed", 0) == 1) lvl2Clear.SetActive(true);
        // if (PlayerPrefs.GetInt("Level3Passed", 0) == 1) lvl3Clear.SetActive(true);
        // if (PlayerPrefs.GetInt("Level4Passed", 0) == 1) lvl4Clear.SetActive(true);
        // if (PlayerPrefs.GetInt("Level5Passed", 0) == 1) lvl5Clear.SetActive(true);

        // if ((PlayerPrefs.GetInt("Level1Passed", 0) == 1) && (PlayerPrefs.GetInt("Level2Passed", 0) == 1)
        //     (PlayerPrefs.GetInt("Level3Passed", 0) == 1) && (PlayerPrefs.GetInt("Level4Passed", 0) == 1)
        //     (PlayerPrefs.GetInt("Level5Passed", 0) == 1)) {

        //         //code to show final ending scene
        //     }
        
        
            
        
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