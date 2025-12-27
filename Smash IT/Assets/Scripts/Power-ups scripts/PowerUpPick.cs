using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public PowerUpType powerUpType;

    [Header("Effect Settings")]
    public float bigPaddleFactor = 1.5f;
    public float speedBoostFactor = 1.4f;
    public float effectDuration = 6f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ball")) return;

        ApplyPowerUpToPlayer();
        Destroy(gameObject);
    }

    private void ApplyPowerUpToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Paddle");
        if (player == null)
        {
            Debug.LogWarning("[PowerUpPickup] Player paddle not found");
            return;
        }

        PowerUpEffect paddleEffect = player.GetComponent<PowerUpEffect>();
        BallPowerupEffect ballEffect = FindFirstObjectByType<BallPowerupEffect>();

        switch (powerUpType)
        {
            case PowerUpType.BigPaddle:
                paddleEffect?.ApplyBigPaddle(bigPaddleFactor, effectDuration);
                break;

            case PowerUpType.SpeedBoost:
                paddleEffect?.ApplySpeedBoost(speedBoostFactor, effectDuration);
                break;

            case PowerUpType.SlowBall:
                ballEffect?.ApplySlowBall(0.6f, effectDuration);
                break;
        }
    }
}
