using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject winPanel;

    [Header("Win Panel UI")]
    public TMP_Text winText;
    public TMP_Text loseText;

    [Header("Score UI")]
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        winPanel.SetActive(false);
    }

    public void UpdateScoreUI(int p1, int p2)
    {
        player1ScoreText.text = "Player 1: " + p1;
        player2ScoreText.text = "Player 2: " + p2;
    }

    public void ShowWinPanel(string winner, string loser)
    {
        winPanel.SetActive(true);
        winText.text = winner;
        loseText.text = loser;
    }

    // Called by WinPanel buttons
    public void ReplayGame()
    {
        StartCoroutine(UIFader.Instance.FadeAndLoadScene(SceneManager.GetActiveScene().name));
    }

    public void GoToMainMenu()
    {
        StartCoroutine(UIFader.Instance.FadeAndLoadScene("MainMenu"));
    }
}
