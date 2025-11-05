using System.Collections;
using UnityEngine;

public class TEST_LEADER_MANAGER : MonoBehaviour
{
    public GameObject playerCue;

    [Header("Direction GameObjects")]
    public GameObject idleSprite;
    public GameObject arrowUp;
    public GameObject arrowDown;
    public GameObject arrowLeft;
    public GameObject arrowRight;

    void Start()
    {
        playerCue.SetActive(false);
        ShowIdle();
    }

    public int[] StartSequence(float bpm, int beats)
    {
        int[] seq = getSequence(beats);
        StartCoroutine(PlaySequence(seq, beats, bpm));
        return seq;
    }

    IEnumerator PlaySequence(int[] seq, int beats, float bpm)
    {
        const float FLASH_DURATION = 0.2f;
        float beatDuration = 60f / bpm;

        // Play each of the beats
        for (int beatIndex = 0; beatIndex < beats; beatIndex++)
        {
            // Show the arrow for this beat
            ShowDirection(seq[beatIndex]);

            // Wait for flash duration
            yield return new WaitForSeconds(FLASH_DURATION);

            // Show idle during the gap between beats
            ShowIdle();

            // Wait for the rest of the beat
            yield return new WaitForSeconds(beatDuration - FLASH_DURATION);
        }
        
        playerCue.SetActive(true);
    }

    void ShowDirection(int direction)
    {
        // Hide idle and all arrows
        if (idleSprite != null) idleSprite.SetActive(false);
        if (arrowUp != null) arrowUp.SetActive(false);
        if (arrowDown != null) arrowDown.SetActive(false);
        if (arrowLeft != null) arrowLeft.SetActive(false);
        if (arrowRight != null) arrowRight.SetActive(false);
        
        GameObject directionToShow = null;
        string directionName = "";
        
        switch (direction)
        {
            case 0: // UP
                directionToShow = arrowUp;
                directionName = "UP";
                break;
            case 1: // DOWN
                directionToShow = arrowDown;
                directionName = "DOWN";
                break;
            case 2: // LEFT
                directionToShow = arrowLeft;
                directionName = "LEFT";
                break;
            case 3: // RIGHT
                directionToShow = arrowRight;
                directionName = "RIGHT";
                break;
        }
        
        if (directionToShow != null)
        {
            directionToShow.SetActive(true);
            Debug.Log($"[{Time.time:F2}] Leader: Showing {directionName}");
        }
    }

    void ShowIdle()
    {
        // Hide ONLY the directional arrows, not the idle sprite
        if (arrowUp != null) arrowUp.SetActive(false);
        if (arrowDown != null) arrowDown.SetActive(false);
        if (arrowLeft != null) arrowLeft.SetActive(false);
        if (arrowRight != null) arrowRight.SetActive(false);
        
        // Show idle (only if not already active)
        if (idleSprite != null && !idleSprite.activeSelf)
        {
            idleSprite.SetActive(true);
        }
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

