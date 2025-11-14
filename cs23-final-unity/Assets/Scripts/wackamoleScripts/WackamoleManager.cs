using UnityEngine;
using System.Collections.Generic;

public class WackamoleManager : BeatmapVisualizerSimple
{
    [System.Serializable]
    public class HoleSprites
    {
        public string holeName;
        public GameObject idleSprite;
        public GameObject snakeSprite;
        public GameObject wormSprite;
    }

    [Header("Hole Sprites")]
    public List<HoleSprites> holeSprites = new List<HoleSprites>();

    void Start()
    {
        Debug.Log("WackamoleManager Start called");
        
        if (npcBeatMap == null || npcBeatMap.Length == 0) 
        {
            Debug.Log("Building alternating top snake / bottom worm beatmap");
            
            // Create a beatmap that alternates between:
            // - Top Snake (value 1) on odd beats
            // - Bottom Worm (value 6) on even beats
            int totalMeasures = 90; // ~3 minute song at 120 BPM
            
            beatmapBuilder builder = new beatmapBuilder(totalMeasures);
            
            bool useTopSnake = true;
            
            for (int measure = 1; measure <= totalMeasures; measure++)
            {
                for (int beat = 1; beat <= 4; beat++)
                {
                    if (useTopSnake)
                    {
                        // Top Snake (value 1)
                        builder.PlaceQuarterNote(measure, beat, 1);
                    }
                    else
                    {
                        // Bottom Worm (value 6)
                        builder.PlaceQuarterNote(measure, beat, 6);
                    }
                    
                    // Flip the flag for next beat
                    useTopSnake = !useTopSnake;
                }
            }
            
            npcBeatMap = builder.GetBeatMap();
            Debug.Log($"Beatmap created with {npcBeatMap.Length} measures");
        }
        
        // Hide all sprites initially
        HideAllSprites();
    }

    protected override void OnBeatTriggered(int noteValue)
    {
        Debug.Log($"OnBeatTriggered called with noteValue: {noteValue}");
        
        // noteValue encoding:
        // 0 = hide all
        // 1-4 = snake in holes 0-3  
        // 5-8 = worm in holes 0-3
        
        if (noteValue == 0) 
        {
            Debug.Log("Hiding all sprites");
            HideAllSprites();
            return;
        }

        // Determine which hole and type
        int holeIndex = (noteValue <= 4) ? (noteValue - 1) : (noteValue - 5);
        bool isWorm = noteValue >= 5;
        
        Debug.Log($"Processing - holeIndex: {holeIndex}, isWorm: {isWorm}");
        
        if (holeIndex >= 0 && holeIndex < holeSprites.Count)
        {
            ShowSprite(holeIndex, isWorm);
        }
        else
        {
            Debug.LogWarning($"Invalid holeIndex: {holeIndex} for noteValue: {noteValue}");
        }
    }

    private void ShowSprite(int holeIndex, bool isWorm)
    {
        Debug.Log($"Showing sprite - holeIndex: {holeIndex}, isWorm: {isWorm}");
        
        // Hide all sprites first
        HideAllSprites();
        
        var hole = holeSprites[holeIndex];
        if (isWorm)
        {
            if (hole.wormSprite != null) 
            {
                hole.wormSprite.SetActive(true);
                Debug.Log($"Activated worm sprite for {hole.holeName}");
            }
        }
        else
        {
            if (hole.snakeSprite != null) 
            {
                hole.snakeSprite.SetActive(true);
                Debug.Log($"Activated snake sprite for {hole.holeName}");
            }
        }
    }

    private void HideAllSprites()
    {
        foreach (var hole in holeSprites)
        {
            if (hole.idleSprite != null) hole.idleSprite.SetActive(true);
            if (hole.snakeSprite != null) hole.snakeSprite.SetActive(false);
            if (hole.wormSprite != null) hole.wormSprite.SetActive(false);
        }
    }
}