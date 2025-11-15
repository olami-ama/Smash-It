using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplashFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Start()
    {
        StartCoroutine(FadeInOut());
    }

    private IEnumerator FadeInOut()
    {
        // Fade In
        yield return Fade(1f, 0f);

        // Wait while your logo shows
        yield return new WaitForSeconds(2f);

        // Fade Out
        yield return Fade(0f, 1f);
    }

    private IEnumerator Fade(float start, float end)
    {
        float elapsed = 0f;

        Color c = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            c.a = Mathf.Lerp(start, end, t);
            fadeImage.color = c;
            yield return null;
        }
    }
}
