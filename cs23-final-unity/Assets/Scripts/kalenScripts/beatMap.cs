using UnityEngine;

public class beatMap : MakeBeatmap
{
    public override Measure[] SpecialBeatMap()
    {
        Debug.Log("Empty beatmap created - no player input required");
        
        // Create an empty beatmap with 1 measure (or however many you need)
        // All notes will be 0 by default, so no input windows will open
        beatmapBuilder builder = new beatmapBuilder(52);
        
        return builder.GetBeatMap();
    }
}