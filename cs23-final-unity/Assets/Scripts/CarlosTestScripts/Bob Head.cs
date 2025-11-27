using UnityEngine;

public class BobHead : MonoBehaviour
{
    public SpriteRenderer sprite;
    public ManageGameML manageGameML;
    public Animator animator;

    private int last_qNote = 0;

    // Update is called once per frame
    void Update()
    {
        double time_in_song = manageGameML.musicSource.time - .1f;
        int curr_tick = (int)(time_in_song * (manageGameML.bpm / 60) * 4 - 1);
        int curr_qNote = (curr_tick / 4);
        if (curr_qNote != last_qNote)
        {
            last_qNote = curr_qNote;
            animator.Play("Pump");
            //if (last_qNote % 2 == 0)
            //{
            //    animator.Play("Pump");
            //}
            //else
            //{
            //    //sprite.transform.localScale = new Vector3(1f, 1f, 1f);
            //}
        }
    }
}
