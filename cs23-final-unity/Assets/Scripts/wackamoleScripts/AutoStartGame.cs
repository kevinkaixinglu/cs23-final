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
                gameManager.beat_map[i].qNotes = new QNote[4];
                for (int j = 0; j < 4; j++)
                {
                    gameManager.beat_map[i].qNotes[j] = new QNote();
                    gameManager.beat_map[i].qNotes[j].sNotes = new int[4]; // All zeros
                }
            }

            gameManager.StartGame();
        }
    }
}