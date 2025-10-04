using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;  // Singleton so any script can call ShopManager

    [Header("Items available in shop")]
    public List<ShopItem> shopItems = new List<ShopItem>(); // Drag & drop items in the Inspector

    // Consumables: itemName -> how many the player owns
    private Dictionary<string, int> consumableInventory = new Dictionary<string, int>();

    // Permanent unlocks (like skins)
    private HashSet<string> ownedPermanentItems = new HashSet<string>();

    // Items selected for the next match
    private List<string> selectedItems = new List<string>();

    // Keys for saving in PlayerPrefs
    private const string CONSUMABLE_KEY = "ConsumableInventory";
    private const string PERMANENT_KEY = "PermanentItems";

    [Header("Selection Rules")]
    public int maxSelections = 3; // Max boosters player can pick before a match

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        LoadInventory(); // Load items from previous sessions
        selectedItems.Clear(); // Always start with empty selection
    }

    // ------------------- BUYING -------------------
    public bool BuyItem(ShopItem item, int quantity = 1)
    {
        // Check if player has enough coins
        if (CoinManager.Instance.GetCoins() < item.cost)
        {
            Debug.Log("[ShopManager] Not enough coins!");
            return false;
        }

        // Deduct coins
        CoinManager.Instance.SpendCoins(item.cost);

        if (item.isConsumable)
        {
            // If consumable, add to inventory count
            if (!consumableInventory.ContainsKey(item.itemName))
                consumableInventory[item.itemName] = 0;

            consumableInventory[item.itemName] += quantity;
            Debug.Log($"[ShopManager] Bought {quantity}x {item.itemName}. Total now: {consumableInventory[item.itemName]}");
        }
        else
        {
            // If permanent, add once
            ownedPermanentItems.Add(item.itemName);
            Debug.Log($"[ShopManager] Permanently unlocked {item.itemName}");
        }

        SaveInventory();
        return true;
    }

    // ------------------- SELECTION -------------------
    public bool SelectItem(string itemName)
    {
        // Permanent item check
        if (ownedPermanentItems.Contains(itemName))
        {
            if (selectedItems.Count < maxSelections)
            {
                selectedItems.Add(itemName);
                return true;
            }
        }
        // Consumable check
        else if (consumableInventory.ContainsKey(itemName) && consumableInventory[itemName] > 0)
        {
            if (selectedItems.Count < maxSelections)
            {
                selectedItems.Add(itemName);
                return true;
            }
        }

        Debug.Log($"[ShopManager] Cannot select {itemName} (not owned or none left).");
        return false;
    }

    public void ConfirmSelections()
    {
        // Deduct consumables when match starts
        foreach (var item in selectedItems)
        {
            if (consumableInventory.ContainsKey(item))
            {
                consumableInventory[item] = Mathf.Max(0, consumableInventory[item] - 1);
                Debug.Log($"[ShopManager] Used 1 {item}. Remaining: {consumableInventory[item]}");
            }
        }

        SaveInventory();
    }

    public void ClearSelections()
    {
        selectedItems.Clear();
    }

    public List<string> GetSelectedItems()
    {
        return new List<string>(selectedItems); // Return a copy so external scripts don’t edit directly
    }

    // ------------------- HELPERS -------------------
    public int GetConsumableCount(string itemName)
    {
        return consumableInventory.ContainsKey(itemName) ? consumableInventory[itemName] : 0;
    }

    public bool HasPermanent(string itemName)
    {
        return ownedPermanentItems.Contains(itemName);
    }

    // ------------------- SAVE / LOAD -------------------
    private void SaveInventory()
    {
        // Save consumables as "Item:Count"
        List<string> consumableData = new List<string>();
        foreach (var kvp in consumableInventory)
            consumableData.Add($"{kvp.Key}:{kvp.Value}");
        PlayerPrefs.SetString(CONSUMABLE_KEY, string.Join(",", consumableData));

        // Save permanent items as list
        PlayerPrefs.SetString(PERMANENT_KEY, string.Join(",", ownedPermanentItems));

        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        consumableInventory.Clear();
        ownedPermanentItems.Clear();

        // Load consumables
        string consumableData = PlayerPrefs.GetString(CONSUMABLE_KEY, "");
        if (!string.IsNullOrEmpty(consumableData))
        {
            string[] entries = consumableData.Split(',');
            foreach (string entry in entries)
            {
                if (string.IsNullOrEmpty(entry)) continue;
                string[] parts = entry.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out int qty))
                {
                    consumableInventory[parts[0]] = qty;
                }
            }
        }

        // Load permanents
        string permanentData = PlayerPrefs.GetString(PERMANENT_KEY, "");
        if (!string.IsNullOrEmpty(permanentData))
        {
            string[] items = permanentData.Split(',');
            foreach (string i in items)
                if (!string.IsNullOrEmpty(i)) ownedPermanentItems.Add(i);
        }
    }
}
