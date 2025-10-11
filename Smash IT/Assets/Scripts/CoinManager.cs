using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;   // Singleton for easy access
    private int coins = 0;

    [Header("Starting Coins (for new players or reset)")]
    public int defaultStartingCoins = 100; //  You can change this in the Inspector

    public event Action<int> OnCoinsChanged;  // Event for UI updates

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
        LoadCoins();

        // Force reset for testing
        coins = 100; //  Set your desired starting amount
        SaveCoins();
        OnCoinsChanged?.Invoke(coins);
    }






    // Core Methods 
    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        OnCoinsChanged?.Invoke(coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            SaveCoins();
            OnCoinsChanged?.Invoke(coins);
            return true;
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false;
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    // Save / Load
    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COIN_KEY, coins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    //  reset from a button or editor
    public void ResetCoins()
    {
        coins = defaultStartingCoins;
        SaveCoins();
        OnCoinsChanged?.Invoke(coins);
        Debug.Log($"[CoinManager] Coins reset to {coins}");
    }
}
