using UnityEngine;
using System.Collections;

public class PowerUpEffect : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 originalScale;
    private Coroutine activeRoutine;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        originalScale = transform.localScale;
    }

    // ------------------------
    // BIG PADDLE
    // ------------------------
    public void ApplyBigPaddle(float factor, float duration)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(BigRoutine(factor, duration));
    }

    private IEnumerator BigRoutine(float factor, float duration)
    {
        Vector3 targetScale = originalScale * factor;

        // Smooth grow
        float t = 0f;
        while (t < 0.3f)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / 0.3f);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        sr.color = Color.yellow;

        yield return new WaitForSeconds(duration - 1f);

        // Flash before ending
        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            sr.color = Color.yellow;
            yield return new WaitForSeconds(0.2f);
        }

        // Reset
        transform.localScale = originalScale;
        sr.color = originalColor;

        activeRoutine = null;
    }

    // ------------------------
    // SPEED BOOST
    // ------------------------
    public void ApplySpeedBoost(float factor, float duration)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(SpeedBoostRoutine(factor, duration));
    }

    private IEnumerator SpeedBoostRoutine(float factor, float duration)
    {
        PaddleMovementScript move = GetComponent<PaddleMovementScript>();
        if (move == null) yield break;

        float originalSpeed = move.moveSpeed;
        move.moveSpeed *= factor;
        sr.color = Color.red;

        yield return new WaitForSeconds(duration - 1f);

        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(0.2f);
            sr.color = Color.green;
            yield return new WaitForSeconds(0.2f);
        }

        move.moveSpeed = originalSpeed;
        sr.color = originalColor;
        activeRoutine = null;
    }

}

