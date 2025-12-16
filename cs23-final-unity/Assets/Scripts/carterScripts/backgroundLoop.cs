using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [SerializeField] public float speed;

    private Transform background1;
    private Transform background2;

    private Transform cloud1;
    private Transform cloud2;

    private Transform cloud11;
    private Transform cloud12;
    private Transform cloud13;
    private Transform cloud14;

    private float spriteWidth;
    private float cloudWidth;

    void Start()
    {
        //Set backgrounds
        background1 = transform.GetChild(0);
        background2 = transform.GetChild(1);

        cloud1 = transform.GetChild(2);
        cloud2 = transform.GetChild(3);



        //Get their widths
        SpriteRenderer sr = background1.GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;

        SpriteRenderer cloudSR = cloud1.GetComponent<SpriteRenderer>();
        cloudWidth = cloudSR.bounds.size.x;

        cloud2.position = new Vector3(cloud1.position.x + cloudWidth, 
                                      cloud1.position.y, cloud1.position.z);
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

        // Shift clouds left (using cloudSpeed)
        float cloudMove = speed * Time.deltaTime;
        cloud1.Translate(-cloudMove, 0, 0);
        cloud2.Translate(-cloudMove, 0, 0);

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

        if (cloud1.position.x <= -cloudWidth)
        {
            cloud1.position = new Vector3(cloud2.position.x + cloudWidth, 
                                          cloud1.position.y, cloud1.position.z);
            SwapClouds();
        }

        // If cloud2 has moved its full width, teleport it to the right of cloud1
        if (cloud2.position.x <= -cloudWidth)
        {
            cloud2.position = new Vector3(cloud1.position.x + cloudWidth, 
                                          cloud2.position.y, cloud2.position.z);
            SwapClouds();
        }
    }

    // Swap backgrounds
    void SwapBackgrounds()
    {
        Transform temp = background1;
        background1 = background2;
        background2 = temp;
    }
    void SwapClouds()
    {
        Transform temp = cloud1;
        cloud1 = cloud2;
        cloud2 = temp;
    }
}
