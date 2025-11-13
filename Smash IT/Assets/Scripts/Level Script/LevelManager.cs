using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Assets")]
    public List<LevelData> levelDataList;

    public int CurrentLevelIndex => GameSession.CurrentLevelIndex;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadLevel(GameSession.CurrentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        Debug.Log($"[LevelManager] LoadLevel called with index: {index}");

        if (index < 0 || index >= levelDataList.Count)
        {
            Debug.LogWarning("[LevelManager] Invalid level index!");
            return;
        }

        // Set current level first
        GameSession.SetCurrentLevel(index);

        LevelData data = levelDataList[index];
        Debug.Log($"[LevelManager] Loading {data.levelName}");

        // Configure game systems
        var ai = FindFirstObjectByType<AIPaddleMovement>();
        if (ai != null) ai.moveSpeed = data.aiSpeed;

        var ball = FindFirstObjectByType<BallMovement>();
        if (ball != null) ball.speedMultiplier = data.ballSpeed / 8f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.winningScore = ExtractWinningScoreFromGoal(data.goalDescription);
            GameManager.Instance.ResetGame();
        }

        // Advance AFTER successful load, not before
        GameSession.AdvanceLevel();
    }

    private int ExtractWinningScoreFromGoal(string goalDescription)
    {
        if (string.IsNullOrEmpty(goalDescription))
        {
            Debug.LogWarning("[LevelManager] goalDescription is empty. Using fallback 5.");
            return 5;
        }

        foreach (var part in goalDescription.Split(' '))
        {
            if (int.TryParse(part, out int score))
                return score;
        }

        Debug.LogWarning($"[LevelManager] Could not parse goal: \"{goalDescription}\". Using fallback 5.");
        return 5;
    }
}

