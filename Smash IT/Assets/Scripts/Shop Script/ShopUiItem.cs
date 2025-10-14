using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUiItem : MonoBehaviour
{
    [Header("Item Data")]
    public ShopItem item;            // The ScriptableObject with item info

    [Header("UI References")]
    public TMP_Text nameText;        // Item name text
    public TMP_Text costText;        // Item cost text
    public TMP_Text ownedText;       // Amount owned text
    public Image icon;               // Item icon
    public Button buyButton;         // Buy button

    private void Start()
    {
        if (item == null)
        {
            Debug.LogWarning("[ShopUIItem] Missing ShopItem reference!");
            return;
        }

        // Initialize display once when the shop loads
        SetupDisplay();

        // Add button listener
        buyButton.onClick.AddListener(Buy);
    }

    // Sets up the display for this item
    private void SetupDisplay()
    {
        nameText.text = item.itemName;
        costText.text = $"Cost: {item.cost}";

        // Load the current owned count from ShopManager
        UpdateOwnedText(ShopManager.Instance.GetConsumableCount(item.itemName));

        if (icon != null)
            icon.sprite = item.icon;
    }

    // Called when buy button is clicked
    private void Buy()
    {
        ShopManager.Instance.BuyItem(item, 1);
        // Immediately refresh the owned count after buying
        UpdateOwnedText(ShopManager.Instance.GetConsumableCount(item.itemName));
    }

    // Updates the number of items owned (called by ShopManager.RefreshAllItemsUI)
    public void UpdateOwnedText(int owned)
    {
        ownedText.text = $"Owned: {owned}";
    }

    //  force refresh after PlayerPrefs reset
    private void OnEnable()
    {
        // Whenever this UI reactivates, reload latest data
        if (ShopManager.Instance != null && item != null)
            UpdateOwnedText(ShopManager.Instance.GetConsumableCount(item.itemName));
    }
}
