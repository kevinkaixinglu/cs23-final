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
        int curr_qNote = gameManager.curr_qNote;
        int curr_sNote = gameManager.curr_sNote;

        // Only trigger once per tick
        if (gameManager.curr_tick != lastTick &&
            currMeas >= 0 && curr_qNote >= 0 && curr_sNote >= 0 &&
            currMeas < npcBeatMap.Length)
        {
            int noteValue = npcBeatMap[currMeas].qNotes[curr_qNote].sNotes[curr_sNote];
            OnBeatTriggered(noteValue);
            lastTick = gameManager.curr_tick;
        }
    }


    // Each subclass decides how to visually react to a note.
    protected abstract void OnBeatTriggered(int noteValue);
}
