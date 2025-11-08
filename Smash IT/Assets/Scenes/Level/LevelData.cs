using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Basic Info")]
    public string levelName = "Level 1";
    public int levelNumber = 1;

    [Header("Gameplay Settings")]
    public float aiSpeed = 5f;
    public float ballSpeed = 8f;

   /* [Header("Visuals (optional)")]
    public Sprite backgroundSprite; // You can leave this for your teammate
    public Color backgroundColor = Color.white;
   */
    [Header("Rewards")]
    public int coinsReward = 100;
}
