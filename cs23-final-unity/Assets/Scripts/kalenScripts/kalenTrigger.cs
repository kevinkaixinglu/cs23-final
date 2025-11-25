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



            builder
                   .PlaceWholeNote(2, 1)
                   .PlaceWholeNote(3, 0)

                   .PlaceWholeNote(5, 1)
                   .PlaceWholeNote(6, 1)
            
                   .PlaceWholeNote(9, 1)
                   .PlaceWholeNote(10, 1)

                   .PlaceHalfNote(12, 2, 1)
                   .PlaceQuarterNote(12, 4, 1)
                   .PlaceQuarterNote(13, 1, 1)

                   .PlaceHalfNote(14, 2, 1)
                   .PlaceQuarterNote(14, 4, 1)
                   .PlaceQuarterNote(15, 1, 1)

                   .PlaceSixteenthNote(15, 3, 2, 1)
                   .PlaceEighthNote(15, 3, 2, 1)
                   .PlaceQuarterNote(15, 4, 1)
                   .PlaceEighthNote(16, 1, 1, 1)

                   .PlaceSixteenthNote(16, 1, 4, 1)
                   .PlaceQuarterNote(16, 2, 1)
                   .PlaceHalfNote(16, 3, 1)
                   .PlaceSixteenthNote(17, 1, 1, 1)

                   .PlaceHalfNote(17, 3, 1)
                   .PlaceWholeNote(18, 1)
                   .PlaceEighthNote(19, 1, 1, 1)

                   .PlaceEighthNote(20, 1, 3, 1)
                   .PlaceQuarterNote(20, 2, 1)
                   .PlaceHalfNote(20, 3, 1)

                   .PlaceEighthNote(21, 1, 3, 1)
                   .PlaceQuarterNote(21, 2, 1)
                   .PlaceHalfNote(21, 3, 1)

                   .PlaceHalfNote(22, 3, 1)
                   .PlaceWholeNote(23, 1)
                   .PlaceHalfNote(24, 1, 1)

                   .PlaceWholeNote(25, 1)
                   .PlaceWholeNote(26, 1)

                   .PlaceWholeNote(31, 1)
                   .PlaceHalfNote(32, 1, 1)
                   .PlaceQuarterNote(32, 3, 1)

                   .PlaceHalfNote(33, 1, 1)
                   .PlaceQuarterNote(33, 3, 1)
                   .PlaceEighthNote(33, 4, 1, 1)

                   .PlaceHalfNote(34, 1, 1)
                   .PlaceQuarterNote(34, 3, 1)
                   .PlaceEighthNote(34, 4, 1, 1)

                   .PlaceWholeNote(35, 1)
                   .PlaceHalfNote(36, 1, 1)
                   .PlaceQuarterNote(36, 3, 1)




                   

                //    .PlaceEighthNote(21, 1, 3, 1)
                //    .PlaceQuarterNote(21, 2, 1)
                //    .PlaceHalfNote(21, 3, 1)

                //    .PlaceHalfNote(22, 3, 1)



                   

            //end marker
                   .PlaceWholeNote(50, 0);






            
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