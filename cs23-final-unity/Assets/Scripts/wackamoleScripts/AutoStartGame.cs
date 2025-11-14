using UnityEngine;

public class AutoStartGame : MonoBehaviour
{
    private ManageGame gameManager;

    void Start()
    {
        gameManager = GetComponent<ManageGame>();
        if (gameManager != null)
        {
            // Create a minimal dummy beat map to prevent errors
            gameManager.beat_map = new Measure[100]; // Large enough for any song
            for (int i = 0; i < gameManager.beat_map.Length; i++)
            {
                gameManager.beat_map[i] = new Measure();
                gameManager.beat_map[i].beats = new Beat[4];
                for (int j = 0; j < 4; j++)
                {
                    gameManager.beat_map[i].beats[j] = new Beat();
                    gameManager.beat_map[i].beats[j].notes = new int[4]; // All zeros
                }
            }

            gameManager.StartGame();
        }
    }
}