using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Boundaries")]
    public bool useBoundaries = false;
    public float minX, maxX, minY, maxY;

    [Header("Smoothing")]
    public bool useSmoothDamping = true;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    [Header("Look Ahead")]
    public bool useLookAhead = false;
    public float lookAheadDistance = 2f;
    public float lookAheadSpeed = 2f;

    private PlayerController playerController;

    private void Start()
    {
        // Tự động tìm player nếu target chưa được set
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                playerController = player.GetComponent<PlayerController>();
            }
        }
        else
        {
            playerController = target.GetComponent<PlayerController>();
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = CalculateTargetPosition();

        if (useSmoothDamping)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        // Apply boundaries if enabled
        if (useBoundaries)
        {
            ApplyBoundaries();
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector3 targetPos = target.position + offset;

        // Look ahead based on player movement
        if (useLookAhead && playerController != null)
        {
            Vector2 moveInput = playerController.GetMoveInput();
            if (moveInput.magnitude > 0.1f)
            {
                Vector3 lookAhead = new Vector3(moveInput.x, moveInput.y, 0) * lookAheadDistance;
                targetPos += Vector3.Lerp(Vector3.zero, lookAhead, lookAheadSpeed * Time.deltaTime);
            }
        }

        return targetPos;
    }

    private void ApplyBoundaries()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    public void SetBoundaries(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        useBoundaries = true;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            playerController = target.GetComponent<PlayerController>();
        }
    }

    // Hiệu ứng shake camera
    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(CameraShake(duration, magnitude));
    }

    private System.Collections.IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}
