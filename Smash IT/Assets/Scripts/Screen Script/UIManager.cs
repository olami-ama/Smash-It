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
    public TMP_Text coinsEarnedText;   // Coins shown on Win Panel

    [Header("Score UI")]
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;

    [Header("Economy")]
    public TMP_Text mainMenuCoinText;   // Coins shown in Main Menu / HUD
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
        // Subscribe to coin updates once CoinManager is ready
        StartCoroutine(InitializeCoinUI());
        Debug.Log("[UIManager] Loaded. Scene: " + SceneManager.GetActiveScene().name);
    }

    private IEnumerator InitializeCoinUI()
    {
        yield return new WaitForSeconds(0.1f); // Wait for CoinManager init

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged -= UpdateCoinText;
            CoinManager.Instance.OnCoinsChanged += UpdateCoinText;

            // Force refresh for Main Menu display only
            UpdateCoinText(CoinManager.Instance.GetCoins());
        }
    }

    // Updates Main Menu coin display only (not Win Panel)
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
        if (winPanel != null)
            winPanel.SetActive(true);

        string winnerLabel = "";
        string loserLabel = "";
        string rewardText = "";
        int rewardEarned = 0;
        
        

        // Reward Logic
        if (mode == MatchSettings.GameMode.PlayerVsBot)
        {
            if (winner == PlayerType.AI)
            {
                winnerLabel = "You Lose";
                loserLabel = "AI Wins!";

                if (CoinManager.Instance != null)
                {
                    int penalty = Mathf.Min(aiWinPenalty, CoinManager.Instance.GetCoins());
                    CoinManager.Instance.SpendCoins(penalty);
                    rewardEarned = -penalty;
                }
            }
            else
            {
                winnerLabel = "You Win!";
                loserLabel = "AI";

                if (CoinManager.Instance != null)
                {
                    CoinManager.Instance.AddCoins(playerWinReward);
                    rewardEarned = playerWinReward;
                }
            }
        }
        else if (mode == MatchSettings.GameMode.PlayerVsPlayer)
        {
            winnerLabel = winner + " Wins!";
            loserLabel = loser.ToString();
        }

        // Update UI 
        if (winText != null)
            winText.text = winnerLabel;
        if (loseText != null)
            loseText.text = loserLabel;

        if (coinsEarnedText != null)
        {
            if (rewardEarned > 0)
                rewardText = $"{rewardEarned} Coins Earned!";
            else if (rewardEarned < 0)
                rewardText = $"{Mathf.Abs(rewardEarned)} Coins Lost!";
           

            coinsEarnedText.text = rewardText;
            Debug.Log($"[UIManager] Updated coinsEarnedText: {rewardText}");
        }

        if (rewardEarned > 0) // only play when player wins
        {
            var confetti = FindFirstObjectByType<ConfettiManager>();
            if (confetti != null)
            {
                Debug.Log("[UIManager] Calling PlayConfetti()");
                confetti.PlayConfetti();
            }
            else
            {
                Debug.LogWarning("[UIManager] ConfettiManager not found!");
            }
        }


        Debug.Log($"[UIManager] Game Ended — Winner: {winnerLabel}, Reward: {rewardEarned}");

        // Delay Trivia popup so it appears above the Win Panel
        StartCoroutine(ShowTriviaAfterDelay(1.2f));
    }

    //  Delay trivia popup for smoother flow 
    private IEnumerator ShowTriviaAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        var triviaManager = FindFirstObjectByType<TriviaManager>();
        if (triviaManager != null)
        {
            triviaManager.ShowRandomQuestion(); // appears above Win Panel
        }
        else
        {
            Debug.LogWarning("[UIManager] TriviaManager not found in scene.");
        }
    }

    public void ReplayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void GoToMainMenu() => SceneManager.LoadScene("MainMenu");
}
