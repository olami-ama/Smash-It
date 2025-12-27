using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Available Shop Items")]
    public List<ShopItem> shopItems = new List<ShopItem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ------------------------
    // COINS
    // ------------------------
    public void AddCoins(int amount)
    {
        CoinManager.Instance.AddCoins(amount);
    }

    public bool SpendCoins(int cost)
    {
        return CoinManager.Instance.SpendCoins(cost);
    }

    // ------------------------
    // BUY ITEM
    // ------------------------
    public void BuyItem(ShopItem item, int quantity = 1)
    {
        if (item == null) return;

        int totalCost = item.cost * quantity;

        if (!SpendCoins(totalCost))
        {
            Debug.Log($"[ShopManager] Not enough coins to buy {item.itemName}");
            return;
        }

        if (item.isConsumable)
        {
            AddConsumable(item.powerUpType, quantity);
        }
        else
        {
            // Permanent item (owned once)
            PlayerPrefs.SetInt(GetKey(item.powerUpType), 1);
        }


        PlayerPrefs.Save();
        RefreshAllItemsUI();
    }

    // ------------------------
    // CONSUMABLE LOGIC (ENUM BASED)
    // ------------------------
    private string GetKey(PowerUpType type)
    {
        return type.ToString() + "_Owned";
    }

    public int GetConsumableCount(PowerUpType type)
    {
        return PlayerPrefs.GetInt(type.ToString() + "_Owned", 0);
    }


    public void AddConsumable(PowerUpType type, int amount)
    {
        int current = GetConsumableCount(type);
        PlayerPrefs.SetInt(GetKey(type), current + amount);
        PlayerPrefs.Save();

        Debug.Log($"[ShopManager] Added {amount}x {type}. Total: {current + amount}");
    }

    public void ConsumeItem(PowerUpType type, int amount = 1)
    {
        string key = type.ToString() + "_Owned";

        int current = PlayerPrefs.GetInt(key, 0);
        current = Mathf.Max(0, current - amount);
        PlayerPrefs.SetInt(key, current);
        PlayerPrefs.Save();

        StartCoroutine(RefreshNextFrame());

    }
    private IEnumerator RefreshNextFrame()
    {
        yield return null; // wait one frame
        RefreshAllItemsUI();
    }


    // ------------------------
    // UI REFRESH
    // ------------------------
    public void RefreshAllItemsUI()
    {
        foreach (var ui in FindObjectsByType<ShopUiItem>(FindObjectsSortMode.None))
        {
            if (ui.item != null)
            {
                int owned = GetConsumableCount(ui.item.powerUpType);
                ui.UpdateOwnedText(owned);
            }
        }

        Canvas.ForceUpdateCanvases();
    }
}
