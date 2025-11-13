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
    public GameObject losePanel;
    public GameObject endlessGameOverPanel;
    public GameObject nextLevelPanel; // reference the panel GameObject directly


    [Header("Win/Lose Panel UI")]
    public TMP_Text winText;
    public TMP_Text loseText;
    public TMP_Text coinsEarnedText;
    public TMP_Text endlessGameOverText;

    [Header("Score UI")]
    public TMP_Text playerScoreText;
    public TMP_Text aiScoreText;

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
        Debug.Log($"[UIManager] ShowNextLevelPanelForCurrentLevel called. GameSession.NextLevelIndex: {GameSession.GetNextLevelIndex()}");

        if (LevelManager.Instance == null) return;

        int nextIndex = GameSession.GetNextLevelIndex();
        if (nextIndex >= LevelManager.Instance.levelDataList.Count)
        {
            Debug.Log("[UIManager] No more levels. Going to main menu.");
            GoToMainMenu();
            return;
        }

        LevelData nextLevel = LevelManager.Instance.levelDataList[nextIndex];
        Debug.Log($"[UIManager] Preparing next level: {nextLevel.levelName} (Index: {nextIndex})");

        if (nextLevelPanel != null)
        {
            nextLevelPanel.SetActive(true);

            var panelComp = nextLevelPanel.GetComponent<NextLevelPanel>();
            if (panelComp == null)
                panelComp = nextLevelPanel.GetComponentInChildren<NextLevelPanel>();

            if (panelComp != null)
            {
                panelComp.Setup(nextLevel);
                winPanel.SetActive(false);
            }
            else
            {
                Debug.LogWarning("[UIManager] NextLevelPanel component missing!");
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
        if (endlessGameOverPanel == null) return;

        endlessGameOverPanel.SetActive(true);

        if (endlessGameOverText != null)
        {
            endlessGameOverText.text = winner == PlayerType.Player ? "New High Score!" : "Game Over! AI Wins.";
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

    

    public void ReplayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void GoToMainMenu() => SceneManager.LoadScene("MainMenu");
}
