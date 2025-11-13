using UnityEngine;

public abstract class BeatmapVisualizer : MonoBehaviour
{
    [Header("Game Manager")]
    public ManageGame gameManager;

    [Header("NPC Beat Map")]
    public Measure[] npcBeatMap;

    private int lastTick = -1;

    void Update()
    {
        if (gameManager == null || !gameManager.isPlaying)
            return;

        int currMeas = gameManager.curr_meas;
        int currBeat = gameManager.curr_beat;
        int currNote = gameManager.curr_note;

        // Only trigger once per tick
        if (gameManager.curr_tick != lastTick &&
            currMeas >= 0 && currBeat >= 0 && currNote >= 0 &&
            currMeas < npcBeatMap.Length)
        {
            int noteValue = npcBeatMap[currMeas].beats[currBeat].notes[currNote];
            OnBeatTriggered(noteValue);
            lastTick = gameManager.curr_tick;
        }
    }


    // Each subclass decides how to visually react to a note.
    protected abstract void OnBeatTriggered(int noteValue);
}
