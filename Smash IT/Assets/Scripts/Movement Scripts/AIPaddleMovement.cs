using UnityEngine;

public class AIPaddleMovement : MonoBehaviour
{
    [Header("Ball (assign or tag as 'Ball')")]
    public Transform ball;
    public Rigidbody2D ballRb;
    public BallMovement ballMovement;

    [Header("AI Settings")]
    public float moveSpeed = 6f;
    public float serveDistance = 1.5f;
    public float smoothness = 3f;

    [Header("Movement Bounds")]
    public float leftLimit = -7f, rightLimit = 7f;
    public float bottomLimit = -4f, topLimit = 4f;

    private Rigidbody2D aiRb;
    private bool loggedFoundBall = false;

    void Start()
    {
        aiRb = GetComponent<Rigidbody2D>();
        if (aiRb != null && aiRb.bodyType != RigidbodyType2D.Kinematic)
            aiRb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (ball == null || ballRb == null)
        {
            GameObject b = GameObject.FindWithTag("Ball");
            if (b == null) return;
            ball = b.transform;
            ballRb = b.GetComponent<Rigidbody2D>();
            if (ballMovement == null) ballMovement = b.GetComponent<BallMovement>();
            if (!loggedFoundBall) { Debug.Log($"[AI] Found Ball {ball.name}"); loggedFoundBall = true; }
        }

        if (ball == null || ballRb == null) return;

        if (ballMovement != null && !ballMovement.isLaunched)
        {
            Vector3 targetPos = new Vector3(Mathf.Clamp(ball.position.x, leftLimit, rightLimit), transform.position.y, transform.position.z);
            MoveTo(targetPos, moveSpeed);

            if (Vector2.Distance(ball.position, transform.position) < serveDistance)
            {
                Debug.Log($"[AI] Serving ball from {name} (dist {Vector2.Distance(ball.position, transform.position)})");
                ballMovement.LaunchFromPaddle(transform);
            }
            return;
        }

        Vector2 v = ballRb.linearVelocity;
        if (v.sqrMagnitude < 0.01f) return;

        Vector2 toPaddle = (Vector2)transform.position - (Vector2)ball.position;
        bool ballComingTowardMe = Vector2.Dot(v, toPaddle) > 0f;
        if (!ballComingTowardMe) return;

        Vector3 followPos = new Vector3(Mathf.Clamp(ball.position.x, leftLimit, rightLimit), transform.position.y, transform.position.z);
        Vector3 newPos = Vector3.Lerp(transform.position, followPos, smoothness * Time.deltaTime);
        MoveTo(newPos, 0f);
    }

    void MoveTo(Vector3 target, float speed)
    {
        if (aiRb != null && aiRb.bodyType == RigidbodyType2D.Kinematic)
        {
            Vector2 pos = speed > 0f ? Vector2.MoveTowards(aiRb.position, (Vector2)target, speed * Time.deltaTime) : (Vector2)target;
            aiRb.MovePosition(pos);
            Debug.Log($"[AI] MovePosition -> {pos}");
        }
        else
        {
            if (speed > 0f)
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            else
                transform.position = target;
            Debug.Log($"[AI] transform.position -> {transform.position}");
        }

        float x = Mathf.Clamp(transform.position.x, leftLimit, rightLimit);
        float y = Mathf.Clamp(transform.position.y, bottomLimit, topLimit);
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
