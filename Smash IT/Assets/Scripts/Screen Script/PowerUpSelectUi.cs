using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
public class PowerUpSelectUi : MonoBehaviour
{
    public static PowerUpSelectUi Instance;

    [Serializable]
    public class PowerUpSlot
    {
        public PowerUpType type;
        public Button button;
        public TMP_Text countText;

        [HideInInspector] public bool isSelected;
        [HideInInspector] public int owned;
    }

    public List<PowerUpSlot> slots = new List<PowerUpSlot>();
    public int maxSelections = 3;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        RefreshUI();
    }

    private void OnEnable()
    {
        if (gameObject.activeInHierarchy)
            RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (var slot in slots)
        {
            slot.owned = ShopManager.Instance.GetConsumableCount(slot.type);
            slot.countText.text = slot.owned.ToString();

            slot.button.onClick.RemoveAllListeners();
            slot.button.onClick.AddListener(() => OnPowerUpClicked(slot));

            slot.isSelected = false;
        }
    }


    private void OnPowerUpClicked(PowerUpSlot slot)
    {
        // DESELECT
        if (slot.isSelected)
        {
            slot.isSelected = false;
            slot.owned += 1;
            slot.countText.text = slot.owned.ToString();

            Debug.Log($"[PowerUpSelectUI] Deselected {slot.type}");
            return;
        }

        // MAX LIMIT
        if (GetSelectedCount() >= maxSelections)
        {
            Debug.Log("[PowerUpSelectUI] Max power-ups selected");
            return;
        }

        // NO STOCK
        if (slot.owned <= 0)
        {
            Debug.Log($"[PowerUpSelectUI] No {slot.type} available");
            return;
        }

        // SELECT
        slot.isSelected = true;
        slot.owned -= 1;
        slot.countText.text = slot.owned.ToString();

        Debug.Log($"[PowerUpSelectUI] Selected {slot.type}");
    }


    private int GetSelectedCount()
    {
        int count = 0;
        foreach (var s in slots)
            if (s.isSelected) count++;
        return count;
    }

    public void ConfirmAndStartGame(Action onDone)
    {
        MatchSettingsData.selectedPowerUps.Clear();

        foreach (var slot in slots)
        {
            if (!slot.isSelected) continue;

            MatchSettingsData.selectedPowerUps.Add(slot.type);
            ShopManager.Instance.ConsumeItem(slot.type, 1);
        }

        Debug.Log("[PowerUpSelectUI] Final selection: " +
            string.Join(", ", MatchSettingsData.selectedPowerUps));

        onDone?.Invoke();
    }

}

