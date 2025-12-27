using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Assets")]
    public List<LevelData> levelDataList;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // When GameplayScene loads, DO NOT start the level.
        // Just show the confirmation panel for the selected level.
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowNextLevelPanelForCurrentLevel();
        }
    }

    // -----------------------------
    // Load and start gameplay
    // -----------------------------
    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelDataList.Count)
        {
            Debug.LogWarning("[LevelManager] Invalid level index");
            return;
        }

        LevelData data = levelDataList[index];

        Debug.Log($"[LevelManager] Starting Level {data.levelNumber}");

        var ai = FindFirstObjectByType<AIPaddleMovement>();
        if (ai != null)
            ai.moveSpeed = data.aiSpeed;

        var ball = FindFirstObjectByType<BallMovement>();
        if (ball != null)
            ball.speedMultiplier = data.ballSpeed / 8f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.winningScore = ExtractWinningScoreFromGoal(data.goalDescription);
            GameManager.Instance.ResetGame();
        }
    }

    private int ExtractWinningScoreFromGoal(string goalDescription)
    {
        foreach (var part in goalDescription.Split(' '))
        {
            if (int.TryParse(part, out int score))
                return score;
        }
        return 5;
    }

    // -----------------------------
    // Called ONLY when player wins
    // -----------------------------
    public void OnLevelCompleted()
    {
        int currentIndex = GameSession.CurrentLevelIndex;
        int nextIndex = currentIndex + 1;

        Debug.Log($"[LevelManager] Level {currentIndex} completed");

        if (nextIndex >= levelDataList.Count)
        {
            UIManager.Instance?.GoToMainMenu();
            return;
        }

        // IMPORTANT:
        // Do NOT set CurrentLevelIndex here
        // Just preview the next level
        UIManager.Instance?.ShowNextLevelPanelForCurrentLevel(nextIndex);
    }
}

