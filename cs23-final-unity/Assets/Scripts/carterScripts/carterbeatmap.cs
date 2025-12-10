using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class carterbeatmap : MakeBeatmap
{
    public override Measure[] SpecialBeatMap()
    {
        Debug.Log("CarterLevel beatmap made");
        int num_measures = 58;

        beatmapBuilder builder = new beatmapBuilder(num_measures);
        int meas;

        meas = 3;
        builder.PlaceSixteenthNote(meas, 1, 2, 1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 1, 3, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 2);
        builder.PlaceSixteenthNote(meas, 3, 1, 2);
        builder.PlaceSixteenthNote(meas, 4, 3, 3);
        meas += 2;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 1, 3, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas, 4, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);

        meas = 17;
        builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas + 1, 1, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas + 1, 3, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 4, 3, 1);
        builder.PlaceSixteenthNote(meas + 2, 1, 1, 1);

        meas = 25;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);

        meas = 33;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 1, 3, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 3, 1);
        builder.PlaceSixteenthNote(meas, 4, 3, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 1, 3, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 3, 1);
        builder.PlaceSixteenthNote(meas, 4, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);

        meas = 37;
        builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas + 1, 1, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas + 1, 1, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);

        meas = 41;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);


        return builder.GetBeatMap();

    }
}
