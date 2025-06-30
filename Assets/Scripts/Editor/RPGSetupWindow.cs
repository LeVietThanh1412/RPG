using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[System.Serializable]
public class RPGSetupWindow : EditorWindow
{
    [MenuItem("RPG Framework/Setup Game")]
    public static void ShowWindow()
    {
        GetWindow<RPGSetupWindow>("RPG Game Setup");
    }

    private GameObject playerPrefab;
    private Sprite playerSprite;

    private void OnGUI()
    {
        GUILayout.Label("RPG Game Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Player Setup", EditorStyles.boldLabel);
        playerSprite = (Sprite)EditorGUILayout.ObjectField("Player Sprite", playerSprite, typeof(Sprite), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Create Player"))
        {
            CreatePlayer();
        }

        if (GUILayout.Button("Setup Camera"))
        {
            SetupCamera();
        }

        if (GUILayout.Button("Create Game Manager"))
        {
            CreateGameManager();
        }

        if (GUILayout.Button("Setup Complete Scene"))
        {
            SetupCompleteScene();
        }

        GUILayout.Space(20);
        GUILayout.Label("This will create a complete RPG scene with:", EditorStyles.helpBox);
        GUILayout.Label("• Player with all components");
        GUILayout.Label("• Camera with follow script");
        GUILayout.Label("• Game Manager");
        GUILayout.Label("• UI Canvas with basic elements");
    }
    private void CreatePlayer()
    {
        // Check if player already exists
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
        {
            Debug.LogWarning("Player already exists in scene!");
            Selection.activeGameObject = existingPlayer;
            return;
        }

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = Vector3.zero;

        // Add components safely
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = player.AddComponent<SpriteRenderer>();
        }
        if (playerSprite != null)
        {
            sr.sprite = playerSprite;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }

        BoxCollider2D col = player.GetComponent<BoxCollider2D>();
        if (col == null)
        {
            col = player.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 0.8f);
        }

        // Add player scripts safely
        if (player.GetComponent<PlayerController>() == null)
            player.AddComponent<PlayerController>();
        if (player.GetComponent<PlayerStats>() == null)
            player.AddComponent<PlayerStats>();
        if (player.GetComponent<PlayerInventory>() == null)
            player.AddComponent<PlayerInventory>();
        if (player.GetComponent<PlayerEquipment>() == null)
            player.AddComponent<PlayerEquipment>();

        // Create animator controller
        Animator animator = player.GetComponent<Animator>();
        if (animator == null)
        {
            animator = player.AddComponent<Animator>();
        }

        Selection.activeGameObject = player;
        Debug.Log("Player created successfully!");
    }

    private void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }

        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 5;

        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
        }

        // Find and assign player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            cameraFollow.target = player.transform;
        }

        Debug.Log("Camera setup completed!");
    }

    private void CreateGameManager()
    {
        GameObject gameManager = new GameObject("Game Manager");
        gameManager.AddComponent<GameManager>();

        Debug.Log("Game Manager created!");
    }

    private void SetupCompleteScene()
    {
        CreatePlayer();
        SetupCamera();
        CreateGameManager();
        CreateUI();

        Debug.Log("Complete scene setup finished!");
    }

    private void CreateUI()
    {
        // Create Canvas
        GameObject canvas = new GameObject("Canvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Add UI Manager
        canvas.AddComponent<UIManager>();

        Debug.Log("UI Canvas created!");
    }
}
#endif
