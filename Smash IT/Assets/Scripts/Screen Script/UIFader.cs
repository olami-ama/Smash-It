using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFader : MonoBehaviour
{
    // Singleton instance (optional, makes it easy to call from other scripts)
    public static UIFader Instance;

    [Header("Overlay Settings")]
    public Image fadeOverlay;      // Fullscreen Image (black, alpha = 0)
    public float fadeDuration = 0.5f; // Duration of fade in/out

    void Awake()
    {
        // Singleton pattern → keep only one UIFader alive
        if (Instance == null)
        {
            Instance = this;
            
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

   
    /// Fade to black, load a new scene, then fade back in.

    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Fade screen to black
        yield return Fade(1);

        // Load the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

        // Fade back to visible
        yield return Fade(0);
    }

 
    /// Fade overlay to a target alpha (0 = transparent, 1 = black).
  
    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeOverlay.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Ensure final alpha is set exactly
        fadeOverlay.color = new Color(0, 0, 0, targetAlpha);
    }
}
