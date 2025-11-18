using UnityEngine;

public class kalenTrigger : BeatmapVisualizer
{
    [Header("Sprites")]
    public GameObject idleSprite;
    public GameObject activeSprite;

    private bool isShowingActive = false; // Track current state

    void Start()
    {
        Debug.Log("bird1 Start() called");
        
        if (gameManager == null)
        {
            Debug.LogError("bird1: gameManager is NOT assigned!");
            return;
        }


        if (npcBeatMap == null || npcBeatMap.Length == 0) 
        {
            beatmapBuilder builder = new beatmapBuilder(52);
            // builder.PlaceWholeNote(5, 1)
            //     .PlaceWholeNote(6, 1)
            //     .PlaceWholeNote(7, 0);
                builder.PlaceQuarterNote(5,1,1);
                builder.PlaceQuarterNote(5,2,0);

            
            npcBeatMap = builder.GetBeatMap();
            Debug.Log($"bird1: Beatmap created");
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