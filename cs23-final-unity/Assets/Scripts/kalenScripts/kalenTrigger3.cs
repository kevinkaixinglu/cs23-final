using UnityEngine;

public class kalenTrigger3 : BeatmapVisualizer
{
    [Header("Sprites")]
    public GameObject idleSprite;
    public GameObject activeSprite;
    public GameObject judgySprite;

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
            int totalMeasures = 57;
            beatmapBuilder builder = new beatmapBuilder(totalMeasures);
            
            
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

                    //speed up
                //   .PlaceHalfNote(33, 3, 1)
                //   .PlaceHalfNote(34, 3, 1)

                   .PlaceQuarterNote(33, 3, 1)
                   .PlaceEighthNote(33, 4, 1, 1)

                   .PlaceQuarterNote(34, 3, 1)
                   .PlaceEighthNote(34, 4, 1, 1)

                   .PlaceHalfNote(36, 1, 1)
                   .PlaceQuarterNote(36, 3, 1)

                   .PlaceEighthNote(37, 4, 3, 1)
                   .PlaceQuarterNote(38, 1, 1)


                   .PlaceEighthNote(39, 2, 3, 1)
                   .PlaceQuarterNote(39, 3, 1)

                   .PlaceEighthNote(40, 2, 3, 1)
                   .PlaceQuarterNote(40, 3, 1)

                   .PlaceHalfNote(42, 1, 1)
                   .PlaceQuarterNote(42, 3, 1)

                   .PlaceHalfNote(44, 1, 1)
                   .PlaceQuarterNote(44, 3, 1)

                   .PlaceEighthNote(45, 2, 3, 1)
                   .PlaceQuarterNote(45, 3, 1)

                   .PlaceEighthNote(46, 2, 3, 1)
                   .PlaceQuarterNote(46, 3, 1)

                   
                   .PlaceEighthNote(49, 2, 1, 1)
                   .PlaceQuarterNote(49, 2, 1)

                   .PlaceEighthNote(51, 2, 1, 1)
                   .PlaceQuarterNote(51, 2, 1)

                   .PlaceHalfNote(54, 1, 1)
                   .PlaceQuarterNote(54, 3, 1)

                   .PlaceHalfNote(56, 1, 1)
                   .PlaceQuarterNote(56, 3, 1)


                   


                   .PlaceHalfNote(57, 1, 0);

                   

                   


            npcBeatMap = builder.GetBeatMap();
            Debug.Log($"kalenTrigger3: Beatmap created with pattern on every other measure");
        }

        // Initialize to idle
        ShowIdle();
    }

    protected override void OnBeatTriggered(int noteValue)
    {
        // FIRST: Check if judgy birds is active
        if (gameManager != null && gameManager.judgyBirds)
        {
            ShowJudgy();
            return;
        }

        // SECOND: Normal idle/active logic when judgy birds is OFF
        if (noteValue == 0)
        {
            ShowIdle();
        }
        else // noteValue != 0
        {
            ShowActive();
        }
        
        // Update tracking
        isShowingActive = (noteValue != 0);
    }

    private void ShowIdle()
    {
        if (idleSprite != null) idleSprite.SetActive(true);
        if (activeSprite != null) activeSprite.SetActive(false);
        if (judgySprite != null) judgySprite.SetActive(false);
        isShowingActive = false;
    }

    private void ShowActive()
    {
        if (idleSprite != null) idleSprite.SetActive(false);
        if (activeSprite != null) activeSprite.SetActive(true);
        if (judgySprite != null) judgySprite.SetActive(false);
        isShowingActive = true;
    }

    private void ShowJudgy()
    {
        if (idleSprite != null) idleSprite.SetActive(false);
        if (activeSprite != null) activeSprite.SetActive(false);
        if (judgySprite != null) judgySprite.SetActive(true);
        isShowingActive = false;
    }
}