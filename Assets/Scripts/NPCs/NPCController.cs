using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("NPC Settings")]
    public string npcName = "NPC";
    [TextArea(3, 5)]
    public string[] dialogueLines;
    public bool isQuestGiver = false;
    public bool isShopkeeper = false;

    [Header("Movement Settings")]
    public bool canMove = false;
    public float moveSpeed = 2f;
    public float moveRange = 3f;
    public float waitTime = 2f;

    [Header("Interaction")]
    public float interactionRange = 2f;
    public KeyCode interactionKey = KeyCode.E;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float waitTimer = 0f;
    private bool isPlayerInRange = false;
    private GameObject player;

    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Animation hashes
    private readonly int moveXHash = Animator.StringToHash("MoveX");
    private readonly int moveYHash = Animator.StringToHash("MoveY");
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPosition = transform.position;
        targetPosition = startPosition;

        // Tìm player
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        CheckPlayerDistance();
        HandleMovement();
        HandleInteraction();
    }

    private void CheckPlayerDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        isPlayerInRange = distance <= interactionRange;

        // Có thể hiển thị interaction prompt ở đây
        if (isPlayerInRange)
        {
            // Show interaction indicator
            // Ví dụ: ShowInteractionPrompt(true);
        }
        else
        {
            // Hide interaction indicator
            // Ví dụ: ShowInteractionPrompt(false);
        }
    }

    private void HandleMovement()
    {
        if (!canMove) return;

        if (isMoving)
        {
            // Di chuyển đến target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Cập nhật animation
            Vector3 direction = (targetPosition - transform.position).normalized;
            UpdateMovementAnimation(direction);

            // Kiểm tra đã đến target chưa
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                waitTimer = waitTime;
                UpdateMovementAnimation(Vector3.zero);
            }
        }
        else
        {
            // Đang đợi
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                SetNewRandomTarget();
            }
        }
    }

    private void SetNewRandomTarget()
    {
        // Tạo target position ngẫu nhiên trong phạm vi di chuyển
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(1f, moveRange);

        targetPosition = startPosition + randomDirection * randomDistance;
        isMoving = true;
    }

    private void UpdateMovementAnimation(Vector3 direction)
    {
        if (animator == null) return;

        bool moving = direction.magnitude > 0.1f;
        animator.SetBool(isMovingHash, moving);

        if (moving)
        {
            animator.SetFloat(moveXHash, direction.x);
            animator.SetFloat(moveYHash, direction.y);
        }
    }

    private void HandleInteraction()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (isQuestGiver)
        {
            // Handle quest interaction
            HandleQuestInteraction();
        }
        else if (isShopkeeper)
        {
            // Handle shop interaction
            HandleShopInteraction();
        }
        else
        {
            // Handle normal dialogue
            HandleDialogue();
        }
    }

    private void HandleDialogue()
    {
        if (dialogueLines.Length > 0)
        {
            // Tìm dialogue manager và hiển thị dialogue - Updated for Unity 2023+
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(npcName, dialogueLines);
            }
            else
            {
                // Fallback - log to console
                Debug.Log($"{npcName}: {dialogueLines[0]}");
            }
        }
    }

    private void HandleQuestInteraction()
    {
        // Implement quest logic
        Debug.Log($"{npcName} has a quest for you!");
        HandleDialogue();
    }

    private void HandleShopInteraction()
    {
        // Implement shop logic
        Debug.Log($"Welcome to {npcName}'s shop!");

        // Có thể mở shop UI ở đây
        // ShopManager.Instance.OpenShop(this);
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ interaction range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        // Vẽ movement range nếu có thể di chuyển
        if (canMove)
        {
            Gizmos.color = Color.green;
            Vector3 center = Application.isPlaying ? startPosition : transform.position;
            Gizmos.DrawWireSphere(center, moveRange);
        }
    }
}
