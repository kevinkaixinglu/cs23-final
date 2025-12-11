using UnityEngine;

public class ScrollingCloud : MonoBehaviour
{
    [Header("Scrolling Settings")]
    private float baseScrollSpeed = 2f;
    private float currentScrollSpeed = 2f;
    private float screenPadding = 2f;  // Extra space beyond screen edge
    
    [Header("BPM Speed Scaling")]
    [SerializeField] private float bpmSpeedMultiplier = 0.1f; // How much speed increases per BPM
    
    private kalenGameManager gameManager;
    private float resetPositionX;
    private float disappearPositionX;
    private Camera mainCamera;
    private double lastBPM = 0;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<kalenGameManager>();
        mainCamera = Camera.main;
        
        // Calculate screen edges based on camera
        float screenHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        
        // Get cloud width (assumes sprite renderer or collider)
        float cloudWidth = 0f;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            cloudWidth = sprite.bounds.size.x;
        }
        
        // Set positions so clouds loop seamlessly
        disappearPositionX = -screenHalfWidth - cloudWidth/2;
        resetPositionX = screenHalfWidth + cloudWidth/2 + screenPadding;
        
        // Initialize scroll speed
        if (gameManager != null)
        {
            lastBPM = gameManager.bpm;
            UpdateScrollSpeed();
        }
    }
    
    void Update()
    {
        if (gameManager != null && gameManager.isPlaying)
        {
            // Check if BPM has changed
            if (gameManager.bpm != lastBPM)
            {
                lastBPM = gameManager.bpm;
                UpdateScrollSpeed();
            }
            
            transform.position += Vector3.left * currentScrollSpeed * Time.deltaTime;
            
            if (transform.position.x < disappearPositionX)
            {
                Vector3 newPos = transform.position;
                newPos.x = resetPositionX;
                transform.position = newPos;
            }
        }
    }
    
    void UpdateScrollSpeed()
    {
        // Calculate new speed based on current BPM
        currentScrollSpeed = baseScrollSpeed + ((float)lastBPM * bpmSpeedMultiplier);
        
        Debug.Log($"[ScrollingCloud] BPM changed to {lastBPM}, scroll speed updated to {currentScrollSpeed}");
    }
}