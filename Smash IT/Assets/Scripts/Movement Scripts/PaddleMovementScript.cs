using UnityEngine;

public class PaddleMovementScript : MonoBehaviour
{
    public float moveSpeed = 15f; // Paddle speed

    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

    private bool isBottomPaddle;
    private float zDistance;

    // debug states to avoid spamming every frame
    private bool prevHadTouch = false;
    private bool prevHadKeyboard = false;

    void Start()
    {
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
            upKey = KeyCode.W; downKey = KeyCode.S; leftKey = KeyCode.A; rightKey = KeyCode.D;
            isBottomPaddle = true;
            Debug.LogWarning($"[PaddleMovement] {name} missing Paddle/Paddle2 tag. Defaulting to bottom keys.");
        }

        if (Camera.main != null)
            zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        else
            zDistance = 10f;

        Debug.Log($"[PaddleMovement] {name} Start - isBottom={isBottomPaddle} zDistance={zDistance}");
    }

    void Update()
    {

          if (GameManager.Instance.IsGameOver()) return;  // stop movement
                                                            
       
        if (Camera.main == null) return;

        Vector3 keyboardMove = Vector3.zero;
        Vector3 desiredPos = transform.position;

        if (Input.GetKey(upKey)) keyboardMove += Vector3.up;
        if (Input.GetKey(downKey)) keyboardMove += Vector3.down;
        if (Input.GetKey(leftKey)) keyboardMove += Vector3.left;
        if (Input.GetKey(rightKey)) keyboardMove += Vector3.right;

        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDistance));
        float midY = (screenBottomLeft.y + screenTopRight.y) * 0.5f;

        SpriteRenderer sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        float halfW = sr != null ? sr.bounds.extents.x : 0.5f;
        float halfH = sr != null ? sr.bounds.extents.y : 0.5f;

        bool hasTouchTarget = false;
        Vector3 touchTarget = Vector3.zero;

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

                if (!prevHadTouch)
                    Debug.Log($"[PaddleMovement] {name} Touch started at screen {touch.position} -> world {world}");
                prevHadTouch = true;
                break;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                if ((isBottomPaddle && mousePos.y <= Screen.height / 2f) || (!isBottomPaddle && mousePos.y >= Screen.height / 2f))
                {
                    Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDistance));
                    world.z = transform.position.z;
                    touchTarget = world;
                    hasTouchTarget = true;

                    if (!prevHadTouch)
                        Debug.Log($"[PaddleMovement] {name} Mouse touch at {mousePos} -> world {world}");
                    prevHadTouch = true;
                }
                else prevHadTouch = false;
            }
            else
            {
                prevHadTouch = false;
            }
        }

        if (hasTouchTarget)
        {
            desiredPos = Vector3.MoveTowards(transform.position, touchTarget, moveSpeed * Time.deltaTime);
        }
        else if (keyboardMove != Vector3.zero)
        {
            if (!prevHadKeyboard)
                Debug.Log($"[PaddleMovement] {name} Keyboard input started (dir {keyboardMove})");
            prevHadKeyboard = true;
            desiredPos += keyboardMove.normalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            prevHadKeyboard = false;
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

        // If clamped changed position from previous, log it once
        if ((Vector2)transform.position != (Vector2)desiredPos)
        {
            Debug.Log($"[PaddleMovement] {name} Moving to {desiredPos}");
            transform.position = desiredPos;
        }
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

    // debug states to avoid spamming every frame
    private bool prevHadTouch = false;
    private bool prevHadKeyboard = false;

    void Start()
    {
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
            upKey = KeyCode.W; downKey = KeyCode.S; leftKey = KeyCode.A; rightKey = KeyCode.D;
            isBottomPaddle = true;
            Debug.LogWarning($"[PaddleMovement] {name} missing Paddle/Paddle2 tag. Defaulting to bottom keys.");
        }

        if (Camera.main != null)
            zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        else
            zDistance = 10f;

        Debug.Log($"[PaddleMovement] {name} Start - isBottom={isBottomPaddle} zDistance={zDistance}");
    }

    void Update()
    {
        if (Camera.main == null) return;

        Vector3 keyboardMove = Vector3.zero;
        Vector3 desiredPos = transform.position;

        if (Input.GetKey(upKey)) keyboardMove += Vector3.up;
        if (Input.GetKey(downKey)) keyboardMove += Vector3.down;
        if (Input.GetKey(leftKey)) keyboardMove += Vector3.left;
        if (Input.GetKey(rightKey)) keyboardMove += Vector3.right;

        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDistance));
        float midY = (screenBottomLeft.y + screenTopRight.y) * 0.5f;

        SpriteRenderer sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        float halfW = sr != null ? sr.bounds.extents.x : 0.5f;
        float halfH = sr != null ? sr.bounds.extents.y : 0.5f;

        bool hasTouchTarget = false;
        Vector3 touchTarget = Vector3.zero;

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

                if (!prevHadTouch)
                    Debug.Log($"[PaddleMovement] {name} Touch started at screen {touch.position} -> world {world}");
                prevHadTouch = true;
                break;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                if ((isBottomPaddle && mousePos.y <= Screen.height / 2f) || (!isBottomPaddle && mousePos.y >= Screen.height / 2f))
                {
                    Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDistance));
                    world.z = transform.position.z;
                    touchTarget = world;
                    hasTouchTarget = true;

                    if (!prevHadTouch)
                        Debug.Log($"[PaddleMovement] {name} Mouse touch at {mousePos} -> world {world}");
                    prevHadTouch = true;
                }
                else prevHadTouch = false;
            }
            else
            {
                prevHadTouch = false;
            }
        }

        if (hasTouchTarget)
        {
            desiredPos = Vector3.MoveTowards(transform.position, touchTarget, moveSpeed * Time.deltaTime);
        }
        else if (keyboardMove != Vector3.zero)
        {
            if (!prevHadKeyboard)
                Debug.Log($"[PaddleMovement] {name} Keyboard input started (dir {keyboardMove})");
            prevHadKeyboard = true;
            desiredPos += keyboardMove.normalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            prevHadKeyboard = false;
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

        // If clamped changed position from previous, log it once
        if ((Vector2)transform.position != (Vector2)desiredPos)
        {
            Debug.Log($"[PaddleMovement] {name} Moving to {desiredPos}");
            transform.position = desiredPos;
        }
    }
}




using UnityEngine;

public class PaddleMovementScript : MonoBehaviour
{
    public float moveSpeed = 15f;

    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

    private bool isBottomPaddle; //  checks if it the bottom paddle
    private float zDistance; // checks how far paddle is from the camera

    void Start()
    {
        // Assign keys for desktop
        if (CompareTag("Paddle")) // Bottom Paddle
        {
            upKey = KeyCode.W;
            downKey = KeyCode.S;
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            isBottomPaddle = true;
        }
        else if (CompareTag("Paddle2")) // Top Paddle
        {
            upKey = KeyCode.UpArrow;
            downKey = KeyCode.DownArrow;
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            isBottomPaddle = false;
        }

        zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z); // stores the distance of the paddle from the camera 
    }

    void Update()
    {
        Vector3 moveDirection = Vector3.zero;

        // for Desktop input
        if (Input.GetKey(upKey)) moveDirection += Vector3.up;  
        if (Input.GetKey(downKey)) moveDirection += Vector3.down; 
        if (Input.GetKey(leftKey)) moveDirection += Vector3.left; 
        if (Input.GetKey(rightKey)) moveDirection += Vector3.right;  

        // for Mobile input
      
        foreach (Touch touch in Input.touches) //
        {
            // Only respond to touches on your half of the screen
            if (isBottomPaddle && touch.position.y > Screen.height / 2) continue; // checks if it the bottom or top screen to get touches from the phone
            if (!isBottomPaddle && touch.position.y < Screen.height / 2) continue; // checks if it the bottom or top screen to get touches from the phone

            // Convert touch to world position (correct z handling)
            Vector3 touchPos = Camera.main.ScreenToWorldPoint( new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane)); //
            touchPos.z = transform.position.z; // makes sure it does not fly away in the Z direction

            
            moveDirection = (touchPos - transform.position).normalized; // Move towards finger
        }


        // Apply movement
        if (moveDirection != Vector3.zero)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime); // Apply movement
        }

        //  Clamp inside screen + midline lock
        Vector3 pos = transform.position; // gets the position of the paddle
        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, zDistance)); // gets the bottom edge of the screen
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDistance)); //gets the top edge of the screen

        float halfW = GetComponent<SpriteRenderer>().bounds.extents.x; // gets the paddle width so it does not freeze half way
        float halfH = GetComponent<SpriteRenderer>().bounds.extents.y; //gets the paddle height so it does not freeze half way

        pos.x = Mathf.Clamp(pos.x, screenBottomLeft.x + halfW, screenTopRight.x - halfW); // keeps paddle in screen X

        if (isBottomPaddle)
        {
            
            pos.y = Mathf.Clamp(pos.y, screenBottomLeft.y + halfH, 0 - halfH); // Bottom paddle can only move in bottom half
        }
        else
        {
            
            pos.y = Mathf.Clamp(pos.y, 0 + halfH, screenTopRight.y - halfH); // Top paddle can only move in top half
        }

        transform.position = pos; // put the paddle back into a safe place
    }
}
*/

