using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;   // The main home screen
    public GameObject gameModePanel;   // The panel for selecting game modes
    public GameObject settingsPanel;   // The panel for settings options
    public GameObject powerUpPanel;    // The panel for pre-match power-up selection
    public GameObject shopPanel;       // The shop panel where players buy items

    private string pendingScene;       // Stores which game scene to load after selection

    private void Start()
    {
        // Ensure only main menu is visible at launch
        mainMenuPanel.SetActive(true);
        gameModePanel.SetActive(false);
        settingsPanel.SetActive(false);
        powerUpPanel.SetActive(false);
        shopPanel.SetActive(false);
    }

    // Opens the Game Mode selection panel
    public void OpenGameModeMenu()
    {
        mainMenuPanel.SetActive(false);
        gameModePanel.SetActive(true);
    }

    // Opens the Settings panel
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Opens the Shop panel
    public void OpenShop()
    {
        mainMenuPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    // Closes the Shop and returns to Main Menu
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Quits the game (works only in builds)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }

    // Player vs AI Mode
    public void StartPlayerVsBot()
    {
        pendingScene = "AI Game Screen";
        OpenPowerUpPanel();
    }

    // Player vs Player Mode
    public void StartPlayerVsPlayer()
    {
        pendingScene = "Multiplayer_MatchScene";
        OpenPowerUpPanel();
    }

    // Opens the Power-Up selection panel
    private void OpenPowerUpPanel()
    {
        gameModePanel.SetActive(false);
        powerUpPanel.SetActive(true);

        // Look for PowerUpSelectUi script dynamically
        var powerupUI = powerUpPanel.GetComponent<PowerUpSelectUi>();
        if (powerupUI != null)
        {
            powerupUI.gameSceneName = pendingScene; // Pass the target scene
            powerupUI.RefreshUI();
        }
        else
        {
            Debug.LogWarning("[MainMenuManager] PowerUpSelectUi not found on PowerUpPanel!");
        }
    }

    // Returns from any panel back to the Main Menu
    public void BackToMainMenu()
    {
        gameModePanel.SetActive(false);
        settingsPanel.SetActive(false);
        powerUpPanel.SetActive(false);
        shopPanel.SetActive(false);

        mainMenuPanel.SetActive(true);
    }
}
