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
        duration = 20;
        for (int i = meas; i < meas + duration; i++)
        {
            for (int j = 1; j < 5; j += 2)
            {
                builder.PlaceSixteenthNote(i, j, 1, 9);
            }
        }

        meas += 4;
        for (int i = meas; i < meas + duration; i++)
        {
            for (int j = 2; j < 5; j += 2)
            {
                builder.PlaceSixteenthNote(i, j, 1, 13);
            }
        }

        meas += 4;
        for (int i = meas; i < meas + duration; i++)
        {
            for (int j = 2; j < 5; j += 2)
            {
                builder.PlaceSixteenthNote(i, j, 3, 13);
            }
        }

        meas += 4;
        for (int i = meas; i < meas + duration; i++)
        {
            for (int j = 1; j < 5; j += 2)
            {
                builder.PlaceSixteenthNote(i, j, 3, 13);
            }
        }

        return builder.GetBeatMap();
    }
}
