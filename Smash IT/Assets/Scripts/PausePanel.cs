using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PausePanel : MonoBehaviour
{
    [Header("UI Components")]
    public CanvasGroup panelCanvasGroup;
    public Button resumeButton, homeButton, replayButton, musicButton, soundButton, closeButton;

    [Header("Audio")]
    public AudioSource musicSource;
    private bool musicOn = true;
    private bool soundOn = true;

    [Header("Fade Settings")]
    public float fadeDuration = 0.25f;

    private bool isPaused = false;

    void Start()
    {
        // Ensure the panel is visible but hidden
        panelCanvasGroup.alpha = 0;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        // Button listeners
        resumeButton.onClick.AddListener(ResumeGame);
        closeButton.onClick.AddListener(ResumeGame);
        homeButton.onClick.AddListener(GoHome);
        replayButton.onClick.AddListener(RestartLevel);
        musicButton.onClick.AddListener(ToggleMusic);
        soundButton.onClick.AddListener(ToggleSound);
    }

    // Call this from your Pause button in Level Mode
    public void ShowPause()
    {
        if (isPaused) return; // Prevent multiple opens
        isPaused = true;

        // Make sure GameObject is active (important!)
        panelCanvasGroup.gameObject.SetActive(true);

        StartCoroutine(FadePanel(1));
        Time.timeScale = 0; // freeze gameplay
    }

    void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;

        StartCoroutine(FadePanel(0));
        Time.timeScale = 1; // resume gameplay
    }

    void GoHome()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ToggleMusic()
    {
        musicOn = !musicOn;
        musicSource.mute = !musicOn;
        // Optional: Update button icon here
    }

    void ToggleSound()
    {
        soundOn = !soundOn;
        // Optional: Integrate with your SoundManager to mute/unmute SFX
    }

    IEnumerator FadePanel(float targetAlpha)
    {
        float startAlpha = panelCanvasGroup.alpha;
        float elapsed = 0f;

        // Enable interactions if fading in
        if (targetAlpha > 0)
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time because Time.timeScale = 0
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        panelCanvasGroup.alpha = targetAlpha;

        // Disable interactions if faded out
        if (targetAlpha == 0)
        {
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }
    }
}
