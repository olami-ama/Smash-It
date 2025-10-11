using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUiItem : MonoBehaviour
{
    [Header("Item Data")]
    public ShopItem item;            // The ScriptableObject with item info

    [Header("UI References")]
    public TMP_Text nameText;        // Item name
    public TMP_Text costText;        // Item cost
    public TMP_Text ownedText;       // Amount owned
    public Image icon;               // Sprite image
    public Button buyButton;         // Buy button

    private void Start()
    {
        if (item == null)
        {
            Debug.LogWarning("[ShopUIItem] Missing ShopItem reference!");
            return;
        }

        // Initialize display
        nameText.text = item.itemName;
        costText.text = "Cost: " + item.cost;
        UpdateOwnedText(ShopManager.Instance.GetConsumableCount(item.itemName));

        if (icon != null)
            icon.sprite = item.icon;

        // Add button listener
        buyButton.onClick.AddListener(() => Buy());
    }

    // Called when buy button is clicked
    public void Buy()
    {
        ShopManager.Instance.BuyItem(item, 1);
    }

    //  Update the number of items owned
    public void UpdateOwnedText(int owned)
    {
        ownedText.text = "Owned: " + owned;
    }
}

