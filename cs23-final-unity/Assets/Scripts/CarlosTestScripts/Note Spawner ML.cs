using UnityEngine;

public class NoteSpawnerML : MonoBehaviour
{
    [Header("Visuals:")]
    public GameObject static_note;
    public GameObject dynamic_note;

    [Header("Game Manager:")]
    public ManageGameML gameManager;

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
        for (int i = 0; i < 4; i++)
        {
            last_tick[i] = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isPlaying)
        {
            float offset = .11f;
            time_in_song[0] = gameManager.musicSource.time + upNote_SpawnTime;
            time_in_song[1] = gameManager.musicSource.time + downNote_SpawnTime;
            time_in_song[2] = gameManager.musicSource.time + leftNote_SpawnBeat * 60 / gameManager.bpm - offset;
            time_in_song[3] = gameManager.musicSource.time + rightNote_SpawnBeat * 2 * 60 / gameManager.bpm - offset;

            for (int i = 0; i < 4; i++)
            {
                if (time_in_song[i] >= gameManager.musicSource.clip.length)
                {
                    curr_tick[i] = 0;
                }
                else
                {
                    curr_tick[i] = ((int)(time_in_song[i] * (gameManager.bpm / 60) * 4)) - 1; // tick = note relative to whole song
                }
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
                        SpawnNote(i + 1);
                    }

                    last_tick[i] = curr_tick[i]; // Wait until we get to the next tick (tick defined above)
                }
            }
        }
    }

    void SpawnNote(int note)
    {

        Vector3 dest = transform.position; // Go towards NoteSpawner
        float edge_of_screen = 9.5f;
        float sno_up = 1f; // Static Note Offset
        float sno_down = .7f; // Static Note Offset

        if (note == 1) // UpNote
        {
            //Create Note
            Vector3 start = transform.position
                    + (transform.up * edge_of_screen);
            GameObject note_obj = Instantiate(static_note, start + (transform.up * sno_up), Quaternion.identity);

            //Send it and call its destructor
            note_obj.GetComponent<StaticNote>().Send(upNote_SpawnTime, dest - start);
            Destroy(note_obj, (float)upNote_SpawnTime + .12f);
        }
        else if (note == 2) // DownNote
        {
            //Create Note
            Vector3 start = transform.position
                    - (transform.up * edge_of_screen);
            GameObject note_obj = Instantiate(static_note, start - (transform.up * sno_down), Quaternion.identity);

            //Send it and call its destructor
            note_obj.GetComponent<StaticNote>().Send(downNote_SpawnTime, dest - start);
            Destroy(note_obj, (float)downNote_SpawnTime + .12f);
        }
        else if (note == 3) // LeftNote
        {
            //Create Note
            Vector3 start = transform.position - (transform.right * edge_of_screen);
            GameObject note_obj = Instantiate(dynamic_note, start, Quaternion.identity);

            //Send it and call its destructor
            int qNotes_per_beat = 1;
            note_obj.GetComponent<DynamicNoteML>().qNotes_per_beat = qNotes_per_beat; // beats = 1 qNote
            note_obj.GetComponent<DynamicNoteML>().beats = leftNote_SpawnBeat;
            note_obj.GetComponent<DynamicNoteML>().dest = dest - start;
            note_obj.GetComponent<DynamicNoteML>().gameManager = gameManager;
            note_obj.GetComponent<DynamicNoteML>().sent = true;
            Destroy(note_obj, (float)leftNote_SpawnBeat * qNotes_per_beat * 60 / (float)gameManager.bpm + .1f);
        }
        else if (note == 4) // RightNote
        {
            //Create Note
            Vector3 start = transform.position + (transform.right * edge_of_screen);
            GameObject note_obj = Instantiate(dynamic_note, start, Quaternion.identity);

            //Send it and call its destructor
            int qNotes_per_beat = 2;
            note_obj.GetComponent<DynamicNoteML>().qNotes_per_beat = qNotes_per_beat; // beats = 2 qNote
            note_obj.GetComponent<DynamicNoteML>().beats = rightNote_SpawnBeat;
            note_obj.GetComponent<DynamicNoteML>().dest = dest - start;
            note_obj.GetComponent<DynamicNoteML>().gameManager = gameManager;
            note_obj.GetComponent<DynamicNoteML>().sent = true;
            Destroy(note_obj, (float)rightNote_SpawnBeat * qNotes_per_beat * 60 / (float)gameManager.bpm + .1f);
        }
    }

}
