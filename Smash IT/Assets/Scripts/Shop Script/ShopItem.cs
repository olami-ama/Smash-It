using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Scriptable Objects/ShopItem")]
public class ShopItem : ScriptableObject
{
   
    public string itemName;   // The name of the item (e.g., "Big Paddle")
    public int cost;          // How many coins it costs
    public Sprite icon;       // Image shown in shop
    public bool isConsumable; // True = one-time use, False = permanent unlock
}

