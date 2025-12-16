using UnityEngine;
using System.Collections;
using TMPro;

public class ConveyorPlayer : MonoBehaviour
{
    public GameHandlerCopy GHS;
    private Collider2D currBeat; // Stores the beat in the zone
    private bool isMoving = false;
    private int indexLocation = 1;
    private int x = 1;

    [Header("Animation:")]
    public Animator pop_up;
    //private bool down = false;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("ConveyorBeat1")) {
            Debug.Log("in zone");
            currBeat = other;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("ConveyorBeat1")) {
            currBeat = other;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ConveyorBeat1"))
        {
            Debug.Log("out of zone");
            if (currBeat == other) {
                currBeat = null;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Player pressed hit key
        {

            Debug.Log(x);
            x++;
            if (currBeat != null)
            {
                float distanceToCenter = Mathf.Abs(currBeat.transform.position.x - transform.position.x);

                if (distanceToCenter < 0.1f) {
                    //Debug.Log("Perfect!");
                    pop_up.Play("Good_Input");
                    GHS.addScore();
                } else if (distanceToCenter < 0.3f) {
                    //Debug.Log("Good!");
                    pop_up.Play("Good_Input");
                    GHS.addScore();
                } else if (distanceToCenter < 0.7f) {
                    //Debug.Log("Okay");
                    pop_up.Play("Good_Input");
                    GHS.addScore();
                } else {
                    //Debug.Log("Miss");
                    pop_up.Play("Bad_Input");
                }
                Destroy(currBeat.gameObject); // Remove beat once hit
                currBeat = null;
            }
            else
            {
                Debug.Log("Miss — no beat in zone!");
            }
        }
        if (!isMoving) {
            if (Input.GetKeyDown(KeyCode.UpArrow) && indexLocation != 2) {
                StartCoroutine(MoveRepeatedUp());
                if (indexLocation < 2){
                    indexLocation++;
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && indexLocation != 0) {
                StartCoroutine(MoveRepeatedDown());
                if (indexLocation > 0){
                    indexLocation--;
                }
            }
        }
    }

    IEnumerator MoveRepeatedUp() {
    isMoving = true;

    for (int i = 0; i < 3; i++)
    {
        Vector2 start = transform.position;
        Vector2 target = start + new Vector2(0, 1);
        
        //move towards target location
        while ((Vector2)transform.position != target) {
            transform.position = Vector2.MoveTowards(
                transform.position, target, 100 * Time.deltaTime);
            yield return null;
        }

    }

    isMoving = false;
    }

    IEnumerator MoveRepeatedDown() {
    isMoving = true;

    for (int i = 0; i < 3; i++)
    {
        Vector2 start = transform.position;
        Vector2 target = start + new Vector2(0, -1);
        
        while ((Vector2)transform.position != target)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                100 * Time.deltaTime
            );
            yield return null;
        }

    }

    isMoving = false;
    }
}
