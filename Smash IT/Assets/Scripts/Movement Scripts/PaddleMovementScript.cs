using UnityEngine;

public class PaddleMovementScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f; // how fast the paddle moves

    [Header("References")]
    [SerializeField] private Transform midline; // assign a Midline object in the Inspector

    // Keys for keyboard input (set depending on paddle type)
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

    private bool isBottomPaddle;  // true = bottom paddle, false = top paddle
    private float zDistance;      // distance from camera to paddle (for ScreenToWorldPoint)

    private Camera cam;
    private SpriteRenderer sr;

    void Start()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();

        // --- Identify paddle by tag ---
        if (CompareTag("Paddle")) // bottom paddle
        {
            upKey = KeyCode.W; downKey = KeyCode.S; leftKey = KeyCode.A; rightKey = KeyCode.D;
            isBottomPaddle = true;
        }
        else if (CompareTag("Paddle2")) // top paddle
        {
            upKey = KeyCode.UpArrow; downKey = KeyCode.DownArrow; leftKey = KeyCode.LeftArrow; rightKey = KeyCode.RightArrow;
            isBottomPaddle = false;
        }
        else
        {
            // fallback if no tag is set
            upKey = KeyCode.W; downKey = KeyCode.S; leftKey = KeyCode.A; rightKey = KeyCode.D;
            isBottomPaddle = true;
            Debug.LogWarning($"[PaddleMovement] {name} missing Paddle/Paddle2 tag. Defaulting to bottom paddle.");
        }

        // Distance to camera
        if (cam != null)
            zDistance = Mathf.Abs(cam.transform.position.z - transform.position.z);
        else
            zDistance = 10f;
    }

    void Update()
    {
        if (cam == null || midline == null) return; // safety check

        Vector3 desiredPos = transform.position;

        // ------------------------------
        // Keyboard  Input
        // ------------------------------
        Vector3 keyboardMove = Vector3.zero;
        if (Input.GetKey(upKey)) keyboardMove += Vector3.up;
        if (Input.GetKey(downKey)) keyboardMove += Vector3.down;
        if (Input.GetKey(leftKey)) keyboardMove += Vector3.left;
        if (Input.GetKey(rightKey)) keyboardMove += Vector3.right;

        // ------------------------------
        // Touch Or mouse input
        // ------------------------------
        bool hasTouchTarget = false;
        Vector3 touchTarget = Vector3.zero;

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (isBottomPaddle && touch.position.y > Screen.height / 2f) continue;
                if (!isBottomPaddle && touch.position.y < Screen.height / 2f) continue;
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) continue;

                Vector3 world = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, zDistance));
                world.z = transform.position.z;

                touchTarget = world;
                hasTouchTarget = true;
                break; // take first valid touch
            }
        }
        else if (Input.GetMouseButton(0)) // mouse fallback (for editor)
        {
            Vector3 mousePos = Input.mousePosition;
            if ((isBottomPaddle && mousePos.y <= Screen.height / 2f) || (!isBottomPaddle && mousePos.y >= Screen.height / 2f))
            {
                Vector3 world = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDistance));
                world.z = transform.position.z;

                touchTarget = world;
                hasTouchTarget = true;
            }
        }

        // ------------------------------
        // Apply midline 
        // ------------------------------
        if (hasTouchTarget)
        {
            desiredPos = Vector3.MoveTowards(transform.position, touchTarget, moveSpeed * Time.deltaTime);
        }
        else if (keyboardMove != Vector3.zero)
        {
            desiredPos += keyboardMove.normalized * moveSpeed * Time.deltaTime;
        }

        // ------------------------------
        // Clamp To screen + Midline
        // ------------------------------
        Vector3 screenBottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 screenTopRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDistance));

        float halfW = sr != null ? sr.bounds.extents.x : 0.5f;
        float halfH = sr != null ? sr.bounds.extents.y : 0.5f;

        // Clamp horizontally
        desiredPos.x = Mathf.Clamp(desiredPos.x, screenBottomLeft.x + halfW, screenTopRight.x - halfW);

        // Clamp vertically using Midline GameObject
        float midY = midline.position.y; // ← your custom divider object
        if (isBottomPaddle)
        {
            float lowerBound = screenBottomLeft.y + halfH;
            float upperBound = midY - halfH;
            desiredPos.y = Mathf.Clamp(desiredPos.y, lowerBound, upperBound);
        }
        else
        {
            float lowerBound = midY + halfH;
            float upperBound = screenTopRight.y - halfH;
            desiredPos.y = Mathf.Clamp(desiredPos.y, lowerBound, upperBound);
        }

        // Apply new position
        transform.position = desiredPos;

        // ------------------------------
        // Debug Midline  (visible in Game View)
        // ------------------------------
        Debug.DrawLine(
            new Vector3(screenBottomLeft.x, midY, transform.position.z),
            new Vector3(screenTopRight.x, midY, transform.position.z),
            Color.red
        );
    }
}

/*using UnityEngine;

public class PaddleMovementScript : MonoBehaviour
{
    public float moveSpeed = 15f; // Paddle speed

    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

    private bool isBottomPaddle;
    private float zDistance;

    void Start()
    {
        // Decide which paddle this is based on tag
        if (CompareTag("Paddle")) // bottom paddle
        {
            upKey = KeyCode.W; downKey = KeyCode.S; leftKey = KeyCode.A; rightKey = KeyCode.D;
            isBottomPaddle = true;
        }
        else if (CompareTag("Paddle2")) // top paddle
        {
            upKey = KeyCode.UpArrow; downKey = KeyCode.DownArrow; leftKey = KeyCode.LeftArrow; rightKey = KeyCode.RightArrow;
            isBottomPaddle = false;
        }
        else
        {
            // fallback keys if tag is wrong
            upKey = KeyCode.W; downKey = KeyCode.S; leftKey = KeyCode.A; rightKey = KeyCode.D;
            isBottomPaddle = true;
            Debug.LogWarning($"[PaddleMovement] {name} missing Paddle/Paddle2 tag. Defaulting to bottom keys.");
        }

        // Distance from camera so ScreenToWorldPoint works correctly
        if (Camera.main != null)
            zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        else
            zDistance = 10f;

        Debug.Log($"[PaddleMovement] {name} Start - isBottom={isBottomPaddle} zDistance={zDistance}");
    }

    void Update()
    {
        // Stop if game is over
        if (GameManager.Instance.IsGameOver()) return;
        if (Camera.main == null) return;

        Vector3 keyboardMove = Vector3.zero;
        Vector3 desiredPos = transform.position;

        // Keyboard input
        if (Input.GetKey(upKey)) keyboardMove += Vector3.up;
        if (Input.GetKey(downKey)) keyboardMove += Vector3.down;
        if (Input.GetKey(leftKey)) keyboardMove += Vector3.left;
        if (Input.GetKey(rightKey)) keyboardMove += Vector3.right;

        // Screen bounds
        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDistance));
        float midY = (screenBottomLeft.y + screenTopRight.y) * 0.5f;

        // Paddle size (for clamping)
        SpriteRenderer sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        float halfW = sr != null ? sr.bounds.extents.x : 0.5f;
        float halfH = sr != null ? sr.bounds.extents.y : 0.5f;

        bool hasTouchTarget = false;
        Vector3 touchTarget = Vector3.zero;

        // Touch input
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (isBottomPaddle && touch.position.y > Screen.height / 2f) continue;
                if (!isBottomPaddle && touch.position.y < Screen.height / 2f) continue;
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) continue;

                Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, zDistance));
                world.z = transform.position.z;
                touchTarget = world;
                hasTouchTarget = true;
                break;
            }
        }
        else
        {
            // Mouse as fallback
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                if ((isBottomPaddle && mousePos.y <= Screen.height / 2f) || (!isBottomPaddle && mousePos.y >= Screen.height / 2f))
                {
                    Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDistance));
                    world.z = transform.position.z;
                    touchTarget = world;
                    hasTouchTarget = true;
                }
            }
        }

        // Move logic
        if (hasTouchTarget)
        {
            desiredPos = Vector3.MoveTowards(transform.position, touchTarget, moveSpeed * Time.deltaTime);
        }
        else if (keyboardMove != Vector3.zero)
        {
            desiredPos += keyboardMove.normalized * moveSpeed * Time.deltaTime;
        }

        // Clamp X
        desiredPos.x = Mathf.Clamp(desiredPos.x, screenBottomLeft.x + halfW, screenTopRight.x - halfW);

        // Clamp Y
        if (isBottomPaddle)
        {
            float lowerBound = screenBottomLeft.y + halfH;
            float upperBound = midY - halfH;
            desiredPos.y = Mathf.Clamp(desiredPos.y, lowerBound, upperBound);
        }
        else
        {
            float lowerBound = midY + halfH;
            float upperBound = screenTopRight.y - halfH;
            desiredPos.y = Mathf.Clamp(desiredPos.y, lowerBound, upperBound);
        }

        // Apply final position
        if ((Vector2)transform.position != (Vector2)desiredPos)
        {
            transform.position = desiredPos;
        }
    }
}
*/