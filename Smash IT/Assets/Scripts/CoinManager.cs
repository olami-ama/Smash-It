using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    private int coins = 0;

    [Header("Starting Coins (for new players or reset)")]
    public int defaultStartingCoins = 100;

    public event Action<int> OnCoinsChanged;

    private const string COIN_KEY = "PlayerCoins";

    private void Awake()
    {
        // Singleton setup
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

    private void Start()
    {
        // Load coins from PlayerPrefs
        if (PlayerPrefs.HasKey(COIN_KEY))
        {
            LoadCoins();
        }
        else
        {
            coins = defaultStartingCoins;
            SaveCoins();
        }

        // Notify UI on startup
        OnCoinsChanged?.Invoke(coins);

        Debug.Log($"[CoinManager] Loaded coins: {coins}");
    }

    // --- Core Methods ---
    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        OnCoinsChanged?.Invoke(coins);
        Debug.Log($"[CoinManager] Added {amount}. Total: {coins}");
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            SaveCoins();
            OnCoinsChanged?.Invoke(coins);
            Debug.Log($"[CoinManager] Spent {amount}. Remaining: {coins}");
            return true;
        }
        else
        {
            Debug.LogWarning("[CoinManager] Not enough coins!");
            return false;
        }
    }
   
    public int GetCoins()
    {
        return coins;
    }

    // --- Save / Load ---
    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COIN_KEY, coins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    // --- Reset from button or editor ---
    public void ResetCoins()
    {
        coins = defaultStartingCoins;
        SaveCoins();
        OnCoinsChanged?.Invoke(coins);
        Debug.Log($"[CoinManager] Coins reset to {coins}");
    }
}
