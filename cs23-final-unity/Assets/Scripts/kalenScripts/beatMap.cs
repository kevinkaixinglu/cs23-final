using UnityEngine;

public class beatMap : MakeBeatmap
{
    public override Measure[] SpecialBeatMap()
    {
        Debug.Log("Player beatmap created");
        
        beatmapBuilder builder = new beatmapBuilder(52);
        
        // Add a quarter note on beat 4 of measure 2
        builder

               .PlaceQuarterNote(2, 4, 1)

               .PlaceWholeNote(6, 1)

               .PlaceHalfNote(10, 3, 1)

               .PlaceQuarterNote(13, 1, 1)

               .PlaceQuarterNote(15, 1, 1)

               .PlaceEighthNote(16, 1, 1, 1)

               .PlaceEighthNote(17, 1, 1, 1)

               .PlaceEighthNote(19, 1, 1, 1)

               .PlaceEighthNote(20, 4, 3, 1)

               .PlaceEighthNote(21, 4, 3, 1)

               .PlaceEighthNote(22, 4, 3, 1)



               .PlaceQuarterNote(30, 1, 0);
         // Measure 2, Beat 4, noteValue 1
        
        return builder.GetBeatMap();
    }
}