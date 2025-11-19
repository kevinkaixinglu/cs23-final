using UnityEngine;

public class kalenTrigger : BeatmapVisualizer
{
    [Header("Sprites")]
    public GameObject idleSprite;
    public GameObject activeSprite;

    private bool isShowingActive = false; // Track current state

    void Start()
    {
        Debug.Log("kalenTrigger: Start() called");
        
        if (gameManager == null)
        {
            Debug.LogError("kalenTrigger: gameManager is NOT assigned!");
            return;
        }

        if (npcBeatMap == null || npcBeatMap.Length == 0) 
        {
            int totalMeasures = 52;
            beatmapBuilder builder = new beatmapBuilder(totalMeasures);

            // // Place note on beat 1 of every OTHER measure (1, 3, 5, 7, etc.)
            // for (int measure = 1; measure <= totalMeasures; measure += 2)
            // {
            //     builder.PlaceQuarterNote(measure, 1, 1); // Beat 1: active
            //     // Beats 2, 3, 4 are automatically 0 (idle)
            // }

            builder.PlaceWholeNote(2, 1);
            builder.PlaceWholeNote(3, 0);

            
            npcBeatMap = builder.GetBeatMap();
            Debug.Log($"kalenTrigger: Beatmap created with pattern on every other measure");
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