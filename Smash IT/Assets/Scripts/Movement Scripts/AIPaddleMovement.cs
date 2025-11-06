using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIPaddleMovement : MonoBehaviour
{
    [Header("Ball (assign or tag as 'Ball')")]
    public Transform ball;
    public Rigidbody2D ballRb;
    public BallMovement ballMovement;

    [Header("AI Settings")]
    public float moveSpeed = 6f;            // used when using MoveTowards style movement
    public float serveDistance = 1.5f;      // how close to ball to trigger serve
    public float smoothness = 3f;           // lerp factor for follow (used to compute target)
    public bool usePrediction = false;      // set true to use simple linear prediction

    [Header("Movement Bounds")]
    public float leftLimit = -7f, rightLimit = 7f;
    public float bottomLimit = -4f, topLimit = 4f;

    [Header("Debug")]
    public bool debugVerbose = false;

    private Rigidbody2D aiRb;
    private Vector2 physicsTargetPosition;  // target used in FixedUpdate
    private bool loggedFoundBall = false;
    // private bool lastServingState = false;

    private float reactionDelay = 0.5f;
    private float nextMoveTime = 0f;


    void Awake()
    {
        aiRb = GetComponent<Rigidbody2D>();
        if (aiRb == null)
            aiRb = gameObject.AddComponent<Rigidbody2D>();

        // Prefer kinematic for AI that directly sets positions
        if (aiRb.bodyType != RigidbodyType2D.Kinematic)
            aiRb.bodyType = RigidbodyType2D.Kinematic;

        physicsTargetPosition = aiRb.position;
    }

    void Update()
    {
        if (Time.time < nextMoveTime) return; // wait for delay before moving again

        //  avoid NullReference if GameManager not set up
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        // lazy find ball if needed
        if (ball == null || ballRb == null)
        {
            GameObject b = GameObject.FindWithTag("Ball");
            if (b != null)
            {
                ball = b.transform;
                ballRb = b.GetComponent<Rigidbody2D>();
                if (ballMovement == null) ballMovement = b.GetComponent<BallMovement>();
                if (!loggedFoundBall)
                {
                    loggedFoundBall = true;
                    if (debugVerbose) Debug.Log($"[AI] Found Ball: {ball.name}");
                }
            }
            else
            {
                // no ball found nothing to do
                return;
            }
        }

        // If the ball isn't launched, move under it to serve
        if (ballMovement != null && !ballMovement.isLaunched)
        {
            // align x with ball (clamped)
            float targetX = Mathf.Clamp(ball.position.x, leftLimit, rightLimit);
            Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);

            // compute a smoothed follow target (so AI doesn't teleport)
            Vector3 smoothed = Vector3.Lerp(transform.position, targetPos, smoothness * Time.deltaTime);
            physicsTargetPosition = ClampToBounds(smoothed);

            // decide whether to serve (distance based on x and y)
            float dist = Vector2.Distance(new Vector2(ball.position.x, ball.position.y), aiRb.position);
            if (dist < serveDistance)
            {
                if (debugVerbose) Debug.Log($"[AI] Serving ball from {name} (dist {dist:F2})");
                ballMovement.LaunchFromPaddle(transform);
                nextMoveTime = Time.time + reactionDelay; // prevent immediate follow
            }

            return;
        }

        // If ballMovement exists and ball was launched, attempt to follow when ball is moving toward paddle
        if (ballRb == null) return;

        Vector2 v = ballRb.linearVelocity;
        if (v.sqrMagnitude < 0.01f)
        {
            // ball is essentially stationary  keep current physicsTargetPosition
            return;
        }

        Vector2 toPaddle = (Vector2)aiRb.position - (Vector2)ball.position;
        bool ballComingTowardMe = Vector2.Dot(v, toPaddle) > 0f;

        // Skip following if the ball is not coming toward AI or too close already
        if (!ballComingTowardMe || Vector2.Distance(ball.position, aiRb.position) < 1.0f)
            return;



        // Compute follow target Optionally predict where the ball will be at this paddle Y (simple linear).
        float targetXFollow = ball.position.x;
        if (usePrediction)
        {
            // Simple linear prediction ignoring collisions 
            float deltaY = aiRb.position.y - ball.position.y;
            if (Mathf.Abs(v.y) > 0.001f)
            {
                float t = deltaY / v.y; // time until ball is at paddle Y 
                if (t > 0f)
                {
                    targetXFollow = ball.position.x + v.x * t;
                }
            }
        }

        targetXFollow = Mathf.Clamp(targetXFollow, leftLimit, rightLimit);
        Vector3 followPos = new Vector3(targetXFollow, transform.position.y, transform.position.z);
        // smooth the movement toward followPos result used by physics in FixedUpdate
        Vector3 newPos = Vector3.Lerp(transform.position, followPos, smoothness * Time.deltaTime);
        physicsTargetPosition = ClampToBounds(newPos);
    }

    void FixedUpdate()
    {
        // Apply movement using Rigidbody2D.MovePosition (physics safe)
        if (aiRb == null) return;

        // Move toward physicsTargetPosition at moveSpeed, but if physicsTargetPosition is same as aiRb.position
        // MovePosition with same pos is fine Use MoveTowards so moveSpeed is respected
        Vector2 current = aiRb.position;
        Vector2 target = physicsTargetPosition;
        float maxDelta = moveSpeed * Time.fixedDeltaTime;

        Vector2 nextPos = Vector2.MoveTowards(current, target, maxDelta);
        aiRb.MovePosition(nextPos);

        //  low verbosity logging only log when there's a noticeable jump
        if (debugVerbose && (nextPos - current).sqrMagnitude > 0.0001f)
        {
            Debug.Log($"[AI] MovePosition -> {nextPos} (target {target})");
        }
    }

    // Ensures a Vector3 is inside the configured bounds (keeps z)
    private Vector3 ClampToBounds(Vector3 pos)
    {
        float x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        float y = Mathf.Clamp(pos.y, bottomLimit, topLimit);
        return new Vector3(x, y, pos.z);
    }
}

