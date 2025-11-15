using UnityEngine;

public class beatmapBuilder
{
    private Measure[] beatMap;

    public beatmapBuilder(Measure[] existingBeatMap)
    {
        beatMap = existingBeatMap;
    }

    public beatmapBuilder(int numberOfMeasures)
    {
        beatMap = new Measure[numberOfMeasures];
        for (int i = 0; i < numberOfMeasures; i++)
        {
            beatMap[i] = beatmapHelper.CreateEmptyMeasure();
        }
    }

    public beatmapBuilder PlaceEighthNote(int measure, int beat, int position = 1, int key = 0)
    {
        int m = measure - 1;
        int b = beat - 1;
        int p = position - 1;
        
        if (IsValid(m, b))
        {
            beatmapHelper.SetEighthNotes(beatMap[m], b, p, key);
        }
        return this;
    }

    public beatmapBuilder PlaceQuarterNote(int measure, int beat, int key = 0)
    {
        int m = measure - 1;
        int b = beat - 1;
        
        if (IsValid(m, b))
        {
            beatmapHelper.SetBeatNote(beatMap[m], b, key, beatmapHelper.QUARTER);
        }
        return this;
    }

    public beatmapBuilder PlaceHalfNote(int measure, int beat, int key = 0)
    {
        int m = measure - 1;
        int b = beat - 1;
        
        if (IsValid(m, b))
        {
            beatmapHelper.SetBeatNote(beatMap[m], b, key, beatmapHelper.HALF);
        }
        return this;
    }

    public beatmapBuilder PlaceWholeNote(int measure, int key = 0)
    {
        int m = measure - 1;
        
        if (m >= 0 && m < beatMap.Length)
        {
            beatmapHelper.SetBeatNote(beatMap[m], 0, key, beatmapHelper.WHOLE);
        }
        return this;
    }

    public beatmapBuilder PlaceSixteenthNote(int measure, int beat, int position, int key = 0)
    {
        int m = measure - 1;
        int b = beat - 1;
        int p = position - 1;
        
        if (IsValid(m, b) && p >= 0 && p < 4)
        {
            beatmapHelper.SetNote(beatMap[m], b, p, key, beatmapHelper.SIXTEENTH);
        }
        return this;
    }

    public beatmapBuilder FillMeasureWithEighthNotes(int measure, int key = 0)
    {
        int m = measure - 1;
        
        if (m >= 0 && m < beatMap.Length)
        {
            for (int beat = 0; beat < 4; beat++)
            {
                beatmapHelper.SetEighthNotes(beatMap[m], beat, 0, key);
                beatmapHelper.SetEighthNotes(beatMap[m], beat, 1, key);
            }
        }
        return this;
    }

    public beatmapBuilder FillMeasureWithQuarterNotes(int measure, int key = 0)
    {
        int m = measure - 1;
        
        if (m >= 0 && m < beatMap.Length)
        {
            for (int beat = 0; beat < 4; beat++)
            {
                beatmapHelper.SetBeatNote(beatMap[m], beat, key, beatmapHelper.QUARTER);
            }
        }
        return this;
    }

    public beatmapBuilder ClearMeasure(int measure)
    {
        int m = measure - 1;
        
        if (m >= 0 && m < beatMap.Length)
        {
            beatmapHelper.ClearMeasure(beatMap[m]);
        }
        return this;
    }

    public beatmapBuilder ClearBeat(int measure, int beat)
    {
        int m = measure - 1;
        int b = beat - 1;
        
        if (IsValid(m, b))
        {
            for (int i = 0; i < 4; i++)
            {
                beatMap[m].qNotes[b].sNotes[i] = 0;
            }
        }
        return this;
    }

    public Measure[] GetBeatMap()
    {
        return beatMap;
    }

    private bool IsValid(int measureIndex, int beatIndex)
    {
        return measureIndex >= 0 && measureIndex < beatMap.Length &&
               beatIndex >= 0 && beatIndex < 4;
    }
}