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
            int totalMeasures = 57;
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

                   .PlaceHalfNote(5, 3, 1)
                   .PlaceHalfNote(6, 1, 1)
                   .PlaceHalfNote(6, 3, 1)

                   .PlaceHalfNote(9, 3, 1)
                   .PlaceWholeNote(10, 1)

                   .PlaceHalfNote(12, 3, 1)
                   .PlaceQuarterNote(13, 1, 1)

                   .PlaceHalfNote(14, 3, 1)
                   .PlaceQuarterNote(15, 1, 1)

                   .PlaceEighthNote(15, 3, 4, 1)
                   .PlaceQuarterNote(15, 4, 1)
                   .PlaceEighthNote(16, 1, 1, 1)

                   .PlaceSixteenthNote(16, 2, 4, 1)
                   .PlaceHalfNote(16, 3, 1)
                   .PlaceSixteenthNote(17, 1, 1, 1)
                   
                   .PlaceWholeNote(18, 1)
                   .PlaceEighthNote(19, 1, 1, 1)

                   .PlaceEighthNote(20, 2, 3, 1)
                   .PlaceHalfNote(20, 3, 1)

                   .PlaceEighthNote(21, 2, 3, 1)
                   .PlaceHalfNote(21, 3, 1)

                   .PlaceWholeNote(23, 1)
                   .PlaceHalfNote(24, 1, 1)

                   .PlaceHalfNote(25, 3, 1)
                   .PlaceWholeNote(26, 1)

                   .PlaceHalfNote(31, 3, 1)
                   .PlaceWholeNote(32, 1)

                   .PlaceHalfNote(31, 3, 1)
                   .PlaceQuarterNote(32, 3, 1)
                   .PlaceQuarterNote(32, 4, 0)

                       //speed up
                   .PlaceQuarterNote(33, 2, 1)
                   .PlaceQuarterNote(33, 3, 1)
                   .PlaceEighthNote(33, 4, 1, 1)

                   .PlaceQuarterNote(34, 2, 1)
                   .PlaceQuarterNote(34, 3, 1)
                   .PlaceEighthNote(34, 4, 1, 1)

                   .PlaceHalfNote(35, 3, 1)
                   .PlaceHalfNote(36, 1, 1)
                   .PlaceQuarterNote(36, 3, 1)

                   .PlaceEighthNote(37, 3, 3, 1)
                   .PlaceQuarterNote(37, 4, 1)
                   .PlaceQuarterNote(38, 1, 1)

                   .PlaceEighthNote(39, 1, 3, 1)
                   .PlaceQuarterNote(39, 2, 1)
                   .PlaceQuarterNote(39, 3, 1)

                   .PlaceEighthNote(40, 1, 3, 1)
                   .PlaceQuarterNote(40, 2, 1)
                   .PlaceQuarterNote(40, 3, 1)

                   .PlaceEighthNote(41, 3, 3, 1)
                   .PlaceQuarterNote(41, 4, 1)
                   .PlaceHalfNote(42, 1, 1)
                   .PlaceQuarterNote(42, 3, 1)

                   .PlaceEighthNote(43, 3, 3, 1)
                   .PlaceQuarterNote(43, 4, 1)
                   .PlaceHalfNote(44, 1, 1)
                   .PlaceQuarterNote(44, 3, 1)

                   .PlaceEighthNote(45, 1, 3, 1)
                   .PlaceQuarterNote(45, 2, 1)
                   .PlaceQuarterNote(45, 3, 1)

                   .PlaceEighthNote(46, 1, 3, 1)
                   .PlaceQuarterNote(46, 2, 1)
                   .PlaceQuarterNote(46, 3, 1)

                   //HERE
                   .PlaceSixteenthNote(49, 1, 2, 1)
                   .PlaceSixteenthNote(49, 1, 3, 1)
                   .PlaceSixteenthNote(49, 1, 4, 1)
                   .PlaceEighthNote(49, 1, 3, 1)
                   .PlaceQuarterNote(49, 2, 1)
                   
                   //HERE
                   .PlaceSixteenthNote(51, 1, 2, 1)
                   .PlaceSixteenthNote(51, 1, 3, 1)
                   .PlaceSixteenthNote(51, 1, 4, 1)
                   .PlaceEighthNote(51, 1, 3, 1)
                   .PlaceQuarterNote(51, 2, 1)
                   

                   .PlaceEighthNote(53, 3, 3, 1)
                   .PlaceQuarterNote(53, 4, 1)
                   .PlaceHalfNote(54, 1, 1)
                   .PlaceQuarterNote(54, 3, 1)

                   .PlaceEighthNote(55, 3, 3, 1)
                   .PlaceQuarterNote(55, 4, 1)
                   .PlaceHalfNote(56, 1, 1)
                   .PlaceQuarterNote(56, 3, 1)

          





                   //end marker
                   .PlaceHalfNote(57, 1, 0);
            
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