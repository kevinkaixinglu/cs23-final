using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class TEST_LEADER_MANAGER : MonoBehaviour
{
    //Assign all Sprites + Gameobjects
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int [] StartSequence(float bpm, int beats)
    {
        int[] seq = getSequence(beats);
        StartCoroutine(PlaySequence(seq, beats, bpm));
        return seq;
    }

    IEnumerator PlaySequence(int[] seq, int beats, float bpm)
    {
        SpriteRenderer sr;

        // Play each of the 4 random arrows
        for (int beatIndex = 0; beatIndex < beats; beatIndex++)
        {
            ResetArrows();

            const float FLASH_DURATION = 0.2f;

            // Select the arrow based on random choice
            switch (seq[beatIndex])
            {
                case 0: // UP
                    sr = arrowUp.GetComponentInChildren<SpriteRenderer>();
                    sr.sprite = arrowUpOn;
                    Debug.Log($"[{Time.time:F2}] Leader Seq0 Beat{beatIndex + 1} (UP) ON");
                    break;
                case 1: // DOWN
                    sr = arrowDown.GetComponentInChildren<SpriteRenderer>();
                    sr.sprite = arrowDownOn;
                    Debug.Log($"[{Time.time:F2}] Leader Seq0 Beat{beatIndex + 1} (DOWN) ON");
                    break;
                case 2: // LEFT
                    sr = arrowLeft.GetComponentInChildren<SpriteRenderer>();
                    sr.sprite = arrowLeftOn;
                    Debug.Log($"[{Time.time:F2}] Leader Seq0 Beat{beatIndex + 1} (LEFT) ON");
                    break;
                case 3: // RIGHT
                    sr = arrowRight.GetComponentInChildren<SpriteRenderer>();
                    sr.sprite = arrowRightOn;
                    Debug.Log($"[{Time.time:F2}] Leader Seq0 Beat{beatIndex + 1} (RIGHT) ON");
                    break;
            }

            // Wait for flash duration
            yield return new WaitForSeconds(FLASH_DURATION);

            // Turn off the arrow to create visible gap
            ResetArrows();

            yield return new WaitForSeconds(60 / bpm - FLASH_DURATION);
        }
    }

    void ResetArrows()
    {
        arrowUp.GetComponentInChildren<SpriteRenderer>().sprite = arrowUpOff;
        arrowDown.GetComponentInChildren<SpriteRenderer>().sprite = arrowDownOff;
        arrowLeft.GetComponentInChildren<SpriteRenderer>().sprite = arrowLeftOff;
        arrowRight.GetComponentInChildren<SpriteRenderer>().sprite = arrowRightOff;
    }

    public int[] getSequence(int beats)
    {
        int[] arr = new int[beats];
        for (int j = 0; j < beats; j++)
        {
            arr[j] = Random.Range(0, 4);
        }
        return arr;
    }

}
