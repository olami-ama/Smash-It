using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NextLevelPanel : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text levelNameText;
    public TMP_Text goalText;
    public Button playButton;

    private bool isStarting = false;
    private LevelData nextLevelData;

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
    }

    public void Setup(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogWarning("[NextLevelPanel] Setup called with null LevelData!");
            return;
        }

        nextLevelData = levelData;

        if (levelNameText != null)
            levelNameText.text = levelData.levelName;

        if (goalText != null)
            goalText.text = levelData.goalDescription;

        Debug.Log($"[NextLevelPanel] Setup complete for Level: {levelData.levelName}");
    }

    public void OnPlayClicked()
    {
        if (isStarting) return; // Prevent double-click issues
        isStarting = true;

        PowerUpSelectUi.Instance.ConfirmAndStartGame(() =>
        {
            GameSession.AdvanceLevel(); //  Advance to next level only now
            StartSelectedLevel();
        });
    }

    private void StartSelectedLevel()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("[NextLevelPanel] LevelManager not found!");
            return;
        }

        int nextLevelIndex = GameSession.CurrentLevelIndex;
        Debug.Log($"[NextLevelPanel] Starting next level index: {nextLevelIndex}");

        LevelManager.Instance.LoadLevel(nextLevelIndex);

        //  Hide this panel after loading
        gameObject.SetActive(false);
        isStarting = false;
    }
}
