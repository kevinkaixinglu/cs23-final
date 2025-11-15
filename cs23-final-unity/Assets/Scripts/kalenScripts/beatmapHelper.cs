using UnityEngine;

public static class beatmapHelper
{
    // Note durations in sixteenth notes
    public const int SIXTEENTH = 1;
    public const int EIGHTH = 2;
    public const int QUARTER = 4;
    public const int HALF = 8;
    public const int WHOLE = 16;


    // "measure" The measure to modify
    // "beatIndex" Which beat (0-3)
    // "noteIndex" Starting position in the beat (0-3)
    // "value" The note value (1-4 for keys, 0 for rest)
    // "duration" Duration in sixteenth notes
    
    public static void SetNote(Measure measure, int beatIndex, int noteIndex, int value, int duration = SIXTEENTH)
    {
        int totalPosition = (beatIndex * 4) + noteIndex;
        
        for (int i = 0; i < duration && totalPosition < 16; i++)
        {
            int curr_qNote = totalPosition / 4;
            int currNote = totalPosition % 4;
            
            if (curr_qNote < 4 && currNote < 4)
            {
                measure.qNotes[curr_qNote].sNotes[currNote] = value;
            }
            totalPosition++;
        }
    }

    //Sets a note on a specific beat (quarter note timing)
    public static void SetBeatNote(Measure measure, int beatIndex, int value, int duration = QUARTER)
    {
        SetNote(measure, beatIndex, 0, value, duration);
    }

    //Clears a measure (sets all to 0)
    public static void ClearMeasure(Measure measure)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                measure.qNotes[i].sNotes[j] = 0;
            }
        }
    }


    // Creates a new measure with all values set to 0
    public static Measure CreateEmptyMeasure()
    {
        Measure measure = new Measure();
        measure.qNotes = new QNote[4];
        
        for (int i = 0; i < 4; i++)
        {
            measure.qNotes[i] = new QNote();
            measure.qNotes[i].sNotes = new int[4];
        }
        
        return measure;
    }


    //Sets eighth notes on specific beats
    public static void SetEighthNotes(Measure measure, int beatIndex, int eighthIndex, int value)
    {
        // eighthIndex: 0 = first eighth, 1 = second eighth of the beat
        int startPosition = eighthIndex * 2;
        SetNote(measure, beatIndex, startPosition, value, EIGHTH);
    }
}