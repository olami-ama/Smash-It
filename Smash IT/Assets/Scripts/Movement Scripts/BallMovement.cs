using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallMovement : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] public float launchSpeed = 10f;
    [SerializeField] public float speedMultiplier = 1f;

    [Header("Serve")]
    [SerializeField] public Transform servePaddle;
    [SerializeField] public float serveOffset = 0.35f;

    private Rigidbody2D rb;
    private Collider2D col;

    public bool isLaunched;
    public bool waitingForServe;

    private float minY, maxY;
    private bool boundsInitialized;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        ResetBallPhysics();
        InitializeBounds();
    }

    // -------------------------
    // PUBLIC API
    // -------------------------
    public void SetServePaddle(Transform paddle)
    {
        servePaddle = paddle;
        waitingForServe = true;
        isLaunched = false;
        ResetBallPhysics();
    }

    public void Launch()
    {
        if (!waitingForServe || servePaddle == null || isLaunched)
            return;

        float centerY = ArenaCenterY();
        Vector2 launchDir = servePaddle.position.y >= centerY ? Vector2.down : Vector2.up;

        rb.bodyType = RigidbodyType2D.Dynamic;
        col.isTrigger = false;
        rb.linearVelocity = launchDir * launchSpeed * speedMultiplier;

        waitingForServe = false;
        isLaunched = true;
    }

    // -------------------------
    // UPDATE
    // -------------------------
  public void Update()
    {
        if (waitingForServe && servePaddle != null)
        {
            float centerY = ArenaCenterY();
            float dir = servePaddle.position.y >= centerY ? -1f : 1f;

            transform.position =
                servePaddle.position + Vector3.up * serveOffset * dir;

            return;
        }

        if (!isLaunched) return;

        HandleEndlessBounds();
    }

    // -------------------------
    // COLLISIONS
    // -------------------------
  public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLaunched || collision.contactCount == 0) return;

        Vector2 normal = collision.GetContact(0).normal;
        Vector2 reflected = Vector2.Reflect(rb.linearVelocity, normal);

        if (collision.collider.CompareTag("Paddle") ||
            collision.collider.CompareTag("Paddle2"))
        {
            float offset =
                transform.position.x - collision.collider.transform.position.x;

            reflected += new Vector2(offset * 0.5f, 0f);

            float centerY = ArenaCenterY();
            float away =
                collision.collider.transform.position.y >= centerY ? -1f : 1f;

            reflected.y = Mathf.Abs(reflected.y) * away;
        }

        rb.linearVelocity =
            reflected.normalized * Mathf.Max(reflected.magnitude, launchSpeed * speedMultiplier);
    }

    // -------------------------
    // ENDLESS MODE CHECKS
    // -------------------------
 public void HandleEndlessBounds()
    {
        if (EndlessGameManager.Instance == null) return;

        if (transform.position.y >= 7.1f)
        {
            EndlessGameManager.Instance.PlayerScores();
            Destroy(gameObject);
        }
        else if (transform.position.y <= -11.1f)
        {
            EndlessGameManager.Instance.RegisterMiss();
        }
    }

    // -------------------------
    // HELPERS
    // -------------------------
   public void ResetBallPhysics()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        col.isTrigger = true;
    }

  public void InitializeBounds()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        float vert = cam.orthographicSize;
        minY = -vert;
        maxY = vert;
        boundsInitialized = true;
    }

    float ArenaCenterY()
    {
        if (!boundsInitialized) InitializeBounds();
        return (minY + maxY) * 0.5f;
    }
}



/*using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallMovement : MonoBehaviour
{
    [SerializeField] private float launchSpeed = 10f;
    private Rigidbody2D rb;
    private Collider2D col;
    public bool isLaunched = false;

    private float minX, maxX, minY, maxY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        col.isTrigger = true;

        GameObject table = GameObject.FindWithTag("Table");
        if (table != null)
        {
            SpriteRenderer sr = table.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Bounds b = sr.bounds;
                float pad = 0.5f;
                minX = b.min.x + pad; maxX = b.max.x - pad; minY = b.min.y + pad; maxY = b.max.y - pad;
                Debug.Log($"[Ball] Using Table bounds min({minX},{minY}) max({maxX},{maxY})");
            }
            else
            {
                Debug.LogWarning("[Ball] Table has no SpriteRenderer. Using camera bounds.");
                UseCameraBounds();
            }
        }
        else
        {
            Debug.LogWarning("[Ball] No GameObject with tag 'Table' found! Using camera bounds.");
            UseCameraBounds();
        }
    }

    void UseCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("[Ball] No Main Camera found — using defaults.");
            minX = -10; maxX = 10; minY = -5; maxY = 5;
            return;
        }
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;
        float pad = 0.5f;
        minX = -horzExtent + pad; maxX = horzExtent - pad; minY = -vertExtent + pad; maxY = vertExtent - pad;
        Debug.Log($"[Ball] Camera bounds min({minX},{minY}) max({maxX},{maxY})");
    }

    public void LaunchFromPaddle(Transform paddleTransform)
    {
        if (isLaunched) return;

        // compute direction
        float xOffset = transform.position.x - paddleTransform.position.x;
        Vector2 baseDir = paddleTransform.CompareTag("Paddle") ? Vector2.up : Vector2.down;
        Vector2 dir = (baseDir + new Vector2(xOffset * 0.5f, 0f)).normalized;

        // Nudge position away to avoid overlap then enable physics
        transform.position += (Vector3)(baseDir * 0.12f);

        rb.bodyType = RigidbodyType2D.Dynamic;
        col.isTrigger = false;
        rb.linearVelocity = dir * launchSpeed;
        isLaunched = true;

        Debug.Log($"[Ball] Launched from {paddleTransform.name} dir={dir} velocity={rb.linearVelocity} pos={transform.position} time={Time.time}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Ball] OnTriggerEnter2D with {other.name} tag={other.tag} isLaunched={isLaunched}");
        if (isLaunched) return;

        if (other.CompareTag("Paddle") || other.CompareTag("Paddle2"))
        {
            LaunchFromPaddle(other.transform);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Ball] OnCollisionEnter2D with {collision.collider.name} tag={collision.collider.tag} contacts={collision.contactCount}");
        if (collision.collider.CompareTag("Goal"))
        {
            if (GameManager.Instance != null) GameManager.Instance.PlayerScores(2);
        }
        else if (collision.collider.CompareTag("Goalp2"))
        {
            if (GameManager.Instance != null) GameManager.Instance.PlayerScores(1);
        }
        else if (isLaunched && collision.contactCount > 0)
        {
            Vector2 normal = collision.GetContact(0).normal;
            Vector2 reflected = Vector2.Reflect(rb.linearVelocity, normal);

            if (collision.collider.CompareTag("Paddle") || collision.collider.CompareTag("Paddle2"))
            {
                float offset = transform.position.x - collision.collider.transform.position.x;
                reflected += new Vector2(offset * 0.5f, 0f);
            }

            rb.linearVelocity = reflected.normalized * Mathf.Max(rb.linearVelocity.magnitude, launchSpeed);
            Debug.Log($"[Ball] Reflected new velocity = {rb.linearVelocity}");
        }
    }

    void Update()
    {
        if (!isLaunched) return;

        Vector3 pos = transform.position;
        Vector2 vel = rb.linearVelocity;
        bool changed = false;

        if (pos.x < minX) { pos.x = minX; vel.x = Mathf.Abs(vel.x); changed = true; }
        else if (pos.x > maxX) { pos.x = maxX; vel.x = -Mathf.Abs(vel.x); changed = true; }

        if (pos.y < minY) { pos.y = minY; vel.y = Mathf.Abs(vel.y); changed = true; }
        else if (pos.y > maxY) { pos.y = maxY; vel.y = -Mathf.Abs(vel.y); changed = true; }

        if (changed)
        {
            transform.position = pos;
            rb.linearVelocity = vel;
            Debug.Log($"[Ball] Bounced/Clamped to {pos} velocity={vel}");
        }

        float speed = rb.linearVelocity.magnitude;
        if (speed > launchSpeed * 1.5f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * launchSpeed * 1.5f;
            Debug.Log($"[Ball] Speed clamped to {rb.linearVelocity.magnitude}");
        }
    }

    public void ResetToPaddle()
    {
        isLaunched = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        col.isTrigger = true;
        Debug.Log("[Ball] ResetToPaddle called");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(maxX, minY));
        Gizmos.DrawLine(new Vector3(maxX, minY), new Vector3(maxX, maxY));
        Gizmos.DrawLine(new Vector3(maxX, maxY), new Vector3(minX, maxY));
        Gizmos.DrawLine(new Vector3(minX, maxY), new Vector3(minX, minY));
    }
}


*/