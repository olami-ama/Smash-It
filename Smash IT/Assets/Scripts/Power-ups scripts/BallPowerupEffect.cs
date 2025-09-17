using UnityEngine;
using System.Collections;

public class BallPowerupEffect : MonoBehaviour
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

    // SLOW BALL (factor < 1 to slow, >1 to speed up)
    public void ApplySlowBall(float factor, float duration)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(SlowBallRoutine_ModifyMultiplier(factor, duration));
    }

    private IEnumerator SlowBallRoutine_ModifyMultiplier(float factor, float duration)
    {
        // Prefer working with BallMovement so the ball's logic respects the change
        BallMovement bm = GetComponent<BallMovement>();
        if (bm != null)
        {
            float originalMultiplier = bm.speedMultiplier;
            bm.speedMultiplier = originalMultiplier * factor;
            Debug.Log($"[PowerUp] Set speedMultiplier {originalMultiplier} -> {bm.speedMultiplier} (factor={factor})");
            sr.color = Color.cyan;

            // Wait duration minus blinking time
            yield return new WaitForSeconds(Mathf.Max(0f, duration - 1f));

            // blinking
            for (int i = 0; i < 3; i++)
            {
                sr.color = Color.blue;
                yield return new WaitForSeconds(0.2f);
                sr.color = Color.cyan;
                yield return new WaitForSeconds(0.2f);
            }

            // restore
            bm.speedMultiplier = originalMultiplier;
            sr.color = originalColor;
            activeRoutine = null;
            yield break;
        }

        // If BallMovement not found, fallback to overriding Rigidbody velocity every physics tick
        yield return StartCoroutine(SlowBallRoutine_FallbackRigidbody(factor, duration));
    }

    // Fallback that repeatedly enforces velocity while active
    private IEnumerator SlowBallRoutine_FallbackRigidbody(float factor, float duration)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        float originalSpeed = rb.linearVelocity.magnitude;
        float targetSpeed = originalSpeed * factor;
        Debug.Log($"[PowerUp-Fallback] original={originalSpeed} target={targetSpeed}");
        sr.color = Color.cyan;

        float end = Time.time + duration;
        while (Time.time < end)
        {
            if (rb.linearVelocity != Vector2.zero)
                rb.linearVelocity = rb.linearVelocity.normalized * targetSpeed;
            yield return new WaitForFixedUpdate();
        }

        // blinking
        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.blue;
            yield return new WaitForSeconds(0.2f);
            sr.color = Color.cyan;
            yield return new WaitForSeconds(0.2f);
        }

        if (rb.linearVelocity != Vector2.zero)
            rb.linearVelocity = rb.linearVelocity.normalized * originalSpeed;

        sr.color = originalColor;
        activeRoutine = null;
    }

    // BIG BALL unchanged
    public void ApplyBigball(float factor, float duration)
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(BigballRoutine(factor, duration));
    }

    private IEnumerator BigballRoutine(float factor, float duration)
    {
        Debug.Log("Big Ball power-up activated!");
        transform.localScale = originalScale * factor;
        sr.color = Color.magenta;
        yield return new WaitForSeconds(duration);
        transform.localScale = originalScale;
        sr.color = originalColor;
        activeRoutine = null;
    }
}
