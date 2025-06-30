using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeedMultiplier = 1.5f;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Input
    private Vector2 moveInput;
    private bool isRunning;
    private bool wasMoving;

    // Animation parameters (theo hướng dẫn README: 4 parameters)
    private readonly int moveXHash = Animator.StringToHash("MoveX");
    private readonly int moveYHash = Animator.StringToHash("MoveY");
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");  // Hash ID để performance tốt
    private readonly int speedHash = Animator.StringToHash("Speed");

    // Current animation state (for debugging and clarity)
    private bool currentIsMoving = false;  // Giá trị bool thực tế của IsMoving

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Immediate fix for common setup issues
        FixCommonIssues();
    }

    private void FixCommonIssues()
    {
        // Fix 1: Ensure Rigidbody2D is properly configured
        if (rb != null)
        {
            rb.gravityScale = 0f;           // Top-down game
            rb.freezeRotation = true;       // Prevent spinning
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Debug.Log("✅ PlayerController: Fixed Rigidbody2D settings");
        }

        // Fix 2: Handle Animator issues
        if (animator != null)
        {
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogWarning("⚠️ PlayerController: No Animator Controller assigned, disabling Animator");
                animator.enabled = false;
            }
            else
            {
                Debug.Log("✅ PlayerController: Animator Controller found");
                // Simply check if controller exists - don't access parameters
                Debug.Log("✅ PlayerController: Animator Controller is valid");
            }
        }

        // Fix 3: Ensure Player tag
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
            Debug.Log("✅ PlayerController: Set GameObject tag to 'Player'");
        }
    }

    private void Start()
    {
        // Setup Rigidbody2D for top-down movement
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            Debug.Log("✅ PlayerController: Rigidbody2D configured for top-down movement");
        }

        // Tạm thời disable Animator để tránh lỗi
        if (animator != null && animator.runtimeAnimatorController == null)
        {
            animator.enabled = false;
            Debug.Log("⚠️ PlayerController: Animator disabled (no controller assigned)");
        }

        // Debug components
        Debug.Log($"PlayerController Start - rb: {rb != null}, animator: {animator != null && animator.enabled}, spriteRenderer: {spriteRenderer != null}");
        Debug.Log("🎮 Use WASD or Arrow Keys to move the player!");
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        HandleInput();

        // Chỉ update animations nếu animator đã được setup đúng cách
        if (animator != null && animator.enabled && animator.runtimeAnimatorController != null)
        {
            UpdateAnimations();
        }

        HandleSpriteFlipping();
    }

    private void HandleInput()
    {
        // Input System mới (nếu có)
        if (Keyboard.current != null)
        {
            Vector2 input = Vector2.zero;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                input.y = 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                input.y = -1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                input.x = -1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                input.x = 1;

            moveInput = input.normalized;
            isRunning = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        }
        else
        {
            // Fallback to old Input Manager
            float horizontal = Input.GetAxisRaw("Horizontal"); // A/D hoặc Left/Right arrows
            float vertical = Input.GetAxisRaw("Vertical");     // W/S hoặc Up/Down arrows

            moveInput = new Vector2(horizontal, vertical).normalized;
            isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }

        // 🎯 DEBUG INPUT CHO IsMoving (chỉ khi có thay đổi):
        // - Khi đứng yên: moveInput = (0,0), magnitude = 0
        // - Khi bấm WASD: moveInput != (0,0), magnitude > 0
        bool currentlyMoving = moveInput.magnitude > 0.1f;

        // Chỉ log khi trạng thái thay đổi để tránh spam
        if (currentlyMoving != wasMoving)
        {
            if (currentlyMoving)
            {
                Debug.Log($"🎮 STARTED MOVING - Input: {moveInput}, Magnitude: {moveInput.magnitude:F3}");
            }
            else
            {
                Debug.Log($"⏸️ STOPPED MOVING - Input: {moveInput}, Magnitude: {moveInput.magnitude:F3}");
            }
            wasMoving = currentlyMoving;
        }
    }

    // Input System callbacks (sẽ override HandleInput nếu được sử dụng)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    private void HandleMovement()
    {
        if (rb == null) return;

        float currentSpeed = moveSpeed;
        if (isRunning)
        {
            currentSpeed *= runSpeedMultiplier;
        }

        Vector2 movement = moveInput * currentSpeed;
        rb.linearVelocity = movement;

        // Debug để kiểm tra input và movement
        if (moveInput.magnitude > 0.1f)
        {
            Debug.Log($"Input: {moveInput}, Speed: {currentSpeed}, Velocity: {rb.linearVelocity}");
        }
    }

    private void UpdateAnimations()
    {
        // Triple check để đảm bảo animator ready và không gây lỗi
        if (animator == null || !animator.enabled || animator.runtimeAnimatorController == null)
        {
            return;
        }

        // 🎯 LOGIC CHÍNH XÁC CHO IsMoving:
        // - Khi đứng yên (không bấm phím): IsMoving = false
        // - Khi bấm WASD (có input): IsMoving = true
        bool isMoving = moveInput.magnitude > 0.1f;
        currentIsMoving = isMoving;  // Track current state

        // Debug animation values mỗi frame để kiểm tra
        Debug.Log($"🎮 ANIMATION UPDATE - Input: {moveInput} | Magnitude: {moveInput.magnitude:F3} | IsMoving: {isMoving} | Hash: {isMovingHash}");

        // Ultra-safe parameter setting - theo README: 4 parameters
        try
        {
            // Set movement parameters (theo README) - USE HASH FOR PERFORMANCE
            SetAnimatorParameterSafe(isMovingHash, isMoving);
            SetAnimatorParameterSafe(moveXHash, moveInput.x);
            SetAnimatorParameterSafe(moveYHash, moveInput.y);

            // Calculate current speed - chỉ khi đang moving
            float currentSpeed = isMoving ? moveSpeed : 0f;
            if (isRunning && isMoving)
            {
                currentSpeed *= runSpeedMultiplier;
            }
            SetAnimatorParameterSafe(speedHash, currentSpeed);

            // Debug animation state mỗi frame để kiểm tra
            Debug.Log($"🎬 Animation - IsMoving: {isMoving}, Speed: {currentSpeed}, MoveX: {moveInput.x:F2}, MoveY: {moveInput.y:F2}");
            Debug.Log($"🎯 SPEED TRANSITION - Speed: {currentSpeed} (>0 = Walk, <1 = Idle)");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ Animation parameter error: {e.Message}");
            Debug.LogWarning("💡 Disabling Animator to prevent further errors. Check Animator Controller parameters.");
            animator.enabled = false; // Disable to prevent further errors
        }
    }

    // Ultra-safe parameter setting method - handle 4 parameters theo README
    private void SetAnimatorParameterSafe(string parameterName, object value)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        try
        {
            // Handle 4 parameters theo README: MoveX, MoveY, IsMoving, Speed
            switch (parameterName)
            {
                case "IsMoving":
                    if (value is bool boolValue)
                    {
                        // Test if parameter exists first
                        animator.GetBool(parameterName);
                        animator.SetBool(parameterName, boolValue);
                        Debug.Log($"✅ Set {parameterName} = {boolValue}");
                    }
                    break;
                case "MoveX":
                case "MoveY":
                case "Speed":
                    if (value is float floatValue)
                    {
                        // Test if parameter exists first
                        animator.GetFloat(parameterName);
                        animator.SetFloat(parameterName, floatValue);
                        Debug.Log($"✅ Set {parameterName} = {floatValue:F2}");
                    }
                    break;
            }
        }
        catch (System.Exception e)
        {
            // Parameter doesn't exist - warn user
            Debug.LogError($"❌ Parameter '{parameterName}' không tồn tại! Lỗi: {e.Message}");
            Debug.LogError($"💡 Hãy chạy 'Tools → RPG Tools → 🚀 ULTIMATE ANIMATOR FIX' để tạo parameters!");
        }
    }

    // Overload method for HASH-based parameter setting (FASTER PERFORMANCE)
    private void SetAnimatorParameterSafe(int parameterHash, object value)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        try
        {
            // Determine parameter type by hash
            if (parameterHash == isMovingHash && value is bool boolValue)
            {
                animator.SetBool(parameterHash, boolValue);
                Debug.Log($"✅ Set IsMoving (hash) = {boolValue}");
            }
            else if ((parameterHash == moveXHash || parameterHash == moveYHash || parameterHash == speedHash) && value is float floatValue)
            {
                animator.SetFloat(parameterHash, floatValue);
                string paramName = parameterHash == moveXHash ? "MoveX" :
                                 parameterHash == moveYHash ? "MoveY" : "Speed";
                Debug.Log($"✅ Set {paramName} (hash) = {floatValue:F2}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Parameter hash {parameterHash} không tồn tại! Lỗi: {e.Message}");
            Debug.LogError($"💡 Hãy chạy 'Tools → RPG Tools → 🚀 ULTIMATE ANIMATOR FIX' để tạo parameters!");
        }
    }

    private void HandleSpriteFlipping()
    {
        if (spriteRenderer == null) return;

        // Flip sprite dựa trên hướng di chuyển
        if (moveInput.x > 0.1f)
        {
            // Di chuyển sang phải - sprite gốc
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.1f)
        {
            // Di chuyển sang trái - flip sprite
            spriteRenderer.flipX = true;
        }
        // Không flip khi di chuyển lên/xuống để giữ hướng hiện tại
    }

    // Helper method to check if parameter exists (simplified version)
    private bool HasParameter(Animator animator, string paramName)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return false;

        // Safe way to check parameters - chỉ check 3 parameters cơ bản
        try
        {
            switch (paramName)
            {
                case "IsMoving":
                    animator.SetBool(paramName, animator.GetBool(paramName));
                    return true;
                case "MoveX":
                case "MoveY":
                    animator.SetFloat(paramName, animator.GetFloat(paramName));
                    return true;
                default:
                    return false;
            }
        }
        catch
        {
            return false;
        }
    }

    // Getter methods cho các script khác
    public Vector2 GetMoveInput() => moveInput;
    public bool IsMoving() => moveInput.magnitude > 0.1f;
    public bool IsRunning() => isRunning;
    public bool GetCurrentIsMoving() => currentIsMoving;  // Get current IsMoving state
    public int GetIsMovingHash() => isMovingHash;  // Get the hash ID for debugging

    #region Animation Events (Fix for Animation Event errors)

    /// <summary>
    /// Animation Event function for footstep sounds
    /// Called automatically by Animation Events in Walk animations
    /// </summary>
    public void OnFootstep()
    {
        // Placeholder for footstep sound effect
        // AudioSource.PlayOneShot(footstepSound);
        Debug.Log("🦶 Footstep sound event");
    }

    /// <summary>
    /// Animation Event function for idle breathing
    /// Called automatically by Animation Events in Idle animations
    /// </summary>
    public void OnIdle()
    {
        // Placeholder for idle breathing effect
        Debug.Log("😴 Idle breathing event");
    }

    /// <summary>
    /// Generic Animation Event function
    /// Use this if Animation Events have no specific function
    /// </summary>
    public void OnAnimationEvent()
    {
        // Generic animation event handler
        Debug.Log("🎬 Generic animation event");
    }

    /// <summary>
    /// Animation Event for walk cycle start
    /// </summary>
    public void OnWalkStart()
    {
        Debug.Log("🚶 Walk animation started");
    }

    /// <summary>
    /// Animation Event for walk cycle end
    /// </summary>
    public void OnWalkEnd()
    {
        Debug.Log("🛑 Walk animation ended");
    }

    #endregion

    #region DEBUG METHODS - FORCE TEST IsMoving

    [System.Obsolete("Debug method only")]
    public void ForceTestIsMoving()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("❌ NO ANIMATOR CONTROLLER! Run Ultimate Animator Fix first!");
            return;
        }

        Debug.Log("🧪 FORCE TESTING IsMoving parameter...");

        // Test 1: Force set to true
        try
        {
            animator.SetBool("IsMoving", true);
            bool result = animator.GetBool("IsMoving");
            Debug.Log($"🧪 Test 1 - Set IsMoving=true, Got: {result}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Test 1 failed: {e.Message}");
        }

        // Test 2: Force set to false  
        try
        {
            animator.SetBool("IsMoving", false);
            bool result = animator.GetBool("IsMoving");
            Debug.Log($"🧪 Test 2 - Set IsMoving=false, Got: {result}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Test 2 failed: {e.Message}");
        }
    }

    // Call this from Console: player.GetComponent<PlayerController>().ForceTestIsMoving()

    #endregion
}
