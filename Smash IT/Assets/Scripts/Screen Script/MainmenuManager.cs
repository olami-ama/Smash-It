using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameModePanel;
    public GameObject settingsPanel;
    public GameObject powerUpPanel;   // NEW

    private string pendingScene; // store which scene to load after selecting power-ups

    // --- MAIN MENU ---
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

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }

    // --- GAME MODES ---
    public void StartPlayerVsBot()
    {
        pendingScene = "AI Game Screen";  // remember which scene to load
        OpenPowerUpPanel();
    }

    public void StartPlayerVsPlayer()
    {
        pendingScene = "Multiplayer_MatchScene";
        OpenPowerUpPanel();
    }

    private void OpenPowerUpPanel()
    {
        gameModePanel.SetActive(false);
        powerUpPanel.SetActive(true);
    }

    public void ConfirmPowerUps()
    {
        // When player is done choosing, load the game
        SceneManager.LoadScene(pendingScene);
    }

    // --- NAVIGATION ---
    public void BackToMainMenu()
    {
        gameModePanel.SetActive(false);
        settingsPanel.SetActive(false);
        powerUpPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
