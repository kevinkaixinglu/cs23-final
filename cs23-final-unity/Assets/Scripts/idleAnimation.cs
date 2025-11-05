using UnityEngine;

public class TwoFrameBPMAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public Sprite frame1;
    public Sprite frame2;
    public bool switchEveryBeat = true;
    
    [Header("BPM Source")]
    public LevelManager levelManager;
    
    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private bool showingFrame1 = true;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError("TwoFrameBPMAnimator: No SpriteRenderer found on " + gameObject.name);
            return;
        }
        
        if (frame1 != null)
        {
            spriteRenderer.sprite = frame1;
        }

        if (levelManager == null)
        {
            levelManager = FindFirstObjectByType<LevelManager>();
            if (levelManager == null)
            {
                Debug.LogError("TwoFrameBPMAnimator: No LevelManager found in scene!");
            }
        }
    }

    void Update()
    {
        if (frame1 == null || frame2 == null || levelManager == null) return;

        float bpm = (float)levelManager.bpm;
        float beatDuration = 60f / bpm;
        float switchInterval = switchEveryBeat ? beatDuration : beatDuration / 2f;

        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            timer -= switchInterval;
            showingFrame1 = !showingFrame1;
            spriteRenderer.sprite = showingFrame1 ? frame1 : frame2;
        }
    }
}