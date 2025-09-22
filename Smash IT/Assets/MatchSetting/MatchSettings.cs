using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MatchSettings", menuName = "Scriptable Objects/MatchSettings")]
public class MatchSettings : ScriptableObject
{
    public enum GameMode
    {
        PlayerVsBot,
        PlayerVsPlayer
    }

    // Selected mode for this match
    public GameMode selectedMode;

    // Power-up control
    public List<PowerUpPickup.PowerUpType> allowedPowerUps = new List<PowerUpPickup.PowerUpType>();

    public int maxPowerUps = 3;
}

