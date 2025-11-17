using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Assets")]
    public List<LevelData> levelDataList;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        // Only load the first level directly
        LoadLevel(GameSession.CurrentLevelIndex);
    }


    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelDataList.Count)
        {
            Debug.LogWarning("[LevelManager] Invalid level index!");
            return;
        }

        LevelData data = levelDataList[index];

        Debug.Log($"[LevelManager] Loading Level: {data.levelName}");

        // Configure AI and ball
        var ai = FindFirstObjectByType<AIPaddleMovement>();
        if (ai != null) ai.moveSpeed = data.aiSpeed;

        var ball = FindFirstObjectByType<BallMovement>();
        if (ball != null) ball.speedMultiplier = data.ballSpeed / 8f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.winningScore = ExtractWinningScoreFromGoal(data.goalDescription);
            GameManager.Instance.ResetGame();
        }
    }

    private int ExtractWinningScoreFromGoal(string goalDescription)
    {
        foreach (var part in goalDescription.Split(' '))
            if (int.TryParse(part, out int score)) return score;
        return 5; // fallback
    }

    // Call after player wins
    public void OnLevelCompleted()
    {
        Debug.Log($"[LevelManager] Level {GameSession.CurrentLevelIndex} completed.");

        GameSession.AdvanceLevel();

        if (GameSession.CurrentLevelIndex >= levelDataList.Count)
        {
            Debug.Log("[LevelManager] All levels completed. Returning to main menu.");
            UIManager.Instance?.GoToMainMenu();
            return;
        }

        UIManager.Instance?.ShowNextLevelPanelForCurrentLevel();
    }
}
