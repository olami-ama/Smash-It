using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MatchSettings", menuName = "Scriptable Objects/MatchSettings")]
public class MatchSettings : ScriptableObject
{
    public enum GameMode
    {
        LevelMode,
        EndlessMode
    }

    public GameMode selectedMode;

    public List<PowerUpType> allowedPowerUps = new List<PowerUpType>();

    public int maxPowerUps = 3;
}
