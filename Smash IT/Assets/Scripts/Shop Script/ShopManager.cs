using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;  // Singleton for global access

    [Header("Available Shop Items")]
    public List<ShopItem> shopItems = new List<ShopItem>(); // All power-ups or items in the shop

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ensures ShopManager persists between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Add coins when player earns them
    public void AddCoins(int amount)
    {
        CoinManager.Instance.AddCoins(amount);
    }

    // Spend coins when buying items
    public bool SpendCoins(int cost)
    {
        return CoinManager.Instance.SpendCoins(cost);
    }

    // Buy an item (consumable or permanent)
    public void BuyItem(ShopItem item, int quantity = 1)
    {
        if (item == null) return;

        int totalCost = item.cost * quantity;

        // Check if player has enough coins
        if (!SpendCoins(totalCost))
        {
            Debug.Log($"[ShopManager] Not enough coins to buy {item.itemName}!");
            return;
        }

        // Handle consumables (can have multiple)
        if (item.isConsumable)
        {
            int current = PlayerPrefs.GetInt(item.itemName + "_Owned", 0);
            PlayerPrefs.SetInt(item.itemName + "_Owned", current + quantity);
        }
        else
        {
            // Permanents can only be bought once
            PlayerPrefs.SetInt(item.itemName + "_Owned", 1);
        }

        PlayerPrefs.Save();
        Debug.Log($"[ShopManager] Bought {quantity}x {item.itemName}. Total now: {GetConsumableCount(item.itemName)}");

        RefreshAllItemsUI(); // Refresh UI display
    }

    // Deduct consumables when used
    public void ConsumeItem(string itemName, int amount = 1)
    {
        int current = PlayerPrefs.GetInt(itemName + "_Owned", 0);
        current = Mathf.Max(0, current - amount);
        PlayerPrefs.SetInt(itemName + "_Owned", current);
        PlayerPrefs.Save();

        Debug.Log($"[ShopManager] Consumed {amount}x {itemName}. Remaining: {current}");
        RefreshAllItemsUI(); // Update shop display
    }

    // Get how many of a consumable the player owns
    public int GetConsumableCount(string itemName)
    {
        return PlayerPrefs.GetInt(itemName + "_Owned", 0);
    }


    public void RefreshAllItemsUI()
    {
        foreach (var ui in FindObjectsByType<ShopUiItem>(FindObjectsSortMode.None))
        {
            if (ui.item != null)
            {
                int owned = GetConsumableCount(ui.item.itemName);
                ui.UpdateOwnedText(owned);
            }
        }

        Canvas.ForceUpdateCanvases(); //  force immediate UI refresh
        Debug.Log("[ShopManager] Refreshed all shop item displays.");
    }


}
