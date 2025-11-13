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
        int currBeat = gameManager.curr_beat;
        int currNote = gameManager.curr_note;


        //check if we're still in the beatmap and within the start of the song
        if (gameManager.curr_tick != lastTick &&
            currMeas >= 0 && currBeat >= 0 && currNote >= 0 &&
            currMeas < npcBeatMap.Length)
        {

            //set the index of when the animation should trigger
            int animTriggerIndex = npcBeatMap[currMeas].beats[currBeat].notes[currNote];

            //if we have a valid index to trigger, trigger the animation
            if (animTriggerIndex != 0)
            {
                string triggerName = "Anim" + animTriggerIndex.ToString(); 
                animator.SetTrigger(triggerName);
                Debug.Log($"[{Time.time:F2}] NPC triggered {triggerName} on M:{currMeas}, B:{currBeat}, N:{currNote}");
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
