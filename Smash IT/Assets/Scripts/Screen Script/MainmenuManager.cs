using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameModePanel;
    public GameObject settingsPanel;
    public GameObject powerUpPanel;

    [Header("UI References")]
    public TMP_Text coinText;   // shows total coins on main menu

    private string pendingScene; // store which scene to load after selecting power-ups

    void Start()
    {
        UpdateCoinDisplay();
    }

    // COINS DISPLAY 
    public void UpdateCoinDisplay()
    {
        if (coinText != null && CoinManager.Instance != null)
        {
            coinText.text = "Coins: " + CoinManager.Instance.GetCoins();
        }
    }

    //  MAIN MENU 
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

    // GAME MODES
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

    // NAVIGATION 
    public void BackToMainMenu()
    {
        gameModePanel.SetActive(false);
        settingsPanel.SetActive(false);
        powerUpPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        // Refresh coin display in case player earned/spent coins before returning
        UpdateCoinDisplay();
    }
}
