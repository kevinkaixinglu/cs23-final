using NUnit.Framework.Internal;
using UnityEngine;

public class WineSpawnerML : MonoBehaviour
{
    [Header("Visuals:")]
    public GameObject Player;
    public GameObject Opp;
    public GameObject Wine;
    public GameObject Table;

    private Animator Opp_Anim;

    [Header("Chirp Sounds")]
    public AudioSource chirp1;
    public AudioSource chirp2;
    public AudioSource chirp3;


    [Header("Game Manager:")]
    public ManageGameSM gameManager;

    private double time_in_song;
    private int curr_tick;
    private int curr_meas;
    private int curr_qNote;
    private int curr_sNote;

    private int last_qNote = -1;
    private int last_tick = -1;

    // BEAT = 2 qNotes
    private int BEATS_AHEAD = 4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Opp_Anim = Opp.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isPlaying)
        {

            /// //
            /// SPAWN WINE
            /// //

            //float offset = .11f;
            time_in_song = gameManager.musicSource.time 
                + (60 / gameManager.bpm) * (16 + 2 * BEATS_AHEAD);

            if (time_in_song >= gameManager.musicSource.clip.length)
            {
                curr_tick = 0;
            }
            else
            {
                curr_tick = ((int)(time_in_song * (gameManager.bpm / 60) * 4)) - 1; // tick = note relative to whole song
            }
            curr_meas = (curr_tick) / 16;
            curr_qNote = ((curr_tick % 16) / 4);
            curr_sNote = curr_tick % 4;

            if (curr_qNote != last_qNote
                && curr_sNote >= 0 && curr_qNote >= 0 && curr_meas >= 0)
            {
                int next_input = gameManager.beat_map[curr_meas].qNotes[curr_qNote].sNotes[1];
                if (next_input == -1) // Secret sygnal not picked up by gamehandler ideally
                {
                    SpawnWine();
                }

                last_qNote = curr_qNote; // Wait until we get to the next tick (tick defined above)
            }

            /// //
            /// PUMP THE OPPOSITE BIRD
            /// //

            //float offset = .11f;
            time_in_song = gameManager.musicSource.time
                + (60 / gameManager.bpm) * 16;

            if (time_in_song >= gameManager.musicSource.clip.length)
            {
                curr_tick = 0;
            }
            else
            {
                curr_tick = ((int)(time_in_song * (gameManager.bpm / 60) * 4)) - 1; // tick = note relative to whole song
            }
            curr_meas = (curr_tick) / 16;
            curr_qNote = ((curr_tick % 16) / 4);
            curr_sNote = curr_tick % 4;

            if (curr_tick != last_tick
                && curr_sNote >= 0 && curr_qNote >= 0 && curr_meas >= 0)
            {
                int next_input = gameManager.beat_map[curr_meas].qNotes[curr_qNote].sNotes[curr_sNote];
                if (next_input > 0)
                {
                    Opp_Anim.Play("Pump");
                }

                last_tick = curr_tick; // Wait until we get to the next tick (tick defined above)
            }

            /// //
            /// PLAY CHIRP
            /// //
            /// 

            float offset = .05f;
            time_in_song = gameManager.musicSource.time
                + (60 / gameManager.bpm) * 16 
                + offset;

            if (time_in_song >= gameManager.musicSource.clip.length)
            {
                curr_tick = 0;
            }
            else
            {
                curr_tick = ((int)(time_in_song * (gameManager.bpm / 60) * 4)) - 1; // tick = note relative to whole song
            }
            curr_meas = (curr_tick) / 16;
            curr_qNote = ((curr_tick % 16) / 4);
            curr_sNote = curr_tick % 4;

            if (curr_tick != last_tick
                && curr_sNote >= 0 && curr_qNote >= 0 && curr_meas >= 0)
            {
                int next_input = gameManager.beat_map[curr_meas].qNotes[curr_qNote].sNotes[curr_sNote];
                if (next_input == 1)
                {
                    chirp2.time = 0.08f;
                    chirp2.pitch = Random.Range(0.9f, 1.1f);
                    chirp2.Play();
                }

                last_tick = curr_tick; // Wait until we get to the next tick (tick defined above)
            }

        }
    }

    void SpawnWine()
    {
        //Create Wine
        Vector3 start = Opp.transform.position;
        Vector3 dest = Player.transform.position;
        Vector3 step = (dest - start) / 8;
        start = start - step * BEATS_AHEAD;
        dest = dest + step * BEATS_AHEAD;
        GameObject note_obj = Instantiate(Wine, start, Quaternion.identity);

        //Send it and call its destructor
        note_obj.GetComponent<StaticWine>().Send((60 / gameManager.bpm) * (16 + 4 * BEATS_AHEAD)
                                                , dest - start);
        Destroy(note_obj, (float)((60 / gameManager.bpm) * (16 + 4 * BEATS_AHEAD)));
    }

}

