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
    public TMP_Text coinsEarnedText;

    [Header("Score UI")]
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;

    [Header("Economy")]
    public TMP_Text coinText; //  Shows current total coins on screen (Main Menu / HUD)
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
        //  Ensure subscription AFTER CoinManager is ready
        StartCoroutine(InitializeCoinUI());
        
        Debug.Log("[UIManager] Loaded. Scene: " + SceneManager.GetActiveScene().name);
        StartCoroutine(InitializeCoinUI());
     }

private IEnumerator InitializeCoinUI()
    {
        // Small delay to let CoinManager initialize first
        yield return new WaitForSeconds(0.1f);

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged -= UpdateCoinText; // prevent duplicates
            CoinManager.Instance.OnCoinsChanged += UpdateCoinText;

            //  Force coin text to refresh immediately
            UpdateCoinText(CoinManager.Instance.GetCoins());
        }
    }


    // Updates the coin UI text whenever coins change
    public void UpdateCoinText(int currentCoins)
    {
        if (coinText != null)
            coinText.text =  "Coins: " +  currentCoins.ToString();
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
        string coinsText = "";

        if (mode == MatchSettings.GameMode.PlayerVsBot)
        {
            if (winner == PlayerType.AI)
            {
                // AI won
                winnerLabel = "You Lose";
                loserLabel = "AI Wins!";

                if (CoinManager.Instance != null)
                {
                    int current = CoinManager.Instance.GetCoins();
                    int taken = Mathf.Min(aiWinPenalty, current);
                    CoinManager.Instance.SpendCoins(taken);
                    coinsText = $"Lost: {taken}\nTotal Coins: {CoinManager.Instance.GetCoins()}";
                }
            }
            else
            {
                // Player won
                winnerLabel = "You Win!";
                loserLabel = "AI";

                if (CoinManager.Instance != null)
                {
                    CoinManager.Instance.AddCoins(playerWinReward);
                    coinsText = $"Reward: {playerWinReward}\nTotal Coins: {CoinManager.Instance.GetCoins()}";
                }
            }
        }
        else if (mode == MatchSettings.GameMode.PlayerVsPlayer)
        {
            // Multiplayer
            winnerLabel = winner.ToString() + " Wins!";
            loserLabel = loser.ToString();
            coinsText = ""; // no rewards in multiplayer
        }

        if (winText != null) winText.text = winnerLabel;
        if (loseText != null) loseText.text = loserLabel;
        if (coinsEarnedText != null) coinsEarnedText.text = coinsText;

        FindObjectOfType<TriviaManager>()?.ShowRandomQuestion();


        Debug.Log($"[UIManager] Mode: {mode}, Winner: {winner}, Loser: {loser}. {coinsText}");
    }

    public void ReplayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void GoToMainMenu() => SceneManager.LoadScene("MainMenu");
}

