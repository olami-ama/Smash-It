using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFader : MonoBehaviour
{
    public static UIFader Instance;

    public Image fadeOverlay; // full screen Image (black with alpha 0)
    public float fadeDuration = 0.5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return Fade(1); // fade to black
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        yield return Fade(0); // fade back in
    }

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
    }
}
