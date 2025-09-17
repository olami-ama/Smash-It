using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public enum PowerUpType
    {
        BigPaddle,
        SpeedBoost,
        SlowBall,
        Bigball, //  Added slot for Faith
    }

    public PowerUpType type;
    public float factor = 1.5f;   // default effect strength
    public float duration = 5f;   // default duration in seconds

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug so you can see what object touched the power-up at runtime
        Debug.Log($"PowerUp collided with: name='{other.name}' tag='{other.tag}'");

        // ---------- PADDLE POWER-UPS ----------
        // Only apply paddle power-ups when the colliding object is a paddle (by tag).
        // Replace or extend these tag checks if you use different tag names.
        if (type == PowerUpType.BigPaddle || type == PowerUpType.SpeedBoost)
        {
            // Check paddle tags. Use CompareTag (fast and null-safe).
            bool isPaddle = other.CompareTag("Paddle") || other.CompareTag("Paddle2") || other.CompareTag("AI_Player");

            if (isPaddle)
            {
                // Try to find the PowerUpEffect component on the collider object or its parent.
                PowerUpEffect paddleEffects = other.GetComponent<PowerUpEffect>()
                                             ?? other.GetComponentInParent<PowerUpEffect>();

                if (paddleEffects != null)
                {
                    if (type == PowerUpType.BigPaddle) paddleEffects.ApplyBigPaddle(factor, duration);
                    if (type == PowerUpType.SpeedBoost) paddleEffects.ApplySpeedBoost(factor, duration);

                    Destroy(gameObject); // only destroy when applied successfully
                    return; // done
                }
                else
                {
                    Debug.LogWarning($"PowerUp: expected PowerUpEffect on paddle '{other.name}' but not found.");
                }
            }
        }

        // ---------- BALL POWER-UPS ----------
        // Only apply ball power-ups when the colliding object is the ball (by tag).
        if (type == PowerUpType.SlowBall || type == PowerUpType.Bigball)
        {
            if (other.CompareTag("Ball"))
            {
                BallPowerupEffect ballEffects = other.GetComponent<BallPowerupEffect>()
                                             ?? other.GetComponentInParent<BallPowerupEffect>();

                if (ballEffects != null)
                {
                    if (type == PowerUpType.SlowBall) ballEffects.ApplySlowBall(factor, duration);
                    if (type == PowerUpType.Bigball) ballEffects.ApplyBigball(factor, duration);

                    Destroy(gameObject);
                    return;
                }
                else
                {
                    Debug.LogWarning($"PowerUp: expected BallPowerupEffect on ball '{other.name}' but not found.");
                }
            }
        }

        // If we reach here, nothing matched — useful for debugging
        Debug.Log($"PowerUp: no effect applied for type={type} on '{other.name}' (tag='{other.tag}')");
    }


}

