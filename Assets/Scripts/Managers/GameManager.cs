using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public bool isPaused = false;

    [Header("Player Reference")]
    public GameObject player;
    public Transform playerSpawnPoint;

    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Events
    public System.Action OnGamePaused;
    public System.Action OnGameResumed;
    public System.Action OnPlayerRespawn;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create PlayerSpawn tag if it doesn't exist
            CreateRequiredTags();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void CreateRequiredTags()
    {
        // This will prevent the "PlayerSpawn is not defined" error
        Debug.Log("🔧 GameManager: Checking required tags...");

        // Use TagHelper to ensure PlayerSpawn tag exists
        if (!TagHelper.EnsureTagExists(TagHelper.PLAYER_SPAWN_TAG))
        {
            Debug.LogWarning("⚠️ GameManager: Could not create PlayerSpawn tag automatically");
            Debug.Log("💡 MANUAL FIX: Edit → Project Settings → Tags and Layers → Tags → Add 'PlayerSpawn'");
        }

        // Try to find PlayerSpawn GameObject
        FindAlternativeSpawnPoint();
    }

    private void FindAlternativeSpawnPoint()
    {
        // Fallback 1: Look for GameObject named "PlayerSpawn" or "SpawnPoint"
        GameObject spawnObject = GameObject.Find("PlayerSpawn") ?? GameObject.Find("SpawnPoint");

        if (spawnObject != null)
        {
            playerSpawnPoint = spawnObject.transform;
            Debug.Log("✅ GameManager: Found spawn point by name: " + spawnObject.name);
            return;
        }

        // Fallback 2: Use Player's current position if found
        if (player != null)
        {
            playerSpawnPoint = player.transform;
            Debug.Log("✅ GameManager: Using Player position as fallback spawn point");
            return;
        }

        // Fallback 3: Use origin
        GameObject tempSpawn = new GameObject("TempPlayerSpawn");
        tempSpawn.transform.position = Vector3.zero;
        playerSpawnPoint = tempSpawn.transform;
        Debug.Log("⚠️ GameManager: Created temporary spawn point at origin (0,0,0)");

        // Show helpful message
        Debug.Log("💡 QUICK FIX: Edit → Project Settings → Tags and Layers → Tags → Add 'PlayerSpawn'");
        Debug.Log("💡 Then create a GameObject and set its tag to 'PlayerSpawn'");
    }

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        HandleInput();
    }

    private void InitializeGame()
    {
        // Tìm player nếu chưa được assign
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Debug.Log("✅ GameManager: Found Player GameObject");
            }
            else
            {
                Debug.LogWarning("⚠️ GameManager: No GameObject with 'Player' tag found!");
            }
        }

        // Tìm spawn point nếu chưa được assign (không bắt buộc)
        if (playerSpawnPoint == null)
        {
            try
            {
                GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
                if (spawnPoint != null)
                {
                    playerSpawnPoint = spawnPoint.transform;
                    Debug.Log("✅ GameManager: Found PlayerSpawn point");
                }
                else
                {
                    Debug.Log("ℹ️ GameManager: No PlayerSpawn GameObject found (tag exists but no object)");
                    if (player != null)
                    {
                        playerSpawnPoint = player.transform;
                        Debug.Log("ℹ️ GameManager: Using Player position as spawn point");
                    }
                }
            }
            catch (UnityException ex)
            {
                Debug.LogWarning($"⚠️ GameManager: PlayerSpawn tag error: {ex.Message}");
                if (player != null)
                {
                    playerSpawnPoint = player.transform;
                    Debug.Log("🔧 GameManager: Using Player position as fallback spawn point");
                }
            }
        }

        // Subscribe to player death event
        if (player != null)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.OnPlayerDeath += HandlePlayerDeath;
            }
        }

        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("🎯 GameManager: Game initialized successfully!");
    }

    private void HandleInput()
    {
        // ESC để pause/unpause game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();
    }

    private void HandlePlayerDeath()
    {
        // Respawn player sau 2 giây
        Invoke(nameof(RespawnPlayer), 2f);
    }

    public void RespawnPlayer()
    {
        if (player != null && playerSpawnPoint != null)
        {
            player.transform.position = playerSpawnPoint.position;

            // Reset player stats
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.Heal(playerStats.GetMaxHealth());
                playerStats.RestoreMana(playerStats.GetMaxMana());
            }

            OnPlayerRespawn?.Invoke();
        }
    }

    public void SaveGame()
    {
        // Implement save functionality
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);

        if (player != null)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                PlayerPrefs.SetInt("PlayerLevel", stats.GetLevel());
                PlayerPrefs.SetInt("PlayerHealth", stats.GetCurrentHealth());
                PlayerPrefs.SetInt("PlayerMana", stats.GetCurrentMana());
                PlayerPrefs.SetInt("PlayerGold", stats.GetGold());
                PlayerPrefs.SetInt("PlayerExp", stats.GetExperience());
            }
        }

        PlayerPrefs.Save();
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            player.transform.position = new Vector3(x, y, 0);

            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                // Load stats would require more complex implementation
                // This is a simplified version
                Debug.Log("Game Loaded!");
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
