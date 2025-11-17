using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NextLevelPanel : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text levelNameText;
    public TMP_Text goalText;
    public Button playButton;

    private LevelData nextLevelData;
    private bool isStarting = false;

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
    }

    public void Setup(LevelData levelData)
    {
        if (levelData == null) return;

        nextLevelData = levelData;
        levelNameText.text = levelData.levelName;
        goalText.text = levelData.goalDescription;

        gameObject.SetActive(true);
        isStarting = false; // reset flag
    }

    public void OnPlayClicked()
    {
        if (isStarting) return;
        isStarting = true;

        PowerUpSelectUi.Instance.ConfirmAndStartGame(() =>
        {
            StartSelectedLevel();
        });
    }

    public void StartSelectedLevel()
    {
        if (LevelManager.Instance == null || nextLevelData == null) return;

        // Load the exact level this panel is showing
        int nextIndex = LevelManager.Instance.levelDataList.IndexOf(nextLevelData);
        if (nextIndex < 0)
        {
            Debug.LogWarning("[NextLevelPanel] LevelData not found in LevelManager!");
            return;
        }

        // Sync CurrentLevelIndex with what we are loading
        GameSession.SetCurrentLevel(nextIndex);

        Debug.Log($"[NextLevelPanel] Starting level index: {nextIndex}");
        LevelManager.Instance.LoadLevel(nextIndex);

        gameObject.SetActive(false);
        isStarting = false;
    }
}
