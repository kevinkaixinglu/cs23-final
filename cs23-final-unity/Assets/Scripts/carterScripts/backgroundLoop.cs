using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [SerializeField] public float speed;

    private Transform background1;
    private Transform background2;
    private float spriteWidth;

    void Start()
    {
        //Set backgrounds
        background1 = transform.GetChild(0);
        background2 = transform.GetChild(1);

        //Get their widths
        SpriteRenderer sr = background1.GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;

        //Set backgrounds next to eachother
        background2.position = new Vector3(background1.position.x + spriteWidth, 
                                           background1.position.y, background1.position.z);
    }

    void Update()
    {
        //Shift background left
        float move = speed * Time.deltaTime;
        background1.Translate(-move, 0, 0);
        background2.Translate(-move, 0, 0);

        //If the first background has moved its full width, teleport all the way to the right
        if (background1.position.x <= -spriteWidth)
        {
            background1.position = new Vector3(background2.position.x + spriteWidth, 
                                               background1.position.y, background1.position.z);
            SwapBackgrounds();
        }

        if (background2.position.x <= -spriteWidth)
        {
            background2.position = new Vector3(background1.position.x + spriteWidth, 
                                               background2.position.y, background2.position.z);
            SwapBackgrounds();
        }
    }

    // Swap backgrounds
    void SwapBackgrounds()
    {
        Transform temp = background1;
        background1 = background2;
        background2 = temp;
    }
}
