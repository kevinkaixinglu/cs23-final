using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class StaticWine : MonoBehaviour
{

    public Animator anim;

    // Update is called once per frame
    public void Send(double time, Vector3 dest)
    {

        //Get velocity
        Vector2 vel = new Vector2((float)(dest.x / time), (float)(dest.y / time));

        //Send it towards line
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = vel; // Send it down
        anim.Play("Shift");

    }
}
