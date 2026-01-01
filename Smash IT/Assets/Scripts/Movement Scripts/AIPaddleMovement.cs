using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIPaddleMovement : MonoBehaviour
{
    [Header("Ball References")]
    public Transform ballTransform;
    public Rigidbody2D ballRb;
    private BallMovement ballMovement;

    [Header("AI Settings")]
    public float moveSpeed = 6f;
    public float serveDistance = 1.5f;
    public float smoothness = 3f;
    public bool usePrediction = false;

    [Header("Movement Bounds")]
    public float leftLimit = -7f;
    public float rightLimit = 7f;
    public float bottomLimit = -4f;
    public float topLimit = 4f;

    [Header("Debug")]
    public bool debugVerbose = false;

    private Rigidbody2D aiRb;
    private Vector2 physicsTargetPosition;
    private bool loggedFoundBall = false;

   // private float reactionDelay = 0.5f;
   // private float nextMoveTime = 0f;

    void Awake()
    {
        aiRb = GetComponent<Rigidbody2D>();
        aiRb.bodyType = RigidbodyType2D.Kinematic;
        physicsTargetPosition = aiRb.position;
    }

  
        void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
                return;

            FindBallIfNeeded();

            if (ballTransform == null || ballRb == null)
                return;

            HandleFollowMovement();
        }


    void FixedUpdate()
    {
        Vector2 current = aiRb.position;
        float maxDelta = moveSpeed * Time.fixedDeltaTime;
        Vector2 nextPos = Vector2.MoveTowards(current, physicsTargetPosition, maxDelta);
        aiRb.MovePosition(nextPos);
    }

    // -------------------------
    // BALL FINDING
    // -------------------------
    void FindBallIfNeeded()
    {
        if (ballTransform != null && ballRb != null && ballMovement != null)
            return;

        GameObject b = GameObject.FindWithTag("Ball");
        if (b == null) return;

        ballTransform = b.transform;
        ballRb = b.GetComponent<Rigidbody2D>();
        ballMovement = b.GetComponent<BallMovement>();

        if (!loggedFoundBall && debugVerbose)
        {
            loggedFoundBall = true;
            Debug.Log($"[AI] Found Ball: {b.name}");
        }
    }

   

    // -------------------------
    // FOLLOW LOGIC
    // -------------------------
    void HandleFollowMovement()
    {
        if (ballRb == null) return;

        Vector2 v = ballRb.linearVelocity;
        if (v.sqrMagnitude < 0.01f) return;

        Vector2 toPaddle = aiRb.position - (Vector2)ballTransform.position;
        bool ballComingTowardMe = Vector2.Dot(v, toPaddle) > 0f;

        if (!ballComingTowardMe) return;

        float targetX = ballTransform.position.x;

        if (usePrediction && Mathf.Abs(v.y) > 0.001f)
        {
            float deltaY = aiRb.position.y - ballTransform.position.y;
            float t = deltaY / v.y;
            if (t > 0f)
                targetX = ballTransform.position.x + v.x * t;
        }

        targetX = Mathf.Clamp(targetX, leftLimit, rightLimit);
        Vector3 followPos = new Vector3(targetX, transform.position.y, transform.position.z);
        Vector3 smoothed = Vector3.Lerp(transform.position, followPos, smoothness * Time.deltaTime);
        physicsTargetPosition = ClampToBounds(smoothed);
    }

    // -------------------------
    // HELPERS
    // -------------------------
    Vector3 ClampToBounds(Vector3 pos)
    {
        float x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        float y = Mathf.Clamp(pos.y, bottomLimit, topLimit);
        return new Vector3(x, y, pos.z);
    }
}
