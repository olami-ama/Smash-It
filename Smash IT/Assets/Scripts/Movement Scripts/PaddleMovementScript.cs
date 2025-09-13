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

        foreach (Touch touch in Input.touches) 
        {
            // Only respond to touches on your half of the screen
            if (isBottomPaddle && touch.position.y > Screen.height / 2) continue; // checks if it the bottom or top screen to get touches from the phone
            if (!isBottomPaddle && touch.position.y < Screen.height / 2) continue; // checks if it the bottom or top screen to get touches from the phone

            // Convert touch to world position (correct z handling)
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane)); //
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





/*using UnityEngine;

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

