using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UpNote : MonoBehaviour
{

    public double time;
    public Vector3 dest;

    private bool sent = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!sent)
        {
            //Get velocity
            Vector2 vel = new Vector2(0, (float)(dest.y / time));

            //Send it towards line
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = vel; // Send it down

            sent = true;
        }
    }
}
