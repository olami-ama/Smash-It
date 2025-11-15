using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndlessGameManager : MonoBehaviour
{
    public static EndlessGameManager Instance;

    [Header("Player/AI")]
    public Transform playerPaddle;
    public Transform aiPaddle;
    public GameObject ballPrefab;

    [Header("Endless Mode Settings")]
    public int pointsPerGoal = 5;
    public int difficultyStep = 50; // AI gets harder every X points
    public int maxMisses = 5;       // Game ends after this many misses

    private int playerScore;
    [HideInInspector] public int missedBalls = 0;
    private int lastDifficultyIncrease = 0;

    private GameObject currentBall;
    private const string ENDLESS_HIGHSCORE_KEY = "EndlessHighScore";
    private const string ENDLESS_FIRST_MATCH_KEY = "EndlessFirstMatch";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[EGM] Duplicate detected. Destroying extra copy.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    public int GetPlayerScore()
    {
        return playerScore;
    }


    void Start()
    {
        playerScore = 0;
        missedBalls = 0;
        lastDifficultyIncrease = 0;
        SpawnBall();
        UpdateUI();
    }

    public void PlayerScores()
    {
        playerScore += pointsPerGoal;
        UpdateUI();
        Debug.Log("[EGM] PlayerScores called. New Score=" + playerScore);

        // Increase AI difficulty if threshold passed
        if (playerScore - lastDifficultyIncrease >= difficultyStep)
        {
            IncreaseAIDifficulty();
            lastDifficultyIncrease = playerScore;
        }
    }

    private void IncreaseAIDifficulty()
    {
        if (aiPaddle != null)
        {
            AIPaddleMovement ai = aiPaddle.GetComponent<AIPaddleMovement>();
            if (ai != null)
            {
                ai.moveSpeed += 1f;
                ai.smoothness = Mathf.Max(1f, ai.smoothness - 0.2f);
                Debug.Log($"[Endless] AI difficulty increased! New speed: {ai.moveSpeed}");
            }
        }
    }

    public void SpawnBall()
    {
        if (ballPrefab == null || playerPaddle == null) return;

        // Destroy previous ball if exists
        if (currentBall != null) Destroy(currentBall);

        Vector3 spawnPos = new Vector3(playerPaddle.position.x, -6.1f, 0f);
        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
    }

    public void RegisterMiss()
    {
        missedBalls++;
        if (currentBall != null) Destroy(currentBall);

        if (missedBalls >= maxMisses)
        {
            EndGame();
        }
        else
        {
            SpawnBall();
        }
    }

    public void EndGame()
    {
        

        bool firstMatch = PlayerPrefs.GetInt(ENDLESS_FIRST_MATCH_KEY, 1) == 1;

        if (firstMatch)
        {
            CoinManager.Instance?.AddCoins(100);
            PlayerPrefs.SetInt(ENDLESS_FIRST_MATCH_KEY, 0);
            Debug.Log("[Endless] First match complete! 100 coins awarded.");
        }
        else
        {
            int highScore = PlayerPrefs.GetInt(ENDLESS_HIGHSCORE_KEY, 0);
            if (playerScore > highScore)
            {
                int coinsEarned = playerScore - highScore;
                CoinManager.Instance?.AddCoins(coinsEarned);
                PlayerPrefs.SetInt(ENDLESS_HIGHSCORE_KEY, playerScore);
                PlayerPrefs.Save();
                Debug.Log($"[Endless] New High Score! Coins awarded: {coinsEarned}");
            }
        }

        Debug.Log("[EGM] EndGame CALLED, PlayerScore=" + playerScore);
        Debug.Log("[EGM] Instance ID = " + GetInstanceID());


        StartCoroutine(ShowEndlessPanelWithDelay());
    
       IEnumerator ShowEndlessPanelWithDelay()
    {
        yield return new WaitForSeconds(1.5f); // let confetti finish
        UIManager.Instance.ShowEndlessGameOver(PlayerType.Player);
    }



}

private void UpdateUI()
    {
        UIManager.Instance.UpdateScoreUI(playerScore, 0); // AI score hidden
    }

    public void ReplayGame()
    {
        playerScore = 0;
        missedBalls = 0;
        lastDifficultyIncrease = 0;
        SpawnBall();
        UpdateUI();
    }

    public void HomeButton() => SceneManager.LoadScene("MainMenu");
}
