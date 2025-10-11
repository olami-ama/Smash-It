using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PowerUpSelectUi : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpSlot
    {
        public string powerUpName;    // Must match ShopItem.itemName exactly
        public Button selectButton;   // The button the player clicks
        public TMP_Text countText;    // Shows how many owned (e.g. "x3")
        [HideInInspector] public bool selected; // Keeps track of which are clicked
    }

    [Header("Power-Up Slots (assign manually in Inspector)")]
    public List<PowerUpSlot> powerUpSlots = new List<PowerUpSlot>();

    [Header("Game Scene to Load")]
    public string gameSceneName; // "AI Game Screen" or "Multiplayer_MatchScene"

    [Header("Selection Limit")]
    public int maxSelections = 3;

    private void OnEnable()
    {
        RefreshUI();
    }

    private void Start()
    {
        RefreshUI();
    }

    // Refreshes all UI buttons and count labels
    public void RefreshUI()
    {
        foreach (var slot in powerUpSlots)
        {
            if (slot == null || slot.selectButton == null || slot.countText == null)
                continue;

            int owned = ShopManager.Instance != null ? ShopManager.Instance.GetConsumableCount(slot.powerUpName) : 0;
            slot.countText.text = "x" + owned;
            slot.selectButton.interactable = owned > 0;

            // Reset color & click
            slot.selectButton.image.color = slot.selected ? Color.green : Color.white;
            slot.selectButton.onClick.RemoveAllListeners();
            slot.selectButton.onClick.AddListener(() => OnPowerUpClicked(slot));
        }
    }

    // When player clicks on a power-up
    private void OnPowerUpClicked(PowerUpSlot slot)
    {
        if (slot.selected)
        {
            slot.selected = false;
            slot.selectButton.image.color = Color.white;
        }
        else
        {
            if (GetSelectedCount() >= maxSelections)
            {
                Debug.Log("Selection limit reached!");
                return;
            }

            int owned = ShopManager.Instance.GetConsumableCount(slot.powerUpName);
            if (owned <= 0)
            {
                Debug.Log($"You don't own {slot.powerUpName}!");
                return;
            }

            slot.selected = true;
            slot.selectButton.image.color = Color.green;
        }
    }

    // Counts how many power-ups are selected
    private int GetSelectedCount()
    {
        int count = 0;
        foreach (var s in powerUpSlots)
            if (s.selected) count++;
        return count;
    }

    // Called by Confirm button before loading the game
    public void ConfirmAndStartGame()
    {
        // Clear any old selections
        MatchSettingsData.selectedPowerUps.Clear();

        // Save the new selections
        foreach (var slot in powerUpSlots)
        {
            if (slot.selected)
            {
                MatchSettingsData.selectedPowerUps.Add(slot.powerUpName);
                ShopManager.Instance.ConsumeItem(slot.powerUpName, 1); // use up one
            }
        }

        Debug.Log($"[PowerUpSelectUI] Saved {MatchSettingsData.selectedPowerUps.Count} power-ups for next match.");

        // Load the chosen scene
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("[PowerUpSelectUI] No gameSceneName assigned!");
        }
    }
}
