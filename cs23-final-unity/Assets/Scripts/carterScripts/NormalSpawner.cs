using UnityEngine;

public class NormalSpawner : MonoBehaviour
{
    public GameObject normalBeat;
    public float spawnRate = 0f;
    private float timer;

    bool pause = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pause == true) {
            return;
        }
        timer += Time.deltaTime;
        if (timer >= spawnRate ) {
            SpawnObject();
            timer = 0f;
        }
    }

    void SpawnObject()
    {
        Vector2 pos = transform.position;
        Instantiate(normalBeat, pos, Quaternion.identity);
    }

    public void PauseNormalSpanwer() {
        pause = true;
    }

    public void UnPauseNormalSpanwer() {
        pause = false;
    }
}
