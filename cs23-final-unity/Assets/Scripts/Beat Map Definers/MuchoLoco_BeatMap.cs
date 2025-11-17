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

        int duration;
        int meas = 1;

        meas += 4;
        duration = 4;
        for (int i = meas; i < meas + duration; i++)
        {
            builder.PlaceSixteenthNote(i, 1, 1, 1 + 4 * ((i + 1) % 4));
        }

        return builder.GetBeatMap();
    }
}
