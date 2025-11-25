using UnityEngine;

public class ConveyorPlayer : MonoBehaviour
{
    private Collider2D currBeat; // Stores the beat in the zone

    private bool up = false;
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
        if (Input.GetKeyDown(KeyCode.UpArrow) && up == false) {
            Vector2 movement = new Vector2(0f, 20f * 10 * Time.deltaTime);
            transform.Translate(movement);
        }
    }
}
