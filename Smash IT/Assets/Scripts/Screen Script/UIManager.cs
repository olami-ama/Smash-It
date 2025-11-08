using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;             // For Level Mode loses
    public GameObject endlessGameOverPanel;  // For Endless Mode losses

    [Header("Win/Lose Panel UI")]
    public TMP_Text winText;
    public TMP_Text loseText;
    public TMP_Text coinsEarnedText; // Coins shown on Win Panel
    public TMP_Text endlessGameOverText;

    [Header("Score UI")]
    public TMP_Text playerScoreText;
    public TMP_Text aiScoreText;

    [Header("Economy")]
    public TMP_Text mainMenuCoinText; // Coins shown in Main Menu / HUD
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
    }


    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(InitializeCoinUI());
        Debug.Log("[UIManager] Loaded. Scene: " + SceneManager.GetActiveScene().name);
    }


    private IEnumerator InitializeCoinUI()
    {
        yield return new WaitForSeconds(0.2f); // give CoinManager time to spawn

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
    }

    //  LEVEL MODE: When player wins
    //  LEVEL MODE: When player wins
    public void ShowWinPanel(PlayerType winner, PlayerType loser, MatchSettings.GameMode mode)
    {
        if (winPanel == null) return;

        winPanel.SetActive(true);
        string winnerLabel = "";
        int rewardEarned = 0;

        // Handle only for LevelMode
        if (mode == MatchSettings.GameMode.LevelMode)
        {
            // Get the current level data
            LevelData currentLevel = null;
            if (LevelManager.Instance != null && LevelManager.Instance.levelDataList.Count > 0)
            {
                int index = Mathf.Clamp(LevelManager.Instance.CurrentLevelIndex, 0, LevelManager.Instance.levelDataList.Count - 1);
                currentLevel = LevelManager.Instance.levelDataList[index];
            }

            // If player wins
            if (winner == PlayerType.Player)
            {
                if (currentLevel != null)
                {
                    winnerLabel = $"{currentLevel.levelName} Complete!";
                    rewardEarned = currentLevel.coinsReward;
                    CoinManager.Instance?.AddCoins(rewardEarned);
                }
                else
                {
                    winnerLabel = "Level Complete!";
                    rewardEarned = 100; // fallback
                    CoinManager.Instance?.AddCoins(rewardEarned);
                }
            }
            else
            {
                // If AI wins (just show lose panel separately)
                ShowLosePanel();
                return;
            }
        }

        // Update text
        if (winText) winText.text = winnerLabel;

        if (coinsEarnedText != null)
        {
            string rewardText = rewardEarned > 0
                ? $"+{rewardEarned} Coins Earned!"
                : "";
            coinsEarnedText.text = rewardText;
        }

        // Confetti on win
        var confetti = FindFirstObjectByType<ConfettiManager>();
        confetti?.PlayConfetti();

        // Trivia popup (optional)
        StartCoroutine(ShowTriviaAfterDelay(7f));

        Debug.Log($"[UIManager] LevelMode Win Panel shown. Winner: {winner}, Reward: {rewardEarned}");
    }


    //  LEVEL MODE: When player loses
    public void ShowLosePanel()
    {
        if (losePanel == null) return;

        // Stop any celebration effects
        var confetti = FindFirstObjectByType<ConfettiManager>();
        confetti?.StopConfetti();

        // Enable the panel and start fade-in
        losePanel.SetActive(true);

        // Ensure there’s a CanvasGroup for fade animation
        CanvasGroup cg = losePanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = losePanel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        StartCoroutine(FadeInLosePanel(cg));

        // Update lose text
        if (loseText != null)
            loseText.text = "You Lost! Try Again?";

        Debug.Log("[UIManager] Lose Panel shown (Level Mode).");
    }

    private IEnumerator FadeInLosePanel(CanvasGroup cg)
    {
        float duration = 0.5f; // half a second fade
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }


    // ENDLESS MODE: When player loses
    public void ShowEndlessGameOver(PlayerType winner)
    {
        if (endlessGameOverPanel == null) return;

        endlessGameOverPanel.SetActive(true);

        if (endlessGameOverText != null)
        {
            if (winner == PlayerType.Player)
                endlessGameOverText.text = "New High Score!";
            else
                endlessGameOverText.text = "Game Over! AI Wins.";
        }

        var confetti = FindFirstObjectByType<ConfettiManager>();
        confetti?.StopConfetti();

        Debug.Log("[UIManager] Endless Mode Game Over panel shown.");
    }

    private IEnumerator ShowTriviaAfterDelay(float delay)
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
    public void GoToNextLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogWarning("[UIManager] LevelManager not found. Make sure it's in the scene!");
        }
    }
    public void HideWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }


    public void ReplayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void GoToMainMenu() => SceneManager.LoadScene("MainMenu");
}
