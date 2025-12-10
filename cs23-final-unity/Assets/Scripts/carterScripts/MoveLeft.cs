using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    public float speed = 5f;
    public float pulseSpeed = 0.5f;     // how fast it pulses
    public float pulseAmount = 0.2f;  // how much bigger/smaller it gets
    private Vector3 originalScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //pulse
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * scale;

        transform.Translate(Vector2.left * speed * Time.deltaTime);
        if (transform.position.x < -15f) {
            Destroy(gameObject);
        }
    }
}
