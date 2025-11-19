using UnityEngine;

public class kalenTrigger2 : BeatmapVisualizer
{
    [Header("Sprites")]
    public GameObject idleSprite;
    public GameObject activeSprite;

    private bool isShowingActive = false; // Track current state

    void Start()
    {
        Debug.Log("kalenTrigger2: Start() called");
        
        if (gameManager == null)
        {
            Debug.LogError("kalenTrigger2: gameManager is NOT assigned!");
            return;
        }

        if (npcBeatMap == null || npcBeatMap.Length == 0) 
        {
            int totalMeasures = 52;
            beatmapBuilder builder = new beatmapBuilder(totalMeasures);
            
            // // Place note on beat 3 of every OTHER measure (1, 3, 5, 7, etc.)
            // for (int measure = 1; measure <= totalMeasures; measure += 2)
            // {
            //     builder.PlaceQuarterNote(measure, 3, 1); // Beat 3: active
            //     // Beat 4 is automatically 0 (idle)
            // }

            builder.PlaceQuarterNote(2, 2, 1)
                   .PlaceQuarterNote(2, 3, 1)
                   .PlaceQuarterNote(2, 4, 1)
                   .PlaceQuarterNote(3, 1, 0);
            
            npcBeatMap = builder.GetBeatMap();
            Debug.Log($"kalenTrigger2: Beatmap created with pattern on every other measure");
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