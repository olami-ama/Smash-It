using UnityEngine;

public static class GameSession
{
    public static int CurrentLevelIndex { get; private set; } = 0;

    public static void SetCurrentLevel(int index)
    {
        CurrentLevelIndex = Mathf.Max(0, index);
        Debug.Log($"[GameSession] Current level set to {CurrentLevelIndex}");
    }

    public static void AdvanceLevel()
    {
        CurrentLevelIndex++;
        Debug.Log($"[GameSession] Advanced to next level: {CurrentLevelIndex}");
    }
}
