using UnityEngine;
using System.Collections;

public class ConveyorPlayer : MonoBehaviour
{
    private Collider2D currBeat; // Stores the beat in the zone
    private bool isMoving = false;
    private int indexLocation = 1;

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
            if (currBeat != null)
            {
                float distanceToCenter = Mathf.Abs(currBeat.transform.position.x - transform.position.x);

                if (distanceToCenter < 0.1f)
                    Debug.Log("Perfect!");
                else if (distanceToCenter < 0.3f)
                    Debug.Log("Good!");
                else if (distanceToCenter < 0.5f)
                    Debug.Log("Okay");
                else
                    Debug.Log("Miss");

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
