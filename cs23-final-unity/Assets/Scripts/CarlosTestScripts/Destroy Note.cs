using UnityEngine;

public class Destroy_Note : MonoBehaviour
{
    
    public void callDestruction(double seconds)
    {
        Destroy(gameObject, (float)seconds);
    }

}
