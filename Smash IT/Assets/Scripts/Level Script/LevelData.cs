using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Basic Info")]
    public string levelName = " 1";
    public int levelNumber = 1;

    [Header("Gameplay Settings")]
    public float aiSpeed = 5f;
    public float ballSpeed = 8f;

    [Header("Goal")]
    public string goalDescription = "5 points ";

    [Header("Rewards")]
    public int coinsReward = 100;
}
