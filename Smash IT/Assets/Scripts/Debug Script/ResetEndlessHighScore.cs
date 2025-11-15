using UnityEngine;

public class ResetEndlessHighScore : MonoBehaviour
{
    private const string ENDLESS_HIGHSCORE_KEY = "EndlessHighScore";

    public void ResetHighScore()
    {
        PlayerPrefs.SetInt(ENDLESS_HIGHSCORE_KEY, 0);
        PlayerPrefs.Save();
        Debug.Log("[TEST] Endless High Score reset to 0");
    }
}
