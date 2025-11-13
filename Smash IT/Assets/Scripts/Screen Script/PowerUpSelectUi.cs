using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PowerUpSelectUi : MonoBehaviour
{
    public static PowerUpSelectUi Instance; // ✅ Added singleton

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [System.Serializable]
    public class PowerUpSlot
    {
        public string powerUpName;
        public Button selectButton;
        public TMP_Text countText;
        [HideInInspector] public bool selected;
    }

    [Header("Power-Up Slots (assign manually in Inspector)")]
    public List<PowerUpSlot> powerUpSlots = new List<PowerUpSlot>();

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

    public void RefreshUI()
    {
        foreach (var slot in powerUpSlots)
        {
            if (slot == null || slot.selectButton == null || slot.countText == null)
                continue;

            int owned = ShopManager.Instance != null ? ShopManager.Instance.GetConsumableCount(slot.powerUpName) : 0;

            slot.countText.text = owned.ToString();
            slot.selectButton.interactable = owned > 0;
            slot.selectButton.image.color = slot.selected ? Color.green : Color.white;

            slot.selectButton.onClick.RemoveAllListeners();
            slot.selectButton.onClick.AddListener(() => OnPowerUpClicked(slot));
        }
    }

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

    private int GetSelectedCount()
    {
        int count = 0;
        foreach (var s in powerUpSlots)
            if (s.selected) count++;
        return count;
    }

    public void ConfirmAndStartGame(System.Action onConfirmed = null)
    {
        MatchSettingsData.selectedPowerUps.Clear();

        foreach (var slot in powerUpSlots)
        {
            if (slot.selected)
            {
                MatchSettingsData.selectedPowerUps.Add(slot.powerUpName);
                ShopManager.Instance.ConsumeItem(slot.powerUpName, 1);
            }
        }

        Debug.Log($"[PowerUpSelectUI] Saved {MatchSettingsData.selectedPowerUps.Count} power-ups for next match.");

        onConfirmed?.Invoke();

        if (GameManager.Instance != null)
        {
            Debug.Log("[PowerUpSelectUI] Starting match inside the same gameplay scene...");
            GameManager.Instance.ResetGame();
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[PowerUpSelectUI] GameManager not found in scene!");
        }
    }
}
