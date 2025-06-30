using UnityEngine;

/// <summary>
/// Script setup nhanh Player GameObject với các components cần thiết
/// Chỉ cần attach script này vào Player và nó sẽ tự setup mọi thứ
/// </summary>
public class QuickPlayerSetup : MonoBehaviour
{
    [Header("Auto Setup Options")]
    [SerializeField] private bool setupOnStart = true;
    [SerializeField] private bool addMissingComponents = true;
    [SerializeField] private bool configureComponents = true;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    void Start()
    {
        if (setupOnStart)
        {
            SetupPlayer();
        }
    }

    [ContextMenu("Setup Player Now")]
    public void SetupPlayer()
    {
        Debug.Log("🔧 QuickPlayerSetup: Starting Player setup...");

        // Ensure Player tag
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
            Debug.Log("✅ Set GameObject tag to 'Player'");
        }

        if (addMissingComponents)
        {
            AddMissingComponents();
        }

        if (configureComponents)
        {
            ConfigureComponents();
        }

        Debug.Log("🎯 QuickPlayerSetup: Player setup complete!");
        Debug.Log("📝 Use WASD keys to move the player.");
    }

    private void AddMissingComponents()
    {
        // Add SpriteRenderer if missing
        if (GetComponent<SpriteRenderer>() == null)
        {
            var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            Debug.Log("✅ Added SpriteRenderer component");
        }

        // Add Rigidbody2D if missing
        if (GetComponent<Rigidbody2D>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            Debug.Log("✅ Added Rigidbody2D component");
        }

        // Add Collider if missing
        if (GetComponent<Collider2D>() == null)
        {
            var collider = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("✅ Added BoxCollider2D component");
        }

        // Add SimpleMovement if missing
        if (GetComponent<SimpleMovement>() == null)
        {
            var movement = gameObject.AddComponent<SimpleMovement>();
            Debug.Log("✅ Added SimpleMovement component");
        }
    }

    private void ConfigureComponents()
    {
        // Configure Rigidbody2D for top-down
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Debug.Log("✅ Configured Rigidbody2D for top-down movement");
        }

        // Configure Collider
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(0.8f, 0.8f);
            Debug.Log("✅ Configured BoxCollider2D size");
        }

        // Configure SimpleMovement
        var movement = GetComponent<SimpleMovement>();
        if (movement != null)
        {
            // Use reflection to set moveSpeed if it's public
            var field = movement.GetType().GetField("moveSpeed");
            if (field != null)
            {
                field.SetValue(movement, moveSpeed);
                Debug.Log($"✅ Set movement speed to {moveSpeed}");
            }
        }

        // Disable Animator if it exists and has no controller
        var animator = GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController == null)
        {
            animator.enabled = false;
            Debug.Log("✅ Disabled Animator (no controller assigned)");
        }

        // Disable other scripts that might cause errors
        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
            Debug.Log("✅ Disabled PlayerController (to prevent conflicts)");
        }
    }

    [ContextMenu("Reset Player Setup")]
    public void ResetPlayerSetup()
    {
        // Remove this script's added components
        var components = new System.Type[]
        {
            typeof(SimpleMovement),
            typeof(BoxCollider2D),
            typeof(Rigidbody2D)
        };

        foreach (var componentType in components)
        {
            var component = GetComponent(componentType);
            if (component != null)
            {
                DestroyImmediate(component);
                Debug.Log($"🗑️ Removed {componentType.Name}");
            }
        }

        Debug.Log("🔄 Player setup reset complete");
    }
}
