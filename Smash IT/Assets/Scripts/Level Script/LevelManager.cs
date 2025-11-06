using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Info")]
    public int currentLevel = 1;
    public float baseAISpeed = 5f;
    public float baseBallSpeed = 8f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Load saved level progress
        if (PlayerPrefs.HasKey("CurrentLevel"))
            currentLevel = PlayerPrefs.GetInt("CurrentLevel");

        Debug.Log($"[LevelManager] Loaded Level {currentLevel}");
    }

    public void StartLevel()
    {
        // Adjust difficulty based on level
        float aiSpeed = baseAISpeed + (currentLevel * 0.4f);
        float ballSpeed = baseBallSpeed + (currentLevel * 0.2f);

        // Find the AI paddle and adjust its speed dynamically
        AIPaddleMovement ai = FindFirstObjectByType<AIPaddleMovement>();
        if (ai != null)
        {
            ai.moveSpeed = aiSpeed;
            Debug.Log($"[LevelManager] AI speed set to {aiSpeed}");
        }

        // Find the ball and adjust its base speed
        BallMovement ball = FindFirstObjectByType<BallMovement>();
        if (ball != null)
        {
            ball.speedMultiplier = ballSpeed / 8f; // scale factor relative to your baseBallSpeed
            Debug.Log($"[LevelManager] Ball speed multiplier set to {ball.speedMultiplier:F2}");
        }

        Debug.Log($"[LevelManager] Level {currentLevel} started | AI Speed: {aiSpeed}, Ball Speed: {ballSpeed}");
    }


    public void CompleteLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        Debug.Log($"[LevelManager] Level {currentLevel - 1} complete!");
    }


    private IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);

        string nextScene = "Level" + currentLevel;
        if (Application.CanStreamedLevelBeLoaded(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("[LevelManager] No more levels found — showing Main Menu.");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("CurrentLevel");
        currentLevel = 1;
        Debug.Log("[LevelManager] Progress reset to Level 1");
    }

   public void LoadNextLevel()
    {
        int nextLevel = currentLevel + 1;
        Debug.Log($"[LevelManager] Loading next level manually (GoToNextLevel). Next Level: {nextLevel}");

        // Try to load the next level scene
        string nextScene = "Level" + nextLevel;
        if (Application.CanStreamedLevelBeLoaded(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("[LevelManager] No next level found — returning to Main Menu.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
