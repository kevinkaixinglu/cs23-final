using UnityEngine;
using System.Collections;

public class Leader_Manager : MonoBehaviour
{
    //Assign all Sprites + Gameobjects
    [Header("Arrow GameObjects")]
    public GameObject arrowUp;
    public GameObject arrowDown;
    public GameObject arrowLeft;
    public GameObject arrowRight;

    [Header("Arrow Sprites")]
    public Sprite arrowUpOff;
    public Sprite arrowUpOn;
    public Sprite arrowDownOff;
    public Sprite arrowDownOn;
    public Sprite arrowLeftOff;
    public Sprite arrowLeftOn;
    public Sprite arrowRightOff;
    public Sprite arrowRightOn;

    
    public bool startNextSequence = false;
    public bool playerTurn = false;
    private int i = 0;

    void Start()
    {
        //Reset the Arrows and start the sequences (set bool true from main menu start)
        ResetArrows();
        startNextSequence = true;
    }

    void Update()
    {
        //If it is not the player's turn and the sequence should be started
        if (startNextSequence == true && playerTurn == false)
        {
            //Start the sequence and then increment for next sequence
            startNextSequence = false;
            ResetArrows();
            StartCoroutine(StartSequence(i)); // start coroutine
            i++;
        }
    }

    //Turn all arrows off
    void ResetArrows()
    {
        arrowUp.GetComponentInChildren<SpriteRenderer>().sprite = arrowUpOff;
        arrowDown.GetComponentInChildren<SpriteRenderer>().sprite = arrowDownOff;
        arrowLeft.GetComponentInChildren<SpriteRenderer>().sprite = arrowLeftOff;
        arrowRight.GetComponentInChildren<SpriteRenderer>().sprite = arrowRightOff;
    }

    
    IEnumerator StartSequence(int sequenceIndex)
    {
        //if statement to start the correct sequence/case
        switch (sequenceIndex)
        {
            case 0:
                //Run leader/demo sequence and then start player's turn
                yield return StartCoroutine(SequenceZero());
                playerTurn = true;
                break;
            case 1:
                Debug.Log("PLAYER FINISHED SEQUENCE, START 2!");

                yield return StartCoroutine(SequenceOne());
                break;
            case 2:
                yield return StartCoroutine(SequenceTwo());
                break;
        }
        ResetArrows();
    }

    IEnumerator SequenceZero()
    {
        SpriteRenderer sr;

        //turn on up arrow
        sr = arrowUp.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = arrowUpOn;
        yield return new WaitForSeconds(1f);

        //turn on down arrow
        ResetArrows();
        sr = arrowDown.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = arrowDownOn;
        yield return new WaitForSeconds(1f);

        //turn on left arrow
        ResetArrows();
        sr = arrowLeft.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = arrowLeftOn;
        yield return new WaitForSeconds(1f);

        //turn on right arrow
        ResetArrows();
        sr = arrowRight.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = arrowRightOn;
        yield return new WaitForSeconds(1f);
    }

    IEnumerator SequenceOne()
    {
        yield return null;
    }

    IEnumerator SequenceTwo()
    {
        yield return null;
    }
}
