using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Health Bar")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    [Header("Mana Bar")]
    public Slider manaBar;
    public TextMeshProUGUI manaText;

    [Header("Experience Bar")]
    public Slider expBar;
    public TextMeshProUGUI levelText;

    [Header("Gold Display")]
    public TextMeshProUGUI goldText;

    [Header("Inventory UI")]
    public GameObject inventoryPanel;
    public Transform inventoryGrid;
    public GameObject inventorySlotPrefab;

    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button saveButton;
    public Button loadButton;
    public Button quitButton;

    private PlayerStats playerStats;
    private PlayerInventory playerInventory;
    private InventorySlotUI[] inventorySlots;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeUI();
        SetupEventListeners();
    }

    private void Update()
    {
        HandleUIInput();
    }

    private void InitializeUI()
    {
        // Tìm player components
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            playerInventory = player.GetComponent<PlayerInventory>();
        }

        // Initialize inventory UI
        CreateInventorySlots();

        // Hide panels initially
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        // Setup button listeners
        if (resumeButton != null) resumeButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());
        if (saveButton != null) saveButton.onClick.AddListener(() => GameManager.Instance.SaveGame());
        if (loadButton != null) loadButton.onClick.AddListener(() => GameManager.Instance.LoadGame());
        if (quitButton != null) quitButton.onClick.AddListener(() => GameManager.Instance.QuitGame());
    }

    private void SetupEventListeners()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged += UpdateHealthBar;
            playerStats.OnManaChanged += UpdateManaBar;
            playerStats.OnLevelUp += UpdateLevelDisplay;
            playerStats.OnGoldChanged += UpdateGoldDisplay;
        }

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateInventorySlot;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGamePaused += ShowPauseMenu;
            GameManager.Instance.OnGameResumed += HidePauseMenu;
        }
    }

    private void HandleUIInput()
    {
        // Toggle inventory với phím I
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    #region UI Updates
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }

    private void UpdateManaBar(int currentMana, int maxMana)
    {
        if (manaBar != null)
        {
            manaBar.maxValue = maxMana;
            manaBar.value = currentMana;
        }

        if (manaText != null)
        {
            manaText.text = $"{currentMana}/{maxMana}";
        }
    }

    private void UpdateLevelDisplay(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }

        if (expBar != null && playerStats != null)
        {
            expBar.maxValue = playerStats.GetExperienceToNextLevel();
            expBar.value = playerStats.GetExperience();
        }
    }

    private void UpdateGoldDisplay(int gold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {gold}";
        }
    }
    #endregion

    #region Inventory UI
    private void CreateInventorySlots()
    {
        if (playerInventory == null || inventoryGrid == null || inventorySlotPrefab == null)
            return;

        int inventorySize = playerInventory.GetInventorySize();
        inventorySlots = new InventorySlotUI[inventorySize];

        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryGrid);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                slotUI.SetSlotIndex(i);
                inventorySlots[i] = slotUI;
            }
        }
    }

    private void UpdateInventorySlot(int slotIndex)
    {
        if (inventorySlots != null && slotIndex >= 0 && slotIndex < inventorySlots.Length)
        {
            InventorySlot slot = playerInventory.GetSlot(slotIndex);
            inventorySlots[slotIndex].UpdateSlotDisplay(slot);
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);

            if (isActive)
            {
                RefreshInventoryDisplay();
            }
        }
    }

    private void RefreshInventoryDisplay()
    {
        if (playerInventory == null || inventorySlots == null) return;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = playerInventory.GetSlot(i);
            inventorySlots[i].UpdateSlotDisplay(slot);
        }
    }
    #endregion

    #region Pause Menu
    private void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }

    private void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    #endregion

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateHealthBar;
            playerStats.OnManaChanged -= UpdateManaBar;
            playerStats.OnLevelUp -= UpdateLevelDisplay;
            playerStats.OnGoldChanged -= UpdateGoldDisplay;
        }

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateInventorySlot;
        }
    }
}
