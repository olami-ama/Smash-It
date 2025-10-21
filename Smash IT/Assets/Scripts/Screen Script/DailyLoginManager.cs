using UnityEngine;
using TMPro;
using System;
using System.Collections;


public class DailyLoginManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dailyRewardPanel;
    public TMP_Text rewardText;

    [Header("Settings")]
    public int[] rewards = { 50, 100, 150, 200, 250, 300, 400 }; // 7 days

    private string lastLoginKey = "LastLoginDate";
    private string streakKey = "LoginStreak";

    void Start()
    {
        StartCoroutine(InitializeDailyLogin());
    }

    private IEnumerator InitializeDailyLogin()
    {
        // Wait until CoinManager.Instance is ready
        yield return new WaitUntil(() => CoinManager.Instance != null);

        // Wait one frame to ensure UI is initialized
        yield return null;

        CheckDailyLogin();
    }


    void CheckDailyLogin()
    {
        string lastLoginString = PlayerPrefs.GetString(lastLoginKey, "");
        DateTime today = DateTime.Now.Date;

        if (string.IsNullOrEmpty(lastLoginString))
        {
            // First time login
            GiveReward(0);
            PlayerPrefs.SetString(lastLoginKey, today.ToString());
            PlayerPrefs.SetInt(streakKey, 1);
        }
        else
        {
            DateTime lastLoginDate = DateTime.Parse(lastLoginString);
            int streak = PlayerPrefs.GetInt(streakKey, 1);

            if ((today - lastLoginDate).Days >= 1)
            {
                // New day
                streak++;
                if (streak > rewards.Length)
                    streak = 1; // reset after 7 days

                GiveReward(streak - 1);

                PlayerPrefs.SetString(lastLoginKey, today.ToString());
                PlayerPrefs.SetInt(streakKey, streak);
            }
            else
            {
                // Already claimed today
                dailyRewardPanel.SetActive(false);
            }
        }
    }

    void GiveReward(int index)
    {
        int rewardAmount = rewards[index];
        CoinManager.Instance.AddCoins(rewardAmount);

        //  Manually refresh UI after reward
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCoinText(CoinManager.Instance.GetCoins());

        rewardText.text = $"You earned {rewardAmount} coins for Day {index + 1}!";
        dailyRewardPanel.SetActive(true);
    }


    public void CloseRewardPanel()
    {
        dailyRewardPanel.SetActive(false);
    }
}
