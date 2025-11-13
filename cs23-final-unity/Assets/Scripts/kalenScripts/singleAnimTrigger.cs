using UnityEngine;

public class singleAnimTrigger : BeatmapVisualizer
{
    [Header("Sprites")]
    public GameObject idleSprite;
    public GameObject activeSprite;

    void Start()
    {
        if (npcBeatMap == null || npcBeatMap.Length == 0) {
            //building the beatmap here
            beatmapBuilder builder = new beatmapBuilder(4);
            builder.PlaceQuarterNote(1, 1, 0)
                .PlaceQuarterNote(1, 2, 1)
                .PlaceQuarterNote(1, 3, 0)
                .PlaceQuarterNote(1, 4, 1);
            
            npcBeatMap = builder.GetBeatMap();
        }
        
    }

    protected override void OnBeatTriggered(int noteValue)
    {
        // Hide both first
        if (idleSprite != null) idleSprite.SetActive(false);
        if (activeSprite != null) activeSprite.SetActive(false);

        // If noteValue is 0 idle, otherwise show active sprite
        if (noteValue == 0)
        {
            if (idleSprite != null) idleSprite.SetActive(true);
        }
        else
        {
            if (activeSprite != null) activeSprite.SetActive(true);
        }
    }
}
