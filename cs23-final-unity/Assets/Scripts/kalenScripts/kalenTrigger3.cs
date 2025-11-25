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
            // for (int measure = 2; measure <= totalMeasures; measure += 2)
            // {
            //     builder.PlaceQuarterNote(measure, 1, 1); // Beat 1: active
                // Beat 2 is automatically 0 (idle)
            // }
            
            builder.PlaceQuarterNote(2, 3, 1)
                   .PlaceQuarterNote(2, 4, 1)
                   .PlaceQuarterNote(3, 1, 0)

                   .PlaceHalfNote(6, 1, 1)
                   .PlaceHalfNote(6, 3, 1)

                   .PlaceWholeNote(10, 1)

                   .PlaceQuarterNote(12, 4, 1)
                   .PlaceQuarterNote(13, 1, 1)

                   .PlaceQuarterNote(14, 4, 1)
                   .PlaceQuarterNote(15, 1, 1)

                   .PlaceSixteenthNote(15, 4, 2, 1)
                   .PlaceEighthNote(15, 4, 2, 1)
                   .PlaceEighthNote(16, 1, 1, 1)
                   
                   .PlaceSixteenthNote(16, 3, 4, 1)
                   .PlaceQuarterNote(16, 4, 1)
                   .PlaceSixteenthNote(17, 1, 1, 1)

                   .PlaceHalfNote(18, 3, 1)
                   .PlaceEighthNote(19, 1, 1, 1)

                   .PlaceEighthNote(20, 3, 3, 1)
                   .PlaceQuarterNote(20, 4, 1)

                   .PlaceEighthNote(21, 3, 3, 1)
                   .PlaceQuarterNote(21, 4, 1)

                   .PlaceHalfNote(23, 3, 1)
                   .PlaceHalfNote(24, 1, 1)

                   .PlaceWholeNote(26, 1)

                   .PlaceHalfNote(32, 1, 1)
                   .PlaceQuarterNote(32, 3, 1)

                   .PlaceQuarterNote(33, 3, 1)
                   .PlaceEighthNote(33, 4, 1, 1)

                   .PlaceQuarterNote(34, 3, 1)
                   .PlaceEighthNote(34, 4, 1, 1)

                   .PlaceHalfNote(36, 1, 1)
                   .PlaceQuarterNote(36, 3, 1)

                   


                   .PlaceHalfNote(50, 1, 0);

                   

                   


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