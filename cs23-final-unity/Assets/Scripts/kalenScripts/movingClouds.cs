using UnityEngine;

public class ScrollingCloud : MonoBehaviour
{
    [Header("Scrolling Settings")]
    public float scrollSpeed = 2f;
    public float screenPadding = 2f;  // Extra space beyond screen edge
    
    private kalenGameManager gameManager;
    private float resetPositionX;
    private float disappearPositionX;
    private Camera mainCamera;
    
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
    }
    
    void Update()
    {
        if (gameManager != null && gameManager.isPlaying)
        {
            transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
            
            if (transform.position.x < disappearPositionX)
            {
                Vector3 newPos = transform.position;
                newPos.x = resetPositionX;
                transform.position = newPos;
            }
        }
    }
}