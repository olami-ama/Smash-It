using UnityEngine;
using System.Collections;

public class ConfettiManager : MonoBehaviour
{
    [Header("Confetti Systems")]
    public ParticleSystem leftConfetti;
    public ParticleSystem rightConfetti;

    [Header("FX Settings")]
    public float confettiDuration = 3.5f; // How long confetti stays before stopping
    public string canvasEffectName = "CanvasEffect"; // Canvas name for overlay rendering

    void Start()
    {
        // Make sure both are off when the scene loads
        StopConfetti();
    }
    void Update()
    {
#if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.C))
    {
        Debug.Log("[ConfettiManager] Manual confetti test (C pressed)");
        PlayConfetti();
    }
#endif
    }


    public void PlayConfetti()
    {
        // Try to find the CanvasEffect GameObject
        GameObject canvasEffectObj = GameObject.Find(canvasEffectName);
        if (canvasEffectObj != null)
        {
            Canvas canvas = canvasEffectObj.GetComponent<Canvas>();
            if (canvas != null && transform.parent != canvas.transform)
            {
                transform.SetParent(canvas.transform, false);
                Debug.Log("[ConfettiManager] Attached to CanvasEffect for overlay rendering.");
            }
        }
        else
        {
            Debug.LogWarning($"[ConfettiManager] Canvas '{canvasEffectName}' not found in scene!");
        }

        // Restart both confetti systems
        if (leftConfetti)
        {
            leftConfetti.Stop();
            leftConfetti.Play();
        }

        if (rightConfetti)
        {
            rightConfetti.Stop();
            rightConfetti.Play();
        }

        Debug.Log("[ConfettiManager] Confetti started!");

        // Stop automatically after a few seconds
        StartCoroutine(StopAfterDelay());
    }

    private IEnumerator StopAfterDelay()
    {
        yield return new WaitForSeconds(confettiDuration);
        StopConfetti();
    }

    public void StopConfetti()
    {
        if (leftConfetti && leftConfetti.isPlaying)
            leftConfetti.Stop();

        if (rightConfetti && rightConfetti.isPlaying)
            rightConfetti.Stop();

        Debug.Log("[ConfettiManager] Confetti stopped.");
    }
}
