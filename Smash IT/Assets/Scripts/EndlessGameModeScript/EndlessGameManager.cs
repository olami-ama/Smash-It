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
    public int difficultyStep = 50;
    public int maxMisses = 5;

    private int playerScore;
    [HideInInspector] public int missedBalls = 0;
    private int lastDifficultyIncrease = 0;

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

    public int GetPlayerScore() => playerScore;

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

        if (playerScore - lastDifficultyIncrease >= difficultyStep)
        {
            IncreaseAIDifficulty();
            lastDifficultyIncrease = playerScore;
        }
    }

    private void IncreaseAIDifficulty()
    {
        if (aiPaddle == null) return;

        AIPaddleMovement ai = aiPaddle.GetComponent<AIPaddleMovement>();
        if (ai != null)
        {
            ai.moveSpeed += 1f;
            ai.smoothness = Mathf.Max(1f, ai.smoothness - 0.2f);
        }
    }

    public void SpawnBall()
    {
        if (ballPrefab == null || playerPaddle == null) return;

        if (currentBall != null) Destroy(currentBall);

        Vector3 spawnPos = new Vector3(playerPaddle.position.x, -6.1f, 0f);
        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
    }

    public void RegisterMiss()
    {
        missedBalls++;

        if (currentBall != null) Destroy(currentBall);

        if (missedBalls >= maxMisses)
            EndGame();
        else
            SpawnBall();
    }

    public void EndGame()
    {
        bool firstMatch = PlayerPrefs.GetInt(ENDLESS_FIRST_MATCH_KEY, 1) == 1;

        if (firstMatch)
        {
            CoinManager.Instance?.AddCoins(100);
            PlayerPrefs.SetInt(ENDLESS_FIRST_MATCH_KEY, 0);
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
            }
        }

        // show UI after delay
        StartCoroutine(ShowEndlessPanelWithDelay());
    }

    private IEnumerator ShowEndlessPanelWithDelay()
    {
        yield return new WaitForSeconds(0.3f);

        uiManager.ShowOnlyEndlessGamePanel();
        uiManager.ShowEndlessGameOver(PlayerType.Player);
    }

    private void UpdateUI()
    {
        UIManager.Instance.UpdateScoreUI(playerScore, 0);
    }

    public void ReplayGame()
    {
        playerScore = 0;
        missedBalls = 0;
        lastDifficultyIncrease = 0;

        SpawnBall();
        UpdateUI();
    }

    public void HomeButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
