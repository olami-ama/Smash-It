using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [Header("Level Info")]
    public int levelIndex; 
    public string gameplaySceneName = "GameScene";

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        Debug.Log("LEVEL BUTTON CLICKED Level " + levelIndex);

        GameSession.SetCurrentLevel(levelIndex);
        SceneManager.LoadScene(gameplaySceneName);
        Debug.Log("CurrentLevelIndex = " + GameSession.CurrentLevelIndex);

    }
}


