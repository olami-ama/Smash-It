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
        Debug.Log("CurrentLevelIndex = " + GameSession.CurrentLevelIndex);

    }

    public void OnPlayClicked()
    {
        if (isStarting) return;
        isStarting = true;

        var powerUI = FindFirstObjectByType<PowerUpSelectUi>();

        if (powerUI == null)
        {
            Debug.LogError("PowerUpSelectUi not found in scene!");
            return;
        }

        powerUI.ConfirmAndStartGame(StartSelectedLevel);

       
    }


    public void StartSelectedLevel()
    {
        if (LevelManager.Instance == null) return;

        int index = GameSession.CurrentLevelIndex;

        LevelManager.Instance.LoadLevel(index);

        gameObject.SetActive(false);
    }

}
