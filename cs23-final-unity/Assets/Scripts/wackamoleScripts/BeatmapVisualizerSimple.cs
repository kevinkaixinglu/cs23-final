using UnityEngine;

public abstract class BeatmapVisualizerSimple : MonoBehaviour
{
    [Header("Rhythm Timer")]
    public RhythmTimer rhythmTimer;

    [Header("NPC Beat Map")]
    public Measure[] npcBeatMap;

    private int lastTick = -1;

    void Update()
    {
        if (rhythmTimer == null || !rhythmTimer.isPlaying)
            return;

        int currMeas = rhythmTimer.curr_meas;
        int currBeat = rhythmTimer.curr_beat;
        int currNote = rhythmTimer.curr_note;

        // Only trigger once per tick
        if (rhythmTimer.curr_tick != lastTick &&
            currMeas >= 0 && currBeat >= 0 && currNote >= 0 &&
            currMeas < npcBeatMap.Length)
        {
            int noteValue = npcBeatMap[currMeas].beats[currBeat].notes[currNote];
            OnBeatTriggered(noteValue);
            lastTick = rhythmTimer.curr_tick;
        }
    }

    // Each subclass decides how to visually react to a note.
    protected abstract void OnBeatTriggered(int noteValue);
}