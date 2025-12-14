using UnityEngine;
using DG.Tweening;
public class DynamicNoteML : MonoBehaviour
{
    [Header("Set by NoteSpawner:")]
    public int qNotes_per_beat;
    public int beats;
    public Vector3 dest;
    public ManageGameML gameManager;
    public bool sent = false;

    private int last_beat = -99;

    // Update is called once per frame
    void Update()
    {
        if (sent)
        {
            double time_in_song = gameManager.musicSource.time - .01f;
            int curr_tick = (int)(time_in_song * (gameManager.bpm / 60) * 4 - 1);
            int curr_qNote = (curr_tick / 4); // Relative to song, not meas
            int curr_beat = curr_qNote / qNotes_per_beat;
            if (curr_beat != last_beat)
            {
                if (last_beat != -99)
                {
                    //transform.position += dest / beats;
                    transform.DOMove(transform.position + dest / beats, .1f)
                        .SetEase(Ease.InQuad)
                        .SetLink(gameObject);
                }
                last_beat = curr_beat;
            }
        }
    }
}
