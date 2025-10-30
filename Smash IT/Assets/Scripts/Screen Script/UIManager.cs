using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject winPanel;

    [Header("Win Panel UI")]
    public TMP_Text winText;
    public TMP_Text loseText;
    public TMP_Text coinsEarnedText; // Coins shown on Win Panel

    [Header("Score UI")]
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;

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

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    private void Start()
    {
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

    public void UpdateScoreUI(int p1, int p2)
    {
        if (player1ScoreText != null)
            player1ScoreText.text = "Player 1: " + p1;

        if (player2ScoreText != null)
            player2ScoreText.text = "Player 2: " + p2;
    }

    public void ShowWinPanel(PlayerType winner, PlayerType loser, MatchSettings.GameMode mode)
    {
        // Retry logic if CoinManager not ready yet
        if (CoinManager.Instance == null)
        {
            Debug.LogWarning("[UIManager] CoinManager not found yet — retrying ShowWinPanel...");
            StartCoroutine(RetryShowWinPanel(winner, loser, mode));
            return;
        }

        if (winPanel != null)
            winPanel.SetActive(true);

        string winnerLabel = "";
        string loserLabel = "";
        string rewardText = "";
        int rewardEarned = 0;

        //  REWARD LOGIC
        if (mode == MatchSettings.GameMode.PlayerVsBot)
        {
            if (winner == PlayerType.AI)
            {
                winnerLabel = "You Lose";
                loserLabel = "AI Wins!";

                int penalty = Mathf.Min(aiWinPenalty, CoinManager.Instance.GetCoins());
                CoinManager.Instance.SpendCoins(penalty);
                rewardEarned = -penalty;
            }
            else
            {
                winnerLabel = "You Win!";
                loserLabel = "AI";

                CoinManager.Instance.AddCoins(playerWinReward);
                rewardEarned = playerWinReward;
            }
        }
        else if (mode == MatchSettings.GameMode.PlayerVsPlayer)
        {
            winnerLabel = winner + " Wins!";
            loserLabel = loser.ToString();
        }

        // UPDATE UI TEXTS
        if (winText) winText.text = winnerLabel;
        if (loseText) loseText.text = loserLabel;

        if (coinsEarnedText != null)
        {
            if (rewardEarned > 0)
                rewardText = $"+{rewardEarned} Coins Earned!";
            else if (rewardEarned < 0)
                rewardText = $"-{Mathf.Abs(rewardEarned)} Coins Lost!";
            else
                rewardText = "";

            coinsEarnedText.text = rewardText;
            Debug.Log($"[UIManager] Updated coinsEarnedText: {rewardText}");
        }

        //  CONFETTI EFFECT 
        if (rewardEarned > 0)
        {
            var confetti = FindFirstObjectByType<ConfettiManager>();
            if (confetti != null)
            {
                confetti.PlayConfetti();
                Debug.Log("[UIManager] Confetti played!");
            }
        }

        Debug.Log($"[UIManager] Game Ended — Winner: {winnerLabel}, Reward: {rewardEarned}");

        //  TRIVIA POPUP (after 7s) 
        StartCoroutine(ShowTriviaAfterDelay(7f));
    }

    private IEnumerator RetryShowWinPanel(PlayerType winner, PlayerType loser, MatchSettings.GameMode mode)
    {
        yield return new WaitForSeconds(0.3f);
        if (CoinManager.Instance != null)
            ShowWinPanel(winner, loser, mode);
    }

    private IEnumerator ShowTriviaAfterDelay(float delay)
    {
        Debug.Log("[UIManager] Waiting before showing trivia...");
        yield return new WaitForSeconds(delay);

        var triviaManager = FindFirstObjectByType<TriviaManager>();
        if (triviaManager != null)
        {
            // Stop confetti before showing trivia
            var confetti = FindFirstObjectByType<ConfettiManager>();
            if (confetti != null)
                confetti.StopConfetti();

            triviaManager.ShowRandomQuestion();
            Debug.Log("[UIManager] Trivia panel shown after delay.");
        }
        else
        {
            Debug.LogWarning("[UIManager] TriviaManager not found in scene.");
        }
    }

    public void ReplayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void GoToMainMenu() => SceneManager.LoadScene("MainMenu");
}
