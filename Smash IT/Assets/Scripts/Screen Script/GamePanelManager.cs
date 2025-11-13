using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GamePanelManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject blurBG;          // Assign the GameSceneBlurBG
    public List<GameObject> panels;    //  Assign all panels here
    [Header("Fade Settings")]
    public float fadeDuration = 0.3f;
    public float defaultBlurAlpha = 0.6f;

    private Image blurImage;

    void Awake()
    {
        if (blurBG != null)
        {
            blurImage = blurBG.GetComponent<Image>();
            if (blurImage == null)
                Debug.LogError("BlurBG must have an Image component!");

            blurBG.SetActive(false);
        }
        else
        {
            Debug.LogError("BlurBG reference is missing!");
        }

        // Disable all panels initially
        foreach (var panel in panels)
        {
            if (panel != null) panel.SetActive(false);
        }
    }

    void Update()
    {
        HandleBlurVisibility();
    }

    private void HandleBlurVisibility()
    {
        if (blurBG == null || blurImage == null) return;

        bool anyPanelActive = false;

        foreach (var panel in panels)
        {
            if (panel != null && panel.activeSelf)
            {
                anyPanelActive = true;
                break;
            }
        }

        // If any panel is active, fade blur in; else fade blur out
        if (anyPanelActive && !blurBG.activeSelf)
        {
            blurBG.SetActive(true);
            StartCoroutine(FadeImage(blurImage, 0f, defaultBlurAlpha, fadeDuration));
        }
        else if (!anyPanelActive && blurBG.activeSelf)
        {
            StartCoroutine(FadeOutBlur());
        }
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = image.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }

    private IEnumerator FadeOutBlur()
    {
        yield return FadeImage(blurImage, blurImage.color.a, 0f, fadeDuration);
        blurBG.SetActive(false);
    }
}
