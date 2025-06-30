using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Verify components
        if (rb == null)
        {
            Debug.LogError("SimpleMovement: Rigidbody2D not found! Please add Rigidbody2D component.");
        }
        else
        {
            Debug.Log("SimpleMovement: Rigidbody2D found successfully!");
        }
    }

    void Update()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        movement.y = Input.GetAxisRaw("Vertical");   // W/S or Up/Down arrows

        // Debug input
        if (movement.magnitude > 0)
        {
            Debug.Log($"Input detected - X: {movement.x}, Y: {movement.y}");
        }
    }

    void FixedUpdate()
    {
        // Move the player
        if (rb != null)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
