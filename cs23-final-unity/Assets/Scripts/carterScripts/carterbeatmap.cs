using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class carterbeatmap : MakeBeatmap
{
    //1 top, 2 mid, 3 bot
    public override Measure[] SpecialBeatMap()
    {
        Debug.Log("CarterLevel beatmap made");
        int num_measures = 34;

        beatmapBuilder builder = new beatmapBuilder(num_measures);
        int meas;

        meas = 1;
        builder.PlaceSixteenthNote(meas, 2, 2, 2);
        builder.PlaceSixteenthNote(meas, 3, 2, 2);
        builder.PlaceSixteenthNote(meas, 4, 2, 2);
        meas += 1;

        builder.PlaceSixteenthNote(meas, 4, 2, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 2, 1);
        builder.PlaceSixteenthNote(meas, 2, 2, 2);
        builder.PlaceSixteenthNote(meas, 3, 2, 3);

        builder.PlaceSixteenthNote(meas, 4, 3, 2);
        builder.PlaceSixteenthNote(meas + 1, 1, 1, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 4, 2);
        builder.PlaceSixteenthNote(meas, 2, 4, 1);
        builder.PlaceSixteenthNote(meas, 3, 4, 1);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 2);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas, 4, 1, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 1, 2);
        builder.PlaceSixteenthNote(meas, 2, 1, 2);

        builder.PlaceSixteenthNote(meas, 4, 2, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 2, 2);
        builder.PlaceSixteenthNote(meas, 2, 2, 2);
        builder.PlaceSixteenthNote(meas, 3, 2, 2);
        builder.PlaceSixteenthNote(meas, 4, 2, 2);

        builder.PlaceSixteenthNote(meas, 4, 4, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 2, 1, 1);
        builder.PlaceSixteenthNote(meas, 2, 4, 1);
        builder.PlaceSixteenthNote(meas, 3, 2, 1);
        builder.PlaceSixteenthNote(meas, 3, 4, 1);
        builder.PlaceSixteenthNote(meas, 4, 2, 1);

        meas += 1;
        builder.PlaceSixteenthNote(meas, 4, 1, 2);
        builder.PlaceSixteenthNote(meas, 4, 3, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 1, 2);
        builder.PlaceSixteenthNote(meas, 1, 4, 2);
        builder.PlaceSixteenthNote(meas, 4, 1, 2);

        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 1, 3);
        builder.PlaceSixteenthNote(meas, 2, 1, 3);
        builder.PlaceSixteenthNote(meas, 3, 1, 3);
        builder.PlaceSixteenthNote(meas, 4, 1, 2);
        builder.PlaceSixteenthNote(meas, 4, 4, 2);

        meas += 1;
        
        builder.PlaceSixteenthNote(meas, 1, 4, 1);
        builder.PlaceSixteenthNote(meas, 2, 2, 1);
        builder.PlaceSixteenthNote(meas, 2, 4, 2);
        builder.PlaceSixteenthNote(meas, 4, 1, 3);
        builder.PlaceSixteenthNote(meas, 4, 4, 3);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 2, 3);
        builder.PlaceSixteenthNote(meas, 2, 2, 3);
        builder.PlaceSixteenthNote(meas, 3, 1, 2);
        builder.PlaceSixteenthNote(meas, 3, 4, 2);

        builder.PlaceSixteenthNote(meas, 4, 3, 1);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 1, 1); //50th note


        //builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 1, 4, 2);
        builder.PlaceSixteenthNote(meas, 2, 2, 2);
        //builder.PlaceSixteenthNote(meas, 2, 4, 1);

        builder.PlaceSixteenthNote(meas, 3, 2, 3);
        builder.PlaceSixteenthNote(meas, 3, 4, 3);
        builder.PlaceSixteenthNote(meas, 4, 3, 3);
        meas += 1;
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 1, 1);
        builder.PlaceSixteenthNote(meas, 2, 1, 2);
        builder.PlaceSixteenthNote(meas, 3, 1, 3);
        builder.PlaceSixteenthNote(meas, 4, 1, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 1, 3);
        builder.PlaceSixteenthNote(meas, 2, 1, 2);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);
        builder.PlaceSixteenthNote(meas, 4, 1, 2);
        meas += 2;

        builder.PlaceSixteenthNote(meas, 4, 3, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 2, 1);
        builder.PlaceSixteenthNote(meas, 1, 4, 1);
        builder.PlaceSixteenthNote(meas, 2, 2, 1);
        
        builder.PlaceSixteenthNote(meas, 3, 1, 2);
        builder.PlaceSixteenthNote(meas, 3, 3, 2);
        builder.PlaceSixteenthNote(meas, 4, 1, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 4, 2);
        builder.PlaceSixteenthNote(meas, 2, 2, 2);
        
        builder.PlaceSixteenthNote(meas, 3, 1, 3);
  
        builder.PlaceSixteenthNote(meas, 3, 3, 3);
        builder.PlaceSixteenthNote(meas, 4, 1, 3);

        meas += 1;
        //start of fucked section:
        builder.PlaceSixteenthNote(meas, 1, 3, 3);
        builder.PlaceSixteenthNote(meas, 2, 1, 2);

        builder.PlaceSixteenthNote(meas, 3, 1, 1);



        builder.PlaceSixteenthNote(meas, 4, 1, 1);


        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 1, 1);


        builder.PlaceSixteenthNote(meas, 2, 3, 1);
        builder.PlaceSixteenthNote(meas, 3, 1, 1);

        //new
        builder.PlaceSixteenthNote(meas, 4, 3, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 2, 1);
        builder.PlaceSixteenthNote(meas, 1, 4, 1);
        builder.PlaceSixteenthNote(meas, 2, 2, 1);
        
        builder.PlaceSixteenthNote(meas, 3, 1, 2);
        builder.PlaceSixteenthNote(meas, 3, 3, 2);
        builder.PlaceSixteenthNote(meas, 4, 1, 2);
        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 4, 2);
        builder.PlaceSixteenthNote(meas, 2, 2, 2);
        
        builder.PlaceSixteenthNote(meas, 2, 4, 3);
        //builder.PlaceSixteenthNote(meas, 3, 3, 1);
        builder.PlaceSixteenthNote(meas, 3, 3, 3);
        builder.PlaceSixteenthNote(meas, 4, 1, 3);

        meas += 1;
        builder.PlaceSixteenthNote(meas, 1, 4, 3);
        builder.PlaceSixteenthNote(meas, 2, 3, 2);

        builder.PlaceSixteenthNote(meas, 4, 1, 1);

        builder.PlaceSixteenthNote(meas, 4, 4, 1);
        meas += 1;

        builder.PlaceSixteenthNote(meas, 1, 2, 1);

        builder.PlaceSixteenthNote(meas, 2, 2, 1);
        builder.PlaceSixteenthNote(meas, 3, 3, 1);
        builder.PlaceSixteenthNote(meas, 4, 1, 1);

        // builder.PlaceSixteenthNote(meas, 1, 3, 3);
        // builder.PlaceSixteenthNote(meas, 2, 1, 2);
        // builder.PlaceSixteenthNote(meas, 2, 3, 1);
        // builder.PlaceSixteenthNote(meas, 3, 1, 1);
        // builder.PlaceSixteenthNote(meas, 4, 1, 1);
        // meas += 1;
        // builder.PlaceSixteenthNote(meas, 1, 1, 1);
        // builder.PlaceSixteenthNote(meas, 2, 3, 1);
        // builder.PlaceSixteenthNote(meas, 3, 1, 1);

        return builder.GetBeatMap();

    }
}
