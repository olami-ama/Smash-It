using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using static GameManager; // gives access to PlayerType

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;


    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject endlessGameOverPanel;
    public GameObject nextLevelPanel; // reference the panel GameObject directly


    [Header("Win/Lose Panel UI")]
    public TMP_Text winText;
    public TMP_Text loseText;
    public TMP_Text coinsEarnedText;

    [Header("Endless Mode UI")]
    public TMP_Text endlessHighScoreText; // Show previous high score
    public TMP_Text endlessCoinsEarnedText; // Show coins earned for new high 
    public TMP_Text endlessCurrentScoreText;


    [Header("Score UI")]
    public TMP_Text playerScoreText;
    public TMP_Text aiScoreText;
    public TMP_Text endlessScoreText;

    [Header("Economy")]
    public TMP_Text mainMenuCoinText;
    public int playerWinReward = 100;
    public int aiWinPenalty = 50;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (endlessGameOverPanel != null) endlessGameOverPanel.SetActive(false);
        if (nextLevelPanel != null) nextLevelPanel.gameObject.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(DelayedStart());
        Debug.Log("CoinsEarnedText Assigned? " + (endlessCoinsEarnedText != null));
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(InitializeCoinUI());
        Debug.Log("[UIManager] Loaded. Scene: " + SceneManager.GetActiveScene().name);
    }

    private IEnumerator InitializeCoinUI()
    {
        yield return new WaitForSeconds(0.2f);

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged -= UpdateCoinText;
            CoinManager.Instance.OnCoinsChanged += UpdateCoinText;
            UpdateCoinText(CoinManager.Instance.GetCoins());
        }
        else
        {
            Debug.LogWarning("[UIManager] CoinManager not found — retrying...");
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(InitializeCoinUI());
        }
    }

    public void UpdateCoinText(int currentCoins)
    {
        if (mainMenuCoinText != null)
            mainMenuCoinText.text = "Coins: " + currentCoins;
    }

    public void UpdateScoreUI(int playerScore, int aiScore)
    {
        if (playerScoreText != null)
            playerScoreText.text = "Player: " + playerScore;

        if (aiScoreText != null)
            aiScoreText.text = "AI: " + aiScore;

        if (endlessScoreText != null)
            endlessScoreText.text = "SCORE: " + playerScore;


    }

    // -------------------
    // LEVEL MODE: Win
    // -------------------
    public void ShowWinPanel(PlayerType winner, PlayerType loser, MatchSettings.GameMode mode)
    {
        if (winPanel == null) return;

        winPanel.SetActive(true);
        string winnerLabel = "";
        int rewardEarned = 0;

        if (mode == MatchSettings.GameMode.LevelMode)
        {
            if (LevelManager.Instance != null && LevelManager.Instance.levelDataList.Count > 0)
            {
                //  Always use CURRENT level index (not next)
                int index = GameSession.CurrentLevelIndex;
                LevelData currentLevel = LevelManager.Instance.levelDataList[index];

                if (winner == PlayerType.Player)
                {
                    winnerLabel = $"{currentLevel.levelName} Complete!";
                    rewardEarned = currentLevel.coinsReward;
                    CoinManager.Instance?.AddCoins(rewardEarned);
                }
                else
                {
                    ShowLosePanel();
                    return;
                }
            }
        }

        if (winText != null) winText.text = winnerLabel;
        if (coinsEarnedText != null) coinsEarnedText.text = rewardEarned > 0 ? $"+{rewardEarned} Coins Earned!" : "";

        var confetti = FindFirstObjectByType<ConfettiManager>();
        confetti?.PlayConfetti();

        // Only show trivia in Endless Mode
        if (mode == MatchSettings.GameMode.EndlessMode)
        {
            StartCoroutine(ShowTriviaAfterDelay(7f));
        }


        Debug.Log($"[UIManager] LevelMode Win Panel shown for Level {GameSession.CurrentLevelIndex + 1}, Reward: {rewardEarned}");
    }

    // -------------------
    // Show NextLevelPanel
    // -------------------
    public void ShowNextLevelPanelForCurrentLevel()
    {
        if (LevelManager.Instance == null) return;

        // Only show if CurrentLevelIndex is not the first level
        if (GameSession.CurrentLevelIndex < 0) return;

        int nextIndex = GameSession.CurrentLevelIndex + 1;

        if (nextIndex >= LevelManager.Instance.levelDataList.Count)
        {
            GoToMainMenu();
            return;
        }

        LevelData nextLevel = LevelManager.Instance.levelDataList[nextIndex];

        if (nextLevelPanel != null)
        {
            nextLevelPanel.SetActive(true);

            var panelComp = nextLevelPanel.GetComponent<NextLevelPanel>();
            if (panelComp == null)
                panelComp = nextLevelPanel.GetComponentInChildren<NextLevelPanel>();

            if (panelComp != null)
            {
                panelComp.Setup(nextLevel);

                // Hide win panel here
                if (winPanel != null)
                    winPanel.SetActive(false);
            }
        }
    }





    public void HideWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    // -------------------
    // LEVEL MODE: Lose
    // -------------------
    public void ShowLosePanel()
    {
        if (losePanel == null) return;

        var confetti = FindFirstObjectByType<ConfettiManager>();
        confetti?.StopConfetti();

        losePanel.SetActive(true);

        CanvasGroup cg = losePanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = losePanel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        StartCoroutine(FadeInLosePanel(cg));

        if (loseText != null)
            loseText.text = "You Lost! Try Again?";

        Debug.Log("[UIManager] Lose Panel shown (Level Mode).");
    }

    private IEnumerator FadeInLosePanel(CanvasGroup cg)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    // -------------------
    // ENDLESS MODE
    // -------------------
    public void ShowEndlessGameOver(PlayerType winner)
    {
        if (EndlessGameManager.Instance == null)
        {
            Debug.LogWarning("[UIManager] EndlessGameManager not found!");
            return;
        }

        // Get the player's score from EndlessGameManager
        int playerScore = EndlessGameManager.Instance.GetPlayerScore();

        // Get previous high score
        int previousHighScore = PlayerPrefs.GetInt("EndlessHighScore", 0);

        Debug.Log("[Endless] Player Score: " + playerScore);
        Debug.Log("[Endless] High Score Before: " + previousHighScore);

        // Set high score text
        if (endlessHighScoreText != null)
            endlessHighScoreText.text = "High Score: " + previousHighScore;

        // Set current score text
        if (endlessCurrentScoreText != null)
            endlessCurrentScoreText.text = "Your Score: " + playerScore;

        // Coins earned (only IF beat high score)
        int coinsEarned = 0;
        if (playerScore > previousHighScore)
        {
            coinsEarned = playerScore - previousHighScore;
            if (endlessCoinsEarnedText != null)
                endlessCoinsEarnedText.text = "+" + coinsEarned + " Coins!";
        }
        else
        {
            if (endlessCoinsEarnedText != null)
                endlessCoinsEarnedText.text = "";
        }

        // Show only endless game panel
        ShowOnlyEndlessGamePanel();

        // Start trivia after delay
        StartCoroutine(ShowTriviaAfterDelay(2f));
    }

    // -------------------
    // CLASS-LEVEL METHODS
    // -------------------
  public  void ShowOnlyEndlessGamePanel()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (nextLevelPanel != null) nextLevelPanel.SetActive(false);
        if (endlessGameOverPanel != null) endlessGameOverPanel.SetActive(true);

        Debug.Log("[UIManager] Endless Game Over Panel shown.");
    }

    IEnumerator ShowTriviaAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        var triviaManager = FindFirstObjectByType<TriviaManager>();
        if (triviaManager != null)
        {
            var confetti = FindFirstObjectByType<ConfettiManager>();
            confetti?.StopConfetti();

            triviaManager.ShowRandomQuestion();
        }
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}