using UnityEngine;

public class beatMap : MakeBeatmap
{
    public override Measure[] SpecialBeatMap()
    {
        Debug.Log("Player beatmap created");
        
        beatmapBuilder builder = new beatmapBuilder(52);
        
        // Add a quarter note on beat 4 of measure 2
        builder.PlaceQuarterNote(2, 4, 1)
               .PlaceQuarterNote(3, 1, 0);
         // Measure 2, Beat 4, noteValue 1
        
        return builder.GetBeatMap();
    }
}