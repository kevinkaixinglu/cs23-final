using UnityEngine;

public class kalenTrigger3 : BeatmapVisualizer
{
    [Header("Sprites")]
    public GameObject idleSprite;
    public GameObject activeSprite;

    private bool isShowingActive = false; // Track current state

    void Start()
    {
        Debug.Log("kalenTrigger3: Start() called");
        
        if (gameManager == null)
        {
            Debug.LogError("kalenTrigger3: gameManager is NOT assigned!");
            return;
        }

        if (npcBeatMap == null || npcBeatMap.Length == 0) 
        {
            int totalMeasures = 52;
            beatmapBuilder builder = new beatmapBuilder(totalMeasures);
            
            // Place note on beat 1 of every OTHER measure starting at measure 2 (2, 4, 6, 8, etc.)
            for (int measure = 2; measure <= totalMeasures; measure += 2)
            {
                builder.PlaceQuarterNote(measure, 1, 1); // Beat 1: active
                // Beat 2 is automatically 0 (idle)
            }
            
            npcBeatMap = builder.GetBeatMap();
            Debug.Log($"kalenTrigger3: Beatmap created with pattern on every other measure");
        }

        // Initialize to idle
        ShowIdle();
    }

    protected override void OnBeatTriggered(int noteValue)
    {
        // Only change state if needed
        if (noteValue == 0 && isShowingActive)
        {
            ShowIdle();
        }
        else if (noteValue != 0 && !isShowingActive)
        {
            ShowActive();
        }
    }

    private void ShowIdle()
    {
        if (idleSprite != null) idleSprite.SetActive(true);
        if (activeSprite != null) activeSprite.SetActive(false);
        isShowingActive = false;
    }

    private void ShowActive()
    {
        if (idleSprite != null) idleSprite.SetActive(false);
        if (activeSprite != null) activeSprite.SetActive(true);
        isShowingActive = true;
    }
}