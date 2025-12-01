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


    [Header("Game Manager:")]
    public ManageGameSM gameManager;

    private double time_in_song;
    private int curr_tick;
    private int curr_meas;
    private int curr_qNote;
    private int curr_sNote;
    private int last_meas = -1;

    private int last_tick = -1;

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
            //float offset = .11f;
            time_in_song = gameManager.musicSource.time + (60 / gameManager.bpm) * 16;

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

            if (curr_meas != last_meas
                && curr_sNote >= 0 && curr_qNote >= 0 && curr_meas >= 0)
            {
                int next_input = gameManager.beat_map[curr_meas].qNotes[0].sNotes[1];
                if (next_input == -1) // Secret sygnal not picked up by gamehandler ideally
                {
                    SpawnWine();
                }

                last_meas = curr_meas; // Wait until we get to the next tick (tick defined above)
            }

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
        }
    }

    void SpawnWine()
    {
        //Create Wine
        Vector3 start = Opp.transform.position;
        Vector3 dest = Player.transform.position;
        GameObject note_obj = Instantiate(Wine, start, Quaternion.identity);

        //Send it and call its destructor
        note_obj.GetComponent<StaticWine>().Send((60 / gameManager.bpm) * 16, dest - start);
        Destroy(note_obj, (float)(60 / gameManager.bpm) * 16);
    }

}

