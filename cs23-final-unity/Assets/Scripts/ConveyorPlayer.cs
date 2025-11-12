using UnityEngine;

public class ConveyorPlayer : MonoBehaviour
{
    public Collider2D bird;
    public Collider2D beat;
    public float margin = 0.02f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("running");
        if (fullyInside(bird, beat)) {
            Debug.Log("hit");
        }
    }

    bool fullyInside(Collider2D bird, Collider2D beat) {
        Bounds birdBounds = bird.bounds;
        Bounds beatBounds = beat.bounds;
        return birdBounds.Contains(beatBounds.min + Vector3.one * margin) &&
               birdBounds.Contains(beatBounds.max - Vector3.one * margin);
    }
}
