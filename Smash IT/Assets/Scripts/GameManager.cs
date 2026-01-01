using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Match Settings")]
    public MatchSettings matchSettings;
    public int winningScore = 5; // Used for Level Mode

    [Header("Ball & Paddles")]
    public GameObject ballPrefab;
    public Transform playerPaddle;
    public Transform aiPaddle;

    private GameObject currentBall;

    public int playerScore;
    public int aiScore;
    private bool isGameOver;

    public enum ServerType
    {
        Player,
        AI
    }

    private ServerType currentServer = ServerType.Player;

    private const string ENDLESS_HIGHSCORE_KEY = "EndlessHighScore";

    public int PlayerScore => playerScore;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ResetGame();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    // Called when a goal is scored
    public void PlayerScores(int scorer)
    {
        if (scorer == 1)
            playerScore++;
        else
            aiScore++;

        UIManager.Instance.UpdateScoreUI(playerScore, aiScore);

        if (matchSettings.selectedMode == MatchSettings.GameMode.LevelMode)
        {
            if (playerScore >= winningScore || aiScore >= winningScore)
            {
                EndGame();
                return;
            }
        }

        // Winner of the point serves next
        currentServer = (scorer == 1) ? ServerType.Player : ServerType.AI;
        SpawnBallForServer();
    }

    void SpawnBallForServer()
    {
        if (currentBall != null)
            Destroy(currentBall);

        currentBall = Instantiate(ballPrefab);

        BallMovement ball = currentBall.GetComponent<BallMovement>();
        if (ball == null)
        {
            Debug.LogError("[GameManager] BallMovement missing on prefab");
            return;
        }

        Transform server =
            currentServer == ServerType.Player ? playerPaddle : aiPaddle;

        ball.ServeFromPaddle(server);
    }


    public void EndGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (currentBall != null)
            Destroy(currentBall);

        PlayerType winner = (playerScore > aiScore) ? PlayerType.Player : PlayerType.AI;
        PlayerType loser = (winner == PlayerType.Player) ? PlayerType.AI : PlayerType.Player;

        if (matchSettings.selectedMode == MatchSettings.GameMode.EndlessMode)
        {
            HandleEndlessRewards();
            UIManager.Instance.ShowEndlessGameOver(winner);
        }
        else
        {
            if (winner == PlayerType.Player)
                UIManager.Instance.ShowWinPanel(winner, loser, matchSettings.selectedMode);
            else
                UIManager.Instance.ShowLosePanel();
        }
    }

    void HandleEndlessRewards()
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

    public void ResetGame()
    {
        playerScore = 0;
        aiScore = 0;
        isGameOver = false;

        currentServer = ServerType.Player; // Player always serves first

        UIManager.Instance.UpdateScoreUI(playerScore, aiScore);
        SpawnBallForServer();
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HomeButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}






/*using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score UI")]
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;
    public GameObject winPanel;
    public TextMeshProUGUI winText;

    [Header("Gameplay")]
    public int winningScore = 5;
    private int player1Score = 0;
    private int player2Score = 0;

    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform paddle1;
    public Transform paddle2;

    // optional offsets (fallback)
    private Vector2 offsetP1 = new Vector2(-1.72f, 2.12f);
    private Vector2 offsetP2 = new Vector2(0f, -2.28f);

    private string player1Label = "Player 1: ";
    private string player2Label = "Player 2: ";

    private int currentServer = 1;
    private GameObject currentBall;

    [Header("Spawn Settings (tweak in Inspector)")]
    [Tooltip("Which layers should be considered when checking spawn overlaps (set to your 'Paddles' layer).")]
    public LayerMask overlapMask = ~0; // default = everything; change in Inspector
    [Tooltip("Extra small padding added to initial spawn offset")]
    public float initialPad = 0.06f;
    [Tooltip("How many times to nudge spawn outwards to avoid paddle overlap")]
    public int maxSpawnAdjustAttempts = 12;
    [Tooltip("How far (world units) to nudge each attempt")]
    public float spawnNudgeStepMultiplier = 1.0f; // multiplied by ball radius

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    void Start()
    {
        winPanel.SetActive(false);
        SpawnBall();
        UpdateScoreUI();
    }

    public void PlayerScores(int playerNumber)
    {
        if (playerNumber == 1) player1Score++; else player2Score++;
        Debug.Log($"[GameManager] Scores -> P1:{player1Score} P2:{player2Score}");
        UpdateScoreUI();

        if (player1Score >= winningScore || player2Score >= winningScore) EndGame();
        else { SwitchServer(); SpawnBall(); }
    }

    void UpdateScoreUI()
    {
        player1ScoreText.text = player1Label + player1Score;
        player2ScoreText.text = player2Label + player2Score;
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void QuitGame() => Application.Quit();

    void EndGame()
    {
        winPanel.SetActive(true);
        winText.text = (player1Score > player2Score) ? "Player 1 Wins!" : "Player 2 Wins!";
        if (currentBall != null) Destroy(currentBall);
        Debug.Log("[GameManager] Game Ended");
    }

    void SwitchServer() => currentServer = (currentServer == 1) ? 2 : 1;

    void SpawnBall()
    {
        if (currentBall != null) Destroy(currentBall);

        // compute ball radius (fallback if prefab has no CircleCollider2D)
        float ballRadius = 0.5f;
        CircleCollider2D ballPrefabCol = ballPrefab.GetComponent<CircleCollider2D>();
        if (ballPrefabCol != null)
        {
            float scale = ballPrefab.transform.lossyScale.x;
            ballRadius = ballPrefabCol.radius * scale;
        }

        // which paddle and direction
        Transform serverPaddle = (currentServer == 1) ? paddle1 : paddle2;
        Vector3 baseDir = (currentServer == 1) ? Vector3.up : Vector3.down;

        // paddle extent Y (fallback to offsets)
        float paddleExtentY = 0f;
        Collider2D paddleCol = serverPaddle.GetComponent<Collider2D>();
        if (paddleCol != null) paddleExtentY = paddleCol.bounds.extents.y;
        else paddleExtentY = (currentServer == 1) ? Mathf.Abs(offsetP1.y) : Mathf.Abs(offsetP2.y);

        // initial spawn just outside paddle
        Vector3 spawnPos = serverPaddle.position + baseDir * (paddleExtentY + ballRadius + initialPad);

        // try to avoid paddle overlaps by nudging spawn if necessary
        int attempts = 0;
        float nudgeStep = (ballRadius * spawnNudgeStepMultiplier) + 0.05f;
        Collider2D[] overlaps;
        bool foundPaddleOverlap = false;

        do
        {
            // Overlap check using LayerMask (only checks layers selected in Inspector)
            overlaps = Physics2D.OverlapCircleAll(spawnPos, ballRadius + 0.02f, overlapMask);
            foundPaddleOverlap = false;
            foreach (var ov in overlaps)
            {
                if (ov == null) continue;
                if (ov.CompareTag("Paddle") || ov.CompareTag("Paddle2"))
                {
                    foundPaddleOverlap = true;
                    break;
                }
            }

            if (foundPaddleOverlap)
            {
                spawnPos += baseDir * nudgeStep;
                attempts++;
            }
        }
        while (foundPaddleOverlap && attempts < maxSpawnAdjustAttempts);

        if (attempts > 0)
            Debug.Log($"[GameManager] Spawn adjusted {attempts} times to avoid paddle overlap. Final spawn: {spawnPos}");

        // final overlap check but only warn if paddle(s) remain in the overlap radius
        overlaps = Physics2D.OverlapCircleAll(spawnPos, ballRadius + 0.02f, overlapMask);
        bool anyPaddleOverlap = false;
        if (overlaps.Length > 0)
        {
            string s = "[GameManager] Spawn overlaps after attempts (filtered):";
            foreach (var ov in overlaps)
            {
                if (ov == null) continue;
                if (ov.CompareTag("Paddle") || ov.CompareTag("Paddle2"))
                {
                    anyPaddleOverlap = true;
                    s += $" {ov.name}(tag:{ov.tag})";
                }
            }
            if (anyPaddleOverlap) Debug.LogWarning(s);
            else Debug.Log("[GameManager] Spawn overlaps only with non-paddle objects (ignored).");
        }

        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
    }
}

*/

