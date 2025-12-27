using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Scriptable Objects/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public int cost;
    public bool isConsumable;

    // 
    public PowerUpType powerUpType;
}
