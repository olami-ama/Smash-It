using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Assets")]
    public List<LevelData> levelDataList;
    private int currentLevelIndex = 0;
    public int CurrentLevelIndex => currentLevelIndex;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelDataList.Count)
        {
            Debug.LogWarning("[LevelManager] Invalid level index!");
            return;
        }

        LevelData data = levelDataList[index];
        Debug.Log($"[LevelManager] Loaded {data.levelName}");

        var ai = FindFirstObjectByType<AIPaddleMovement>();
        if (ai != null) ai.moveSpeed = data.aiSpeed;

        var ball = FindFirstObjectByType<BallMovement>();
        if (ball != null) ball.speedMultiplier = data.ballSpeed / 8f;

        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.ResetGame();
        }

    }

    // Called when player wins
    public void CompleteLevel()
    {
        Debug.Log($"[LevelManager] Level {currentLevelIndex} completed!");
        // Just show the win panel  don’t load next level yet
        UIManager.Instance.ShowWinPanel(PlayerType.Player, PlayerType.AI, MatchSettings.GameMode.LevelMode);
    }

    // Called only when "Next Level" button is pressed
    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levelDataList.Count)
        {
            Debug.Log($"[LevelManager] Loading next level: {currentLevelIndex}");
            LoadLevel(currentLevelIndex);
            UIManager.Instance.HideWinPanel();
        }
        else
        {
            Debug.Log("[LevelManager] All levels complete! Returning to main menu.");
            UIManager.Instance.GoToMainMenu();
        }
    }
}

