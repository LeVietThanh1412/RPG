using UnityEngine;

/// <summary>
/// Simple movement script for testing - no animations, just pure movement
/// Use this if you want to test movement immediately without any setup
/// </summary>
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Simple Movement Test")]
    public float moveSpeed = 5f;
    public bool useArrowKeys = true;
    public bool useWASD = true;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get or add required components
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Configure for top-down movement
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Set player tag
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
        }

        Debug.Log("🎮 SimplePlayerMovement: Ready! Use WASD or Arrow Keys to move");
    }

    void Update()
    {
        Vector2 moveInput = Vector2.zero;

        // WASD Input
        if (useWASD)
        {
            if (Input.GetKey(KeyCode.W)) moveInput.y = 1;
            if (Input.GetKey(KeyCode.S)) moveInput.y = -1;
            if (Input.GetKey(KeyCode.A)) moveInput.x = -1;
            if (Input.GetKey(KeyCode.D)) moveInput.x = 1;
        }

        // Arrow Keys Input
        if (useArrowKeys)
        {
            if (Input.GetKey(KeyCode.UpArrow)) moveInput.y = 1;
            if (Input.GetKey(KeyCode.DownArrow)) moveInput.y = -1;
            if (Input.GetKey(KeyCode.LeftArrow)) moveInput.x = -1;
            if (Input.GetKey(KeyCode.RightArrow)) moveInput.x = 1;
        }

        // Normalize diagonal movement
        moveInput = moveInput.normalized;

        // Apply movement
        rb.linearVelocity = moveInput * moveSpeed;

        // Simple sprite flipping
        if (spriteRenderer != null && moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }

        // Debug movement
        if (moveInput.magnitude > 0)
        {
            Debug.Log($"Moving: {moveInput} at speed {moveSpeed}");
        }
    }

    void OnEnable()
    {
        Debug.Log("✅ SimplePlayerMovement enabled - ready to move!");
    }

    void OnDisable()
    {
        Debug.Log("⚠️ SimplePlayerMovement disabled");
    }
}
