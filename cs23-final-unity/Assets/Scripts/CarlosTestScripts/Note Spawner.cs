using UnityEditor;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Visuals:")]
    public GameObject line;
    public GameObject note_prefab;

    [Header("Game Manager:")]
    public ManageGame gameManager;

    [Header("Timing for spawning notes:")]
    public double seconds_in_future = 2;

    private int last_tick = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        double time_in_song = gameManager.time_in_song + seconds_in_future;
        int curr_tick = ((int)(time_in_song * (gameManager.bpm / 60) * 4)) - 1; // tick = note relative to whole song
        int curr_meas = (curr_tick) / 16;
        int curr_beat = ((curr_tick % 16) / 4);
        int curr_note = curr_tick % 4;

        int currMeas = gameManager.curr_meas;
        int currBeat = gameManager.curr_beat;
        int currNote = gameManager.curr_note;

        //END OF KALEN'S NEW CODE

        if (curr_tick != last_tick && curr_note >= 0 && curr_beat >= 0 && curr_meas >= 0)
        {
            int next_input = gameManager.beat_map[curr_meas].beats[curr_beat].notes[curr_note];
            if (next_input != 0)
            {
                //Get note position
                float camera_hor_radius = 8.88f; // I measured this
                float new_x = camera_hor_radius * 2f * .20f * next_input - camera_hor_radius;
                Vector3 pos = new Vector3(new_x, transform.position.y, 0);

                //Create Note
                GameObject note = Instantiate(note_prefab, pos, Quaternion.identity);

                //Get velocity
                double dist_to_line = line.transform.position.y - transform.position.y;
                Vector2 vel = new Vector2(0, (float)(dist_to_line / seconds_in_future));

                //Send it towards line
                Rigidbody2D rb = note.GetComponent<Rigidbody2D>();
                rb.linearVelocity = vel; // Send it down

                //Call its destruction
                note.GetComponent<Destroy_Note>().callDestruction(seconds_in_future + 1);
            }

            last_tick = curr_tick; // Wait until we get to the next tick (tick defined above)
        }

    }
}
