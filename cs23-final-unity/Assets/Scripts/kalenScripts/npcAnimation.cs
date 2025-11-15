using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    [Header("Game Manager")]
    public ManageGame gameManager;

    [Header("Animator")]
    public Animator animator;

    [Header("NPC Beat Map")]
    public Measure[] npcBeatMap;

    // Used to record note changes
    private int lastTick = -1;

    void Update()
    {

        //if something is wrong return early
        if (gameManager == null || !gameManager.isPlaying)
            return;

        //get current measure beat and note from the game manager
        int currMeas = gameManager.curr_meas;
        int curr_qNote = gameManager.curr_qNote;
        int curr_sNote = gameManager.curr_sNote;


        //check if we're still in the beatmap and within the start of the song
        if (gameManager.curr_tick != lastTick &&
            currMeas >= 0 && curr_qNote >= 0 && curr_sNote >= 0 &&
            currMeas < npcBeatMap.Length)
        {

            //set the index of when the animation should trigger
            int animTriggerIndex = npcBeatMap[currMeas].qNotes[curr_qNote].sNotes[curr_sNote];

            //if we have a valid index to trigger, trigger the animation
            if (animTriggerIndex != 0)
            {
                string triggerName = "Anim" + animTriggerIndex.ToString(); 
                animator.SetTrigger(triggerName);
                Debug.Log($"[{Time.time:F2}] NPC triggered {triggerName} on M:{currMeas}, B:{curr_qNote}, N:{curr_sNote}");
            }

            lastTick = gameManager.curr_tick;
        }
    }

    //SEMI PSUEDOODE FOR WHEN WE HAVE THIS IMPLEMENTED! handler script will be
    //able to call one of these upon a wrong or correct note so the leader
    //can react for extra reacticity

    // public void PlayReaction(string type)
    // {
    //     if type == "had":
    //         animator.Play("happy animation");
    //     else if type == "sad":
    //         animator.Play("sad animation");
    // }   
}
