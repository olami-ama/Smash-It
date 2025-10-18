using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class FactManagerScript : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text headerText;      // "Did You Know?" title
    public TMP_Text factText;        // Fact content text
    public Image factIcon;           // The light bulb image
    public GameObject factsPanel;    // The full panel container

    [Header("Fact Settings")]
    [TextArea(2, 5)]
    public string[] facts =
    {
        "The longest ping pong rally lasted over 8 hours!",
        "Table tennis was invented in England during the 1880s.",
        "The first official world championship was held in 1926.",
        "Ping pong balls were once made of celluloid, which is very flammable.",
        "A table tennis ball weighs only 2.7 grams!",
        "China has won more than 60% of all Olympic table tennis medals.",
        "The fastest recorded smash in ping pong was 116 km/h!",
        "Table tennis is one of the most popular indoor sports in the world.",
        "Players can impart up to 9000 rpm of spin on a table tennis ball!"
    };

    [Header("Visual Settings")]
    public float fadeDuration = 1f;      // How long it fades in
    public float displayDuration = 4f;   // How long the panel stays visible

    private CanvasGroup panelGroup;

    void Start()
    {
        if (factsPanel != null)
        {
            panelGroup = factsPanel.GetComponent<CanvasGroup>();
            if (panelGroup == null)
                panelGroup = factsPanel.AddComponent<CanvasGroup>();

            factsPanel.SetActive(false);
        }

        // Start showing fact after small delay
        Invoke(nameof(ShowRandomFact), 0.3f);
    }

    public void ShowRandomFact()
    {
        if (factsPanel == null || factText == null) return;

        // Select a random fact
        int randomIndex = Random.Range(0, facts.Length);
        string selectedFact = facts[randomIndex];
        factText.text = selectedFact;

        // Make sure header says "Did You Know?"
        if (headerText != null)
            headerText.text = "Did You Know?";

        // Show the panel and start animation
        factsPanel.SetActive(true);
        StartCoroutine(FadePanelRoutine());
    }

    IEnumerator FadePanelRoutine()
    {
        // Fade In
        panelGroup.alpha = 0f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        // Wait while the player reads
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        factsPanel.SetActive(false);
    }
}
