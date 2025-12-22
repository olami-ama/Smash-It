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

    [Header("Scoring")]
    public int pointsPerGoal = 5;
    public int maxMisses = 5;

    [Header("Coin Milestones")]
    public int coinMilestonePoints = 40;
    public int coinsPerMilestone = 5;
    private int lastCoinMilestone = 0;
    private int coinsEarnedThisRun = 0;


    [Header("Ball Speed Scaling")]
    public int speedMilestonePoints = 40;
    public float speedIncreaseStep = 0.1f;
    public float maxSpeedMultiplier = 1.8f;
    private int lastSpeedMilestone = 0;

    [Header("AI Difficulty")]
    public int difficultyStep = 50;
    private int lastDifficultyIncrease = 0;

    public int playerScore;
    public int missedBalls = 0;

    private GameObject currentBall;

    private const string ENDLESS_HIGHSCORE_KEY = "EndlessHighScore";
    private const string ENDLESS_FIRST_MATCH_KEY = "EndlessFirstMatch";

    private UIManager uiManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        uiManager = UIManager.Instance;
    }

    void Start()
    {
        ResetEndlessState();
        SpawnBall();
        UpdateUI();
    }

    void ResetEndlessState()
    {
        playerScore = 0;
        missedBalls = 0;
        lastCoinMilestone = 0;
        lastSpeedMilestone = 0;
        lastDifficultyIncrease = 0;
    }

    public int GetCoinsEarnedThisRun()
    {
        return coinsEarnedThisRun;
    }

    public void PlayerScores()
    {
        playerScore += pointsPerGoal;
        UpdateUI();

        HandleCoinMilestone();
        HandleBallSpeedMilestone();
        HandleAIDifficulty();

        SpawnBall();
    }

    void HandleCoinMilestone()
    {
        if (playerScore - lastCoinMilestone >= coinMilestonePoints)
        {
            CoinManager.Instance?.AddCoins(coinsPerMilestone);
            coinsEarnedThisRun += coinsPerMilestone;

        }
    }

    void HandleBallSpeedMilestone()
    {
        if (playerScore - lastSpeedMilestone >= speedMilestonePoints)
        {
            if (currentBall != null)
            {
                BallMovement ball = currentBall.GetComponent<BallMovement>();
                if (ball != null)
                {
                    ball.speedMultiplier = Mathf.Min(
                        ball.speedMultiplier + speedIncreaseStep,
                        maxSpeedMultiplier
                    );
                }
            }

            lastSpeedMilestone = playerScore;
        }
    }

    void HandleAIDifficulty()
    {
        if (playerScore - lastDifficultyIncrease >= difficultyStep)
        {
            IncreaseAIDifficulty();
            lastDifficultyIncrease = playerScore;
        }
    }

    void IncreaseAIDifficulty()
    {
        if (aiPaddle == null) return;

        AIPaddleMovement ai = aiPaddle.GetComponent<AIPaddleMovement>();
        if (ai != null)
        {
            ai.moveSpeed += 1f;
            ai.smoothness = Mathf.Max(1f, ai.smoothness - 0.2f);
        }
    }

    public void RegisterMiss()
    {
        missedBalls++;

        if (currentBall != null)
            Destroy(currentBall);

        if (missedBalls >= maxMisses)
            EndGame();
        else
            SpawnBall();
    }

    void SpawnBall()
    {
        if (ballPrefab == null || playerPaddle == null) return;

        currentBall = Instantiate(
            ballPrefab,
            new Vector3(playerPaddle.position.x, -7.1f, 1f),
            Quaternion.identity
        );
    }

    void EndGame()
    {
        bool firstMatch = PlayerPrefs.GetInt(ENDLESS_FIRST_MATCH_KEY, 1) == 1;

        if (firstMatch)
        {
            CoinManager.Instance?.AddCoins(100);
            coinsEarnedThisRun += 100;

        }
        else
        {
            int highScore = PlayerPrefs.GetInt(ENDLESS_HIGHSCORE_KEY, 0);

            if (playerScore > highScore)
            {
                int bonusCoins = playerScore - highScore;

                CoinManager.Instance?.AddCoins(bonusCoins);
                coinsEarnedThisRun += bonusCoins;

                PlayerPrefs.SetInt(ENDLESS_HIGHSCORE_KEY, playerScore);
                PlayerPrefs.Save();
            }

        }

        StartCoroutine(ShowEndlessPanelWithDelay());
    }

    IEnumerator ShowEndlessPanelWithDelay()
    {
        yield return new WaitForSeconds(0.3f);
        uiManager.ShowOnlyEndlessGamePanel();
        uiManager.ShowEndlessGameOver(PlayerType.Player);
    }

    void UpdateUI()
    {
        UIManager.Instance.UpdateScoreUI(playerScore, 0);
    }

    public void ReplayGame()
    {
        ResetEndlessState();
        SpawnBall();
        UpdateUI();
    }

    public void HomeButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
