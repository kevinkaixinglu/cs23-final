// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.UIElements;

// public class CarterSongScript : MakeBeatmap
// {
//     public override Measure[] SpecialBeatMap()
//     {
//         Debug.Log("CarterSongScript beatmap made");
//         int num_measures = 58;

//         beatmapBuilder builder = new beatmapBuilder(num_measures);
//         int meas;

//         const private int row1N = 0;
//         const private int row2N = 1;
//         const private int row3N = 2;

//         const private int row1F = 3;
//         const private int row2F = 4;
//         const private int row3F = 5;

//         meas = 9;
//         builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas, 1, 3, 1);
//         builder.PlaceSixteenthNote(meas, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas, 4, 3, 1);
//         meas += 2;
//         builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas, 1, 3, 1);
//         builder.PlaceSixteenthNote(meas, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas, 4, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);

//         meas = 17;
//         builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas + 1, 1, 3, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
//         builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
//         meas += 2;
//         builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas + 1, 3, 3, 1);
//         builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 4, 3, 1);
//         builder.PlaceSixteenthNote(meas + 2, 1, 1, 1);

//         meas = 25;
//         builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
//         meas += 2;
//         builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);

//         meas = 33;
//         builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas, 1, 3, 1);
//         builder.PlaceSixteenthNote(meas, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 3, 1);
//         builder.PlaceSixteenthNote(meas, 4, 3, 1);
//         meas += 2;
//         builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas, 1, 3, 1);
//         builder.PlaceSixteenthNote(meas, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 3, 1);
//         builder.PlaceSixteenthNote(meas, 4, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);

//         meas = 37;
//         builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas + 1, 1, 3, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
//         builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);
//         meas += 2;
//         builder.PlaceSixteenthNote(meas + 1, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas + 1, 1, 3, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
//         builder.PlaceSixteenthNote(meas + 1, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 4, 1, 1);

//         meas = 41;
//         builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);
//         meas += 2;
//         builder.PlaceSixteenthNote(meas, 1, 2, -1); // WINE
//         builder.PlaceSixteenthNote(meas, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas, 3, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 1, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 1, 1);
//         builder.PlaceSixteenthNote(meas + 1, 2, 3, 1);


//         return builder.GetBeatMap();

//     }
// }
