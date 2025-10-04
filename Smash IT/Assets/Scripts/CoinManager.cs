using UnityEngine;
using System; 

public class CoinManager : MonoBehaviour
{
  
    public static CoinManager Instance;   // Singleton for easy access
    private int coins = 0;

    public event Action<int> OnCoinsChanged;  // Event for UI updates

    private const string COIN_KEY = "PlayerCoins";

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCoins();
        AddCoins(500); // start with 500 coins for testing
    }

    
    // Add coins after a match
    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        OnCoinsChanged?.Invoke(coins);
    }

    // Spend coins (returns true if successful)
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

    // Get current coins
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
        coins = PlayerPrefs.GetInt(COIN_KEY, 0); // default 0
    }
}

