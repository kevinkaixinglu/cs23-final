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
        builder.PlaceSixteenthNote(meas, 1, 2, -1);
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 1, 3, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas, 4, 3, 1);
        meas += 2;
        builder.PlaceSixteenthNote(meas, 1, 2, -1);
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 1, 3, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas, 4, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
        builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);

        return builder.GetBeatMap();

    }
}
