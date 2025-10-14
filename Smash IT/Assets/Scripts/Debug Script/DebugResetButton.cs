using UnityEngine;
using TMPro;

public class DebugResetButton : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text feedbackText;          // Shows short messages like “Coins Reset!”
    public GameObject confirmationPanel;   // The small popup that asks “Are you sure?”

    // This will hold the action to run after pressing “Yes”
    private System.Action confirmedAction;

    void Awake()
    {
        // Double protection  make sure this panel never shows in a final build
#if !UNITY_EDITOR
        if (gameObject.activeSelf)
        {
            Debug.LogWarning("[DebugResetButton] Debug panel disabled — not allowed in builds.");
            gameObject.SetActive(false);
        }
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        // Developer shortcut — Press F1 to toggle Debug Panel visibility (only in Editor)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        // Secret unlock (Shift + D)
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
        {
            gameObject.SetActive(true);
            Debug.Log("[DebugResetButton] Secret Debug Menu Unlocked!");
        }
#endif
    }

    // RESET BUTTON FUNCTIONS

    // When I click the Reset Coins button
    public void ConfirmResetCoins()
    {
        ShowConfirmation(() =>
        {
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.ResetCoins();
                ShowFeedback("Coins reset to default!");
            }
            else
            {
                ShowFeedback("CoinManager not found!");
            }
        });
    }

    // When I click the Reset Daily Rewards button
    public void ConfirmResetDailyRewards()
    {
        ShowConfirmation(() =>
        {
            // Delete the keys used by the daily login system
            PlayerPrefs.DeleteKey("LastLoginDate");
            PlayerPrefs.DeleteKey("LoginStreak");
            PlayerPrefs.Save();

            ShowFeedback("Daily rewards reset!");
            Debug.Log("[DebugPanel] Daily login data cleared.");
        });
    }

    // When I click the Reset Power-Ups button
    public void ConfirmResetPowerUps()
    {
        ShowConfirmation(() =>
        {
            // These are the save keys used by my power-up and shop system
            string[] powerupKeys = {
                "Bigball_Owned",
                "BigPaddle_Owned",
                "SlowBall_Owned",
                "SpeedBoost_Owned"
            };

            // Go through each key and delete it if it exists
            foreach (string key in powerupKeys)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(key);
                    Debug.Log($"[DebugPanel] Deleted Power-Up Key: {key}");
                }
            }

            PlayerPrefs.Save();

            // Wait a short moment before refreshing UI so PlayerPrefs updates first
            Invoke(nameof(RefreshPowerUpUI), 0.1f);

            ShowFeedback("All Power-Ups reset!");
        });
    }

    // When I click the Reset All button
    public void ConfirmResetAll()
    {
        ShowConfirmation(() =>
        {
            // This removes every saved PlayerPref (coins, shop, power-ups)
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            ShowFeedback("All data cleared!");
            Debug.Log("[DebugPanel] ALL PlayerPrefs cleared!");
        });
    }

    // CONFIRMATION POPUP LOGIC 

    // This shows the confirmation panel and stores what to do next
    private void ShowConfirmation(System.Action onConfirm)
    {
        confirmedAction = onConfirm;
        if (confirmationPanel != null)
            confirmationPanel.SetActive(true);
    }

    // When I click Yes on the confirmation popup
    public void OnConfirmYes()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        confirmedAction?.Invoke(); // Run the action that was waiting
    }

    // When I click No on the confirmation popup
    public void OnConfirmNo()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        ShowFeedback("Reset canceled.");
    }

    //HELPER FUNCTIONS 

    // Shows text messages on screen and in the console
    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;

        Debug.Log(message);
    }

    // This refreshes my Power-Up or Shop UI after resetting
    private void RefreshPowerUpUI()
    {
        // Look for the PowerUpSelectUi script in the scene
        var powerUpUI = FindAnyObjectByType<PowerUpSelectUi>();
        if (powerUpUI != null)
        {
            powerUpUI.RefreshUI();
            Debug.Log("[DebugPanel] Power-Up UI refreshed successfully.");
        }

        // Look for the ShopManager script in the scene
        var shopManager = FindAnyObjectByType<ShopManager>();
        if (shopManager != null)
        {
            // Use your ShopManager’s existing function to update the item display
            shopManager.RefreshAllItemsUI();
            Debug.Log("[DebugPanel] Shop UI refreshed successfully.");
        }
    }
}
