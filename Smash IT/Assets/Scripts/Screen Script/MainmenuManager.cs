using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameModePanel;
    public GameObject settingsPanel;
    public GameObject shopPanel;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        gameModePanel.SetActive(false);
        settingsPanel.SetActive(false);
        shopPanel.SetActive(false);
    }

    public void OpenGameModeMenu()
    {
        mainMenuPanel.SetActive(false);
        gameModePanel.SetActive(true);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenShop()
    {
        mainMenuPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }

    // Start Endless Mode directly
    public void StartEndlessMode()
    {
        SceneManager.LoadScene("EndlessModeScene"); // Load your reusable game scene
        // LevelManager in GameScene will detect that Endless Mode should start
        // You can optionally set a flag in GameSession like:
        // GameSession.GameMode = MatchSettings.GameMode.EndlessMode;
    }

    // Start Level Mode → open Level Map inside GameScene
    public void StartLevelMode()
    {
        SceneManager.LoadScene("GameScene"); // Load the reusable game scene
        // GameScene should show the Level Map panel immediately
        // You can optionally set a flag like:
        // GameSession.GameMode = MatchSettings.GameMode.LevelMode;
    }

    public void BackToMainMenu()
    {
        gameModePanel.SetActive(false);
        settingsPanel.SetActive(false);
        shopPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
