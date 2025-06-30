using UnityEngine;

/// <summary>
/// SAFE PlayerController - Version không bao giờ gây lỗi Console
/// Sử dụng file này nếu PlayerController gốc vẫn có lỗi
/// </summary>
public class SafePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeedMultiplier = 1.5f;

    [Header("Safety Settings")]
    [SerializeField] private bool enableAnimator = true;
    [SerializeField] private bool enableDebugLogs = true;

    // Components (cached)
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Input state
    private Vector2 moveInput;
    private bool isRunning;

    // Animation parameter hashes (for performance) - theo README: 4 parameters
    private int moveXHash;
    private int moveYHash;
    private int isMovingHash;
    private int speedHash;  // Fix: dùng Speed thay vì IsRunning

    // Safety flags
    private bool animatorIsValid = false;

    private void Awake()
    {
        InitializeComponents();
        CacheAnimationHashes();
        CheckAnimatorParameters();
    }

    private void InitializeComponents()
    {
        // Get or add Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // Top-down game
            rb.freezeRotation = true;
            if (enableDebugLogs) Debug.Log("✅ SafePlayerController: Added Rigidbody2D");
        }

        // Get or add SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            if (enableDebugLogs) Debug.Log("✅ SafePlayerController: Added SpriteRenderer");
        }

        // Get Animator (optional)
        animator = GetComponent<Animator>();
        if (animator == null && enableAnimator)
        {
            animator = gameObject.AddComponent<Animator>();
            if (enableDebugLogs) Debug.Log("✅ SafePlayerController: Added Animator");
        }

        // Ensure Player tag
        if (!CompareTag("Player"))
        {
            tag = "Player";
            if (enableDebugLogs) Debug.Log("✅ SafePlayerController: Set Player tag");
        }
    }

    private void CacheAnimationHashes()
    {
        // Pre-compute animation parameter hashes for performance - theo README: 4 parameters
        moveXHash = Animator.StringToHash("MoveX");
        moveYHash = Animator.StringToHash("MoveY");
        isMovingHash = Animator.StringToHash("IsMoving");
        speedHash = Animator.StringToHash("Speed");  // Fix: dùng Speed theo README
    }

    private void CheckAnimatorParameters()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            animatorIsValid = false;
            if (enableDebugLogs) Debug.Log("⚠️ SafePlayerController: No Animator Controller - animations disabled");
            return;
        }

        // Safely check if all required parameters exist - theo README: 4 parameters
        try
        {
            animator.GetBool(isMovingHash);     // IsMoving
            animator.GetFloat(moveXHash);       // MoveX
            animator.GetFloat(moveYHash);       // MoveY
            animator.GetFloat(speedHash);       // Speed

            animatorIsValid = true;
            if (enableDebugLogs) Debug.Log("✅ SafePlayerController: All 4 animator parameters found (README compliant)");
        }
        catch (System.Exception)
        {
            animatorIsValid = false;
            if (enableDebugLogs) Debug.Log("⚠️ SafePlayerController: Missing animator parameters - creating safe defaults");
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateAnimations();
        HandleSpriteFlip();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        // Get movement input (multiple input methods)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Alternative input methods if Horizontal/Vertical not working
        if (moveInput == Vector2.zero)
        {
            float h = 0f, v = 0f;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h = -1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h = 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v = -1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v = 1f;

            moveInput.x = h;
            moveInput.y = v;
        }

        // Normalize diagonal movement
        if (moveInput.magnitude > 1f)
        {
            moveInput = moveInput.normalized;
        }

        // Running input
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void HandleMovement()
    {
        if (rb == null) return;

        // Calculate current speed
        float currentSpeed = moveSpeed;
        if (isRunning && moveInput.magnitude > 0.1f)
        {
            currentSpeed *= runSpeedMultiplier;
        }

        // Apply movement
        Vector2 velocity = moveInput * currentSpeed;
        rb.linearVelocity = velocity;

        // Debug info (only when actually moving)
        if (enableDebugLogs && moveInput.magnitude > 0.1f)
        {
            Debug.Log($"🎮 SafePlayer Moving - Input: {moveInput:F2}, Speed: {currentSpeed:F1}, Running: {isRunning}");
        }
    }

    private void UpdateAnimations()
    {
        // Skip if animator not valid or disabled
        if (!animatorIsValid || !enableAnimator || animator == null)
            return;

        bool isMoving = moveInput.magnitude > 0.1f;

        // Calculate current speed
        float currentSpeed = moveSpeed;
        if (isRunning && isMoving)
        {
            currentSpeed *= runSpeedMultiplier;
        }

        // Ultra-safe parameter setting - theo README: 4 parameters
        SafeSetBool(isMovingHash, isMoving);        // IsMoving
        SafeSetFloat(moveXHash, moveInput.x);       // MoveX
        SafeSetFloat(moveYHash, moveInput.y);       // MoveY
        SafeSetFloat(speedHash, currentSpeed);      // Speed

        // Debug output cho troubleshooting
        if (enableDebugLogs && isMoving)
        {
            Debug.Log($"🎭 Animation Update: IsMoving={isMoving}, MoveX={moveInput.x:F2}, MoveY={moveInput.y:F2}, Speed={currentSpeed:F1}");
        }
    }

    private void SafeSetBool(int paramHash, bool value)
    {
        try
        {
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.SetBool(paramHash, value);
            }
        }
        catch (System.Exception)
        {
            // Silently ignore parameter errors
        }
    }

    private void SafeSetFloat(int paramHash, float value)
    {
        try
        {
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.SetFloat(paramHash, value);
            }
        }
        catch (System.Exception)
        {
            // Silently ignore parameter errors
        }
    }

    private void HandleSpriteFlip()
    {
        if (spriteRenderer == null) return;

        // Flip sprite based on horizontal movement
        if (moveInput.x > 0.1f)
        {
            spriteRenderer.flipX = false; // Moving right
        }
        else if (moveInput.x < -0.1f)
        {
            spriteRenderer.flipX = true; // Moving left
        }
    }

    // Public getters for other scripts
    public Vector2 GetMoveInput() => moveInput;
    public bool IsMoving() => moveInput.magnitude > 0.1f;
    public bool IsRunning() => isRunning && IsMoving();
    public float GetCurrentSpeed() => isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;

    // Public method to manually refresh animator parameters
    [ContextMenu("Refresh Animator Parameters")]
    public void RefreshAnimatorParameters()
    {
        CheckAnimatorParameters();
        if (enableDebugLogs)
        {
            Debug.Log($"🔄 SafePlayerController: Animator refreshed - Valid: {animatorIsValid}");
        }
    }

    // Debug info
    [ContextMenu("Show Component Status")]
    public void ShowComponentStatus()
    {
        Debug.Log($"🔍 SafePlayerController Component Status:\n" +
                  $"- Rigidbody2D: {rb != null}\n" +
                  $"- SpriteRenderer: {spriteRenderer != null}\n" +
                  $"- Animator: {animator != null}\n" +
                  $"- Animator Valid: {animatorIsValid}\n" +
                  $"- Current Input: {moveInput}\n" +
                  $"- Is Moving: {IsMoving()}\n" +
                  $"- Is Running: {IsRunning()}");
    }

    // Test method để force set IsMoving = true
    [ContextMenu("TEST: Force IsMoving True")]
    public void TestForceIsMovingTrue()
    {
        if (animator != null && animatorIsValid)
        {
            try
            {
                animator.SetBool("IsMoving", true);
                animator.SetFloat("MoveX", 1f);
                animator.SetFloat("MoveY", 0f);
                animator.SetFloat("Speed", 5f);
                Debug.Log("🧪 TEST: Forced IsMoving=true, MoveX=1, check Animator window!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ TEST Failed: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("❌ TEST Failed: Animator not valid!");
        }
    }

    // Test method để check parameters trong runtime
    [ContextMenu("TEST: Check Current Parameters")]
    public void TestCheckCurrentParameters()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            try
            {
                bool isMoving = animator.GetBool("IsMoving");
                float moveX = animator.GetFloat("MoveX");
                float moveY = animator.GetFloat("MoveY");
                float speed = animator.GetFloat("Speed");

                Debug.Log($"🔍 CURRENT PARAMETERS:\n" +
                         $"- IsMoving: {isMoving}\n" +
                         $"- MoveX: {moveX}\n" +
                         $"- MoveY: {moveY}\n" +
                         $"- Speed: {speed}\n" +
                         $"- Input Magnitude: {moveInput.magnitude}\n" +
                         $"- Should Be Moving: {moveInput.magnitude > 0.1f}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Parameter check failed: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("❌ No Animator Controller assigned!");
        }
    }
}
