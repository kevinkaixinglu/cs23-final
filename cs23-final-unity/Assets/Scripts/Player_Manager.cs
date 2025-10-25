using UnityEngine;
using System.Collections;


public class Player_Manager : MonoBehaviour
{
    //Assign sprites/gameobjects
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

    private int i = 0;
    public Leader_Manager leaderManager;

    void Start()
    {
        ResetArrows();
    }

    void Update()
    {
        //if leader manager tells us it's player turn, start
        if (leaderManager.playerTurn == true)
        {
            //run follow sequence and turn off our turn and start next leader sequence
            ResetArrows();
            StartCoroutine(StartSequence(i)); // start coroutine
            i++;
            leaderManager.playerTurn = false;
            leaderManager.startNextSequence = true;
        }
    }

    void ResetArrows()
    {
        arrowUp.GetComponentInChildren<SpriteRenderer>().sprite = arrowUpOff;
        arrowDown.GetComponentInChildren<SpriteRenderer>().sprite = arrowDownOff;
        arrowLeft.GetComponentInChildren<SpriteRenderer>().sprite = arrowLeftOff;
        arrowRight.GetComponentInChildren<SpriteRenderer>().sprite = arrowRightOff;
    }

    IEnumerator StartSequence(int sequenceIndex)
    {
        //find which sequence we are following
        switch (sequenceIndex)
        {
            case 0:
                yield return StartCoroutine(SequenceZero());
                break;
            case 1:
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
        float timer = 0f;
        bool inputOne = false;
        bool inputTwo = false;
        bool inputThree = false;
        bool inputFour = false;
        int score = 0;
        bool playZero = true;
        Debug.Log("Starting Sequence One\n");
        //give .5 sec buffer
        yield return new WaitForSeconds(0.5f);
        while (playZero) // fake time loop?
        {
            //increment time
            timer += Time.deltaTime;
        
            //turn on any keys being pressed
            activateKeys();

            //Check if they are pressing at right time and increment score
            if (Input.GetKeyDown(KeyCode.UpArrow) && timer >= 0.0 && timer <= .5 && !inputOne)
            {
                score++;
                Debug.Log("Scored Input One\n");
                inputOne = true;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && timer >= 0.8 && timer <= 1.3 && !inputTwo)
            {
                score++;
                Debug.Log("Scored Input Two\n");
                inputTwo = true;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) && timer >= 1.8 && timer <= 2.3 && !inputThree)
            {
                score++;
                Debug.Log("Scored Input Three\n");
                inputThree = true;
            }
            //End sequence no matter what
            if (Input.GetKeyDown(KeyCode.RightArrow) && timer >= 2.8 && timer <= 3.3 && !inputFour || timer >= 3.5)
            {
                score++;
                Debug.Log("Scored Input Four\n");
                inputFour = true;
                playZero = false;
            }
            yield return null; // wait for next frame
        }
    Debug.Log("SCORE: " + score + "\n");
    }

    IEnumerator SequenceOne()
    {
        // Example placeholder
        yield return null;
    }

    IEnumerator SequenceTwo()
    {
        // Example placeholder
        yield return null;
    }

    void activateKeys() {
        SpriteRenderer sr;

        //activate/deactivate up key
        if (Input.GetKey(KeyCode.UpArrow))
        {
            sr = arrowUp.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowUpOn;
        } else {
            sr = arrowUp.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowUpOff;
        }

        //activate/deactivate down key
        if (Input.GetKey(KeyCode.DownArrow))
        {
            sr = arrowDown.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowDownOn;
        } else {
            sr = arrowDown.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowDownOff;
        }

        //activate/deactivate left key
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            sr = arrowLeft.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowLeftOn;
        } else {
            sr = arrowLeft.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowLeftOff;
        }

        //activate/deactivate right key
        if (Input.GetKey(KeyCode.RightArrow))
        {
            sr = arrowRight.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowRightOn;
        } else {
            sr = arrowRight.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = arrowRightOff;
        }
    }
}
