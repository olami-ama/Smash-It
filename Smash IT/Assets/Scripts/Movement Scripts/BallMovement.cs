using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallMovement : MonoBehaviour
{
    [SerializeField] private float launchSpeed = 10f;
    public float speedMultiplier = 1f;

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
                
            }
            else
            {
                
                UseCameraBounds();
            }
        }
        else
        {

            UseCameraBounds();
        }
    }
   

    void UseCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            
            minX = -10; maxX = 10; minY = -5; maxY = 5;
            return;
        }
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;
        float pad = 0.5f;
        minX = -horzExtent + pad; maxX = horzExtent - pad; minY = -vertExtent + pad; maxY = vertExtent - pad;
       
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
        rb.linearVelocity = dir * launchSpeed * speedMultiplier;
        isLaunched = true;

        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (isLaunched) return;

        if (other.CompareTag("Paddle") || other.CompareTag("Paddle2"))
        {
            LaunchFromPaddle(other.transform);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
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

            rb.linearVelocity = reflected.normalized * Mathf.Max(rb.linearVelocity.magnitude, launchSpeed * speedMultiplier);
           
        }
    }

    void Update()
    {
       // if (GameManager.Instance.IsGameOver()) return;  // stop movement
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
            
        }
        // after
        float speed = rb.linearVelocity.magnitude;
        float clampLimit = launchSpeed * speedMultiplier * 1.5f;
        if (speed > clampLimit)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * clampLimit;
            
        }
    }

    public void ResetToPaddle()
    {
        isLaunched = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        col.isTrigger = true;
        
    }
    // draws a box to show the ball constraint
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(maxX, minY));
        Gizmos.DrawLine(new Vector3(maxX, minY), new Vector3(maxX, maxY));
        Gizmos.DrawLine(new Vector3(maxX, maxY), new Vector3(minX, maxY));
        Gizmos.DrawLine(new Vector3(minX, maxY), new Vector3(minX, minY));
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