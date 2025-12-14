using Unity.VisualScripting;
using UnityEngine;

public class InfoPage_L1 : MonoBehaviour
{

    [Header("InfoPage Customization")]
    public bool Done = false;
    public int Required_Success = 0;
    public Vector3[] windows = null;
    public KeyCode[] key = null;

    [Header("Visual Animator")]
    public Animator visuals;

    private float lastNormalizedTime = 999f;
    private float timeSinceLastLoop = 0f;

    private bool[] waiting_for_input = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waiting_for_input = new bool[key.Length];
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        var state = visuals.GetCurrentAnimatorStateInfo(0);

        if (state.loop)
        {
            float currentNormalized = state.normalizedTime % 1f;

            // Detect loop
            if (currentNormalized < lastNormalizedTime)
            {
                Debug.Log("Animation looped");
                timeSinceLastLoop = 0f; // RESET on loop
                Reset();
            }
            else
            {
                timeSinceLastLoop += Time.deltaTime; // ACCUMULATE
            }

            UpdateWindows();

            lastNormalizedTime = currentNormalized;
        }
    }

    private void Reset()
    {
        for (int i = 0; i < key.Length; i++)
        {
            waiting_for_input[i] = false;
        }
    }
    private void UpdateWindows()
    {
        bool[] waiting_for_input_temp = new bool[waiting_for_input.Length];
        for (int i = 0; i < waiting_for_input_temp.Length; i++)
        {
            waiting_for_input_temp[i] = false;
        }

        for (int i = 0; i < windows.Length; i++)
        {
            if (timeSinceLastLoop > windows[i].x && timeSinceLastLoop < windows[i].y) // Were in window
            {
                waiting_for_input_temp[(int)windows[i].z] = true;
            }
        }
    }
}
