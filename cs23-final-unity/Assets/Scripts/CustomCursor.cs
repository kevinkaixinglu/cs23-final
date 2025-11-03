using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    void Start()
    {
        // Set the custom cursor
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    void OnDisable()
    {
        // Reset to default cursor when disabled
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}