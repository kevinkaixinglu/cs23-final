using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MuchoLoco_BeatMap : MakeBeatmap
{
    public override Measure[] SpecialBeatMap()
    {
        Debug.Log("MuchoLoco beatmap made");
        int num_measures = 58;
        
        beatmapBuilder builder = new beatmapBuilder(num_measures);

        for (int i = 5; i < num_measures + 1; i = i + 2)
        {
            builder.PlaceSixteenthNote(i, 1, 1, 1);
        }
        for (int i = 7; i < num_measures + 1; i = i + 2)
        {
            builder.PlaceSixteenthNote(i + 1, 1, 1, 1);
        }
        for (int i = 9; i < num_measures + 1; i = i + 1)
        {
            builder.PlaceSixteenthNote(i, 2, 1, 1);
            builder.PlaceSixteenthNote(i, 3, 1, 1);
            builder.PlaceSixteenthNote(i, 4, 1, 1);
        }

        return builder.GetBeatMap();
    }
}
