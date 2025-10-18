using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TriviaManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject triviaPanel;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public Button optionAButton;
    public Button optionBButton;
    public Button optionCButton;
    public Button nextButton;

    [System.Serializable]
    public class TriviaQuestion
    {
        public string question;
        public string[] options; // 3 options
        public int correctIndex; // 0, 1, or 2
    }

    [Header("Trivia Data")]
    public List<TriviaQuestion> triviaQuestions = new List<TriviaQuestion>();

    private int currentQuestionIndex;

    void Start()
    {
        triviaPanel.SetActive(false);
        nextButton.onClick.AddListener(() => triviaPanel.SetActive(false));
    }

    // Called after a match ends
    public void ShowRandomQuestion()
    {
        if (triviaQuestions.Count == 0)
        {
            Debug.LogWarning("[TriviaManager] No questions available!");
            return;
        }

        currentQuestionIndex = Random.Range(0, triviaQuestions.Count);
        TriviaQuestion q = triviaQuestions[currentQuestionIndex];

        questionText.text = q.question;
        optionAButton.GetComponentInChildren<TMP_Text>().text = q.options[0];
        optionBButton.GetComponentInChildren<TMP_Text>().text = q.options[1];
        optionCButton.GetComponentInChildren<TMP_Text>().text = q.options[2];

        feedbackText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        triviaPanel.SetActive(true);

        // Assign button actions
        optionAButton.onClick.RemoveAllListeners();
        optionBButton.onClick.RemoveAllListeners();
        optionCButton.onClick.RemoveAllListeners();

        optionAButton.onClick.AddListener(() => CheckAnswer(0));
        optionBButton.onClick.AddListener(() => CheckAnswer(1));
        optionCButton.onClick.AddListener(() => CheckAnswer(2));
    }

    void CheckAnswer(int selectedIndex)
    {
        TriviaQuestion q = triviaQuestions[currentQuestionIndex];
        bool isCorrect = selectedIndex == q.correctIndex;

        feedbackText.gameObject.SetActive(true);
        feedbackText.text = isCorrect ? "Correct! +50 Coins" : " Wrong Answer!";
        feedbackText.color = isCorrect ? Color.green : Color.red;

        if (isCorrect && CoinManager.Instance != null)
            CoinManager.Instance.AddCoins(50);

        nextButton.gameObject.SetActive(true);
    }
}
