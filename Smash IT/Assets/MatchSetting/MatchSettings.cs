using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "MatchSettings", menuName = "Scriptable Objects/MatchSettings")]
public class MatchSettings : ScriptableObject
{
    // Uses your existing enum type from PowerUpPickup
    public List<PowerUpPickup.PowerUpType> allowedPowerUps = new List<PowerUpPickup.PowerUpType>();
    public int maxPowerUps = 3;


}
