using UnityEngine;

/// <summary>
/// Simple alternative player controller
/// Use this if PlayerController has issues
/// </summary>
public class SimplePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runSpeedMultiplier = 1.5f;

    [Header("Input Settings")]
    public bool useWASD = true;
    public bool useArrowKeys = true;
    public KeyCode runKey = KeyCode.LeftShift;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private bool isRunning;

    void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Auto-add components if missing
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // Top-down game
            Debug.Log("✅ SimplePlayerController: Added Rigidbody2D");
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            Debug.Log("✅ SimplePlayerController: Added SpriteRenderer");
        }

        // Ensure Player tag
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
            Debug.Log("✅ SimplePlayerController: Set Player tag");
        }

        Debug.Log("🎮 SimplePlayerController: Ready! Use WASD or Arrow keys to move");
    }

    void Update()
    {
        GetInput();
        HandleSpriteFlip();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void GetInput()
    {
        movement.x = 0;
        movement.y = 0;

        // Get movement input
        if (useWASD)
        {
            if (Input.GetKey(KeyCode.W)) movement.y += 1;
            if (Input.GetKey(KeyCode.S)) movement.y -= 1;
            if (Input.GetKey(KeyCode.A)) movement.x -= 1;
            if (Input.GetKey(KeyCode.D)) movement.x += 1;
        }

        if (useArrowKeys)
        {
            if (Input.GetKey(KeyCode.UpArrow)) movement.y += 1;
            if (Input.GetKey(KeyCode.DownArrow)) movement.y -= 1;
            if (Input.GetKey(KeyCode.LeftArrow)) movement.x -= 1;
            if (Input.GetKey(KeyCode.RightArrow)) movement.x += 1;
        }

        // Get run input
        isRunning = Input.GetKey(runKey);

        // Normalize diagonal movement
        if (movement.magnitude > 1)
        {
            movement = movement.normalized;
        }
    }

    void MovePlayer()
    {
        if (rb == null) return;

        // Calculate speed
        float currentSpeed = moveSpeed;
        if (isRunning)
        {
            currentSpeed *= runSpeedMultiplier;
        }

        // Move player
        Vector2 newPosition = rb.position + movement * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    void HandleSpriteFlip()
    {
        if (spriteRenderer == null) return;

        // Flip sprite based on movement direction
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false; // Moving right
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // Moving left
        }
    }

    // Public getters for other scripts
    public Vector2 GetMovementInput() => movement;
    public bool IsMoving() => movement.magnitude > 0.1f;
    public bool IsRunning() => isRunning && IsMoving();
}
