using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public Image fadeImage;          // The full screen black Image
    public GameObject logo;          // Your logo object (Image or group)

    [Header("Timing")]
    public float fadeDuration = 1f;   // How long fade in and fade out last
    public float displayTime = 2f;    // How long logo stays visible

    private bool skipping = false;

    private void Start()
    {
        // Make sure logo visible and fadeImage alpha = 1 at start
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        logo.SetActive(true);

        StartCoroutine(PlaySplash());
    }

    private void Update()
    {
        // Tap to skip
        if (Input.GetMouseButtonDown(0) && skipping == false)
        {
            skipping = true;
        }
    }

    private IEnumerator PlaySplash()
    {
        // Fade In
        yield return Fade(1f, 0f);

        // Hold logo visible
        float timer = 0f;
        while (timer < displayTime)
        {
            if (skipping) break;
            timer += Time.deltaTime;
            yield return null;
        }

        // Fade Out
        yield return Fade(0f, 1f);

        // Load Main Menu
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            c.a = Mathf.Lerp(from, to, t);
            fadeImage.color = c;

            if (skipping)
            {
                // instantly go to black when skipping
                c.a = to;
                fadeImage.color = c;
                break;
            }

            yield return null;
        }
    }
}
