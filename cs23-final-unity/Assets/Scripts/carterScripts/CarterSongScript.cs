using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CarterSong_BeatMap : MakeBeatmap
{
    public override Measure[] SpecialBeatMap()
    {
        Debug.Log("CarterSong beatmap made");
        int num_measures = 58;

        beatmapBuilder builder = new beatmapBuilder(num_measures);

        // The below line would store "4" in the 3rd sixteenth note
        // of the second quarter note of the first measure in your
        // beatmap
        //builder.PlaceSixteenthNote(1, 2, 3, 4); 


        return builder.GetBeatMap();

    }
}
