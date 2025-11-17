using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NoteSpawner2 : MonoBehaviour
{
    [Header("Visuals:")]
    public GameObject note_up;
    public GameObject note_down;
    public GameObject note_left;
    public GameObject note_right;

    [Header("Game Manager:")]
    public ManageGame gameManager;

    [Header("Timing for spawning notes:")]
    public double upNote_SpawnTime = 2;
    public double downNote_SpawnTime = 2;
    public int leftNote_SpawnBeat = 2;
    public int rightNote_SpawnBeat = 2;

    private double[] time_in_song = new double[4];
    private int[] curr_tick = new int[4];
    private int[] curr_meas = new int[4];
    private int[] curr_qNote = new int[4];
    private int[] curr_sNote = new int[4];
    private int[] last_tick = new int[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            last_tick[i] = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time_in_song[0] = gameManager.time_in_song + upNote_SpawnTime;
        time_in_song[1] = gameManager.time_in_song + downNote_SpawnTime;
        time_in_song[2] = gameManager.time_in_song + leftNote_SpawnBeat * 60 / gameManager.bpm;
        time_in_song[3] = gameManager.time_in_song + rightNote_SpawnBeat * 60 / gameManager.bpm;

        for (int i = 0; i < 4; i++)
        {
            curr_tick[i] = ((int)(time_in_song[i] * (gameManager.bpm / 60) * 4)) - 1; // tick = note relative to whole song
            curr_meas[i] = (curr_tick[i]) / 16;
            curr_qNote[i] = ((curr_tick[i] % 16) / 4);
            curr_sNote[i] = curr_tick[i] % 4;
        }

        for (int i = 0; i < 4; i++)
        {
            if (curr_tick[i] != last_tick[i]
                && curr_sNote[i] >= 0 && curr_qNote[i] >= 0 && curr_meas[i] >= 0)
            {
                int next_input = gameManager.beat_map[curr_meas[i]].qNotes[curr_qNote[i]].sNotes[curr_sNote[i]];
                if (next_input == 1 + 4 * i)
                {
                    SpawnNote(i + 1, time_in_song[i] - gameManager.time_in_song);
                }

                last_tick[i] = curr_tick[i]; // Wait until we get to the next tick (tick defined above)
            }
        }
    }

    void SpawnNote(int note, double time)
    {

        Vector3 dest = transform.position; // Go towards NoteSpawner

        if (note == 1) // UpNote
        {
            //Create Note
            Vector3 start = transform.position + (transform.up * 9.5f);
            GameObject note_obj = Instantiate(note_up, start, Quaternion.identity);

            //Give it info and call its destruction
            note_obj.GetComponent<UpNote>().time = time;
            note_obj.GetComponent<UpNote>().dest = dest - start;
            Destroy(note_obj, (float)time + .04f);
        }
        else if (note == 2) // DownNote
        {
            //Create Note
            Vector3 start = transform.position + (-transform.up * 9.5f);
            GameObject note_obj = Instantiate(note_up, start, Quaternion.identity);

            //Give it info and call its destruction
            note_obj.GetComponent<UpNote>().time = time;
            note_obj.GetComponent<UpNote>().dest = dest - start;
            Destroy(note_obj, (float)time + .04f);
        }
        else if (note == 3) // LeftNote
        {

        }
        else if (note == 4) // RightNote
        {

        }
    }

}
