using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemData itemToPickup;
    public int quantity = 1;

    [Header("Pickup Settings")]
    public float pickupRange = 1.5f;
    public bool autoPickup = false;
    public KeyCode pickupKey = KeyCode.F;

    [Header("Visual Effects")]
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;
    public GameObject pickupEffect;

    private Vector3 startPosition;
    private bool canPickup = false;
    private GameObject player;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set sprite from item data
        if (itemToPickup != null && itemToPickup.icon != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = itemToPickup.icon;
        }

        // Find player
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // Bob animation
        BobAnimation();

        // Check pickup
        CheckPickup();
    }

    private void BobAnimation()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void CheckPickup()
    {
        if (player == null || itemToPickup == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        canPickup = distance <= pickupRange;

        if (canPickup)
        {
            if (autoPickup)
            {
                PickupItem();
            }
            else if (Input.GetKeyDown(pickupKey))
            {
                PickupItem();
            }
        }
    }

    private void PickupItem()
    {
        PlayerInventory playerInventory = player.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            if (playerInventory.AddItem(itemToPickup, quantity))
            {
                // Successfully picked up
                OnItemPickedUp();
            }
            else
            {
                // Inventory full
                Debug.Log("Inventory is full!");
            }
        }
    }

    private void OnItemPickedUp()
    {
        // Spawn pickup effect
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        // Log pickup message
        Debug.Log($"Picked up {quantity}x {itemToPickup.itemName}");

        // Destroy the pickup object
        Destroy(gameObject);
    }

    // Method để tạo item pickup từ code
    public static void CreateItemPickup(ItemData item, int quantity, Vector3 position)
    {
        GameObject pickupPrefab = Resources.Load<GameObject>("Prefabs/ItemPickup");
        if (pickupPrefab == null)
        {
            // Tạo pickup object đơn giản nếu không có prefab
            GameObject pickup = new GameObject($"{item.itemName} Pickup");
            pickup.transform.position = position;

            // Add components
            pickup.AddComponent<SpriteRenderer>();
            pickup.AddComponent<CircleCollider2D>().isTrigger = true;

            ItemPickup pickupScript = pickup.AddComponent<ItemPickup>();
            pickupScript.itemToPickup = item;
            pickupScript.quantity = quantity;
        }
        else
        {
            GameObject pickup = Instantiate(pickupPrefab, position, Quaternion.identity);
            ItemPickup pickupScript = pickup.GetComponent<ItemPickup>();
            if (pickupScript != null)
            {
                pickupScript.itemToPickup = item;
                pickupScript.quantity = quantity;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw pickup range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (autoPickup && other.CompareTag("Player"))
        {
            player = other.gameObject;
            PickupItem();
        }
    }
}
