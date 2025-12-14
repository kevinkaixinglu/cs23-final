using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SmoothMoves_BeatMap : MakeBeatmap
{
    public override Measure[] SpecialBeatMap()
    {
        Debug.Log("SmoothMoves beatmap made");
        int num_measures = 58;

        beatmapBuilder builder = new beatmapBuilder(num_measures);
        int meas;

        meas = 9;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);

        meas = 17;
        builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas + 1, 3, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
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
        builder.PlaceSixteenthNote(meas, 4, 3, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 1, 3, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas, 4, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);

        meas = 41;
        builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas + 1, 1, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas + 1, 3, 2, -1); // WINE
        builder.PlaceSixteenthNote(meas + 1, 3, 3, 1);
        builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 4, 3, 1);
        builder.PlaceSixteenthNote(meas + 2, 1, 1, 1);

        meas = 49;
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
