using UnityEngine;  

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallMovement : MonoBehaviour
{
    private float launchForce = 10f;     // Speed/force to launch the ball
    private Rigidbody2D rb;              // Reference to Rigidbody2D for movement
    private Collider2D col;              // Reference to Collider2D for collisions
    public bool isLaunched = false;      // Track whether the ball is currently in play

    // Table/camera boundaries
    private float minX, maxX, minY, maxY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();      // Get Rigidbody2D component
        col = GetComponent<Collider2D>();      // Get Collider2D component

        rb.bodyType = RigidbodyType2D.Kinematic; // Start ball as Kinematic (not moving)
        col.isTrigger = true;                   // Collider set as trigger until launch

        // Find the table by its tag
        GameObject table = GameObject.FindWithTag("Table");

        // If a table object is found
        if (table != null)
        {
            SpriteRenderer sr = table.GetComponent<SpriteRenderer>(); // Get its SpriteRenderer

            if (sr != null) // If table has a SpriteRenderer
            {
                Bounds b = sr.bounds;   // Get the size/bounds of the table
                minX = b.min.x + 0.5f;  // Set left boundary (with small padding)
                maxX = b.max.x - 0.5f;  // Set right boundary (with small padding)
                minY = b.min.y + 0.5f;  // Set bottom boundary
                maxY = b.max.y - 0.5f;  // Set top boundary
            }
            else
            {
                Debug.LogWarning(" Table has no SpriteRenderer. Using camera bounds.");
                UseCameraBounds(); // Fallback to camera bounds
            }
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'Table' found! Using camera bounds.");
            UseCameraBounds(); // Fallback if no table is found
        }
    }

    // Use the main camera’s orthographic size to define boundaries
    void UseCameraBounds()
    {
        Camera cam = Camera.main;                // Get main camera
        float vertExtent = cam.orthographicSize; // Camera vertical size
        float horzExtent = vertExtent * cam.aspect; // Camera horizontal size

        minX = -horzExtent + 0.5f; // Left boundary
        maxX = horzExtent - 0.5f;  // Right boundary
        minY = -vertExtent + 0.5f; // Bottom boundary
        maxY = vertExtent - 0.5f;  // Top boundary
    }

    // Launch the ball from a paddle
    public void LaunchFromPaddle(Transform paddleTransform)
    {
        if (isLaunched) return; // Don’t launch if already moving

        isLaunched = true;             // Mark ball as launched
        rb.bodyType = RigidbodyType2D.Dynamic; // Enable physics
        col.isTrigger = false;         // Allow collisions

        // Horizontal offset between ball and paddle
        float xOffset = transform.position.x - paddleTransform.position.x;

        // Decide launch direction: up for Paddle1, down for Paddle2
        Vector2 baseDir = paddleTransform.CompareTag("Paddle") ? Vector2.up : Vector2.down;

        // Add a small horizontal offset for variety
        Vector2 dir = (baseDir + new Vector2(xOffset, 0)).normalized;

        // Apply velocity to launch the ball
        rb.linearVelocity = dir * launchForce;
    }

    // Trigger detection (used for initial paddle hit)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLaunched) return; // Ignore if already launched

        // If ball touches either paddle
        if (other.CompareTag("Paddle") || other.CompareTag("Paddle2"))
        {
            LaunchFromPaddle(other.transform); // Launch from that paddle
        }
    }

    // Handle collisions with goals
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If ball enters Player 1’s goal
        if (collision.collider.CompareTag("Goal"))
        {
            if (GameManager.Instance != null)   // Check GameManager exists
                GameManager.Instance.PlayerScores(2); // Award score to Player 2
        }
        // If ball enters Player 2’s goal
        else if (collision.collider.CompareTag("Goalp2"))
        {
            if (GameManager.Instance != null)   // Check GameManager exists
                GameManager.Instance.PlayerScores(1); // Award score to Player 1
        }
    }

    void Update()
    {
        if (!isLaunched) return; // Do nothing if ball isn’t moving

        Vector3 pos = transform.position; // Current ball position

        // Check horizontal boundaries
        if (pos.x < minX || pos.x > maxX)
        {
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y); // Bounce horizontally
        }

        // Check vertical boundaries
        if (pos.y < minY || pos.y > maxY)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y); // Bounce vertically
        }
    }

    // Draw boundaries in Scene view for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Set gizmo color to green
        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(maxX, minY)); // Bottom line
        Gizmos.DrawLine(new Vector3(maxX, minY), new Vector3(maxX, maxY)); // Right line
        Gizmos.DrawLine(new Vector3(maxX, maxY), new Vector3(minX, maxY)); // Top line
        Gizmos.DrawLine(new Vector3(minX, maxY), new Vector3(minX, minY)); // Left line
    }
}



