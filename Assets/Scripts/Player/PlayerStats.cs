using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxMana = 50;
    [SerializeField] private int currentMana;
    [SerializeField] private int level = 1;
    [SerializeField] private int experience = 0;
    [SerializeField] private int experienceToNextLevel = 100;

    [Header("Combat Stats")]
    [SerializeField] private int attack = 10;
    [SerializeField] private int defense = 5;
    [SerializeField] private int gold = 0;

    // Events
    public System.Action<int, int> OnHealthChanged;
    public System.Action<int, int> OnManaChanged;
    public System.Action<int> OnLevelUp;
    public System.Action<int> OnGoldChanged;
    public System.Action OnPlayerDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    #region Health Management
    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - defense, 1);
        currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    #endregion

    #region Mana Management
    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            OnManaChanged?.Invoke(currentMana, maxMana);
            return true;
        }
        return false;
    }

    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        OnManaChanged?.Invoke(currentMana, maxMana);
    }

    public void SetMaxMana(int newMaxMana)
    {
        maxMana = newMaxMana;
        currentMana = Mathf.Min(currentMana, maxMana);
        OnManaChanged?.Invoke(currentMana, maxMana);
    }
    #endregion

    #region Experience and Leveling
    public void GainExperience(int amount)
    {
        experience += amount;

        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        experience -= experienceToNextLevel;
        level++;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f);

        // Tăng stats khi lên level
        maxHealth += 10;
        maxMana += 5;
        attack += 2;
        defense += 1;

        // Hồi phục đầy máu và mana khi lên level
        currentHealth = maxHealth;
        currentMana = maxMana;

        OnLevelUp?.Invoke(level);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnManaChanged?.Invoke(currentMana, maxMana);
    }
    #endregion

    #region Gold Management
    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke(gold);
            return true;
        }
        return false;
    }
    #endregion

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        // Có thể thêm logic respawn hoặc game over ở đây
    }

    #region Getters
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetCurrentMana() => currentMana;
    public int GetMaxMana() => maxMana;
    public int GetLevel() => level;
    public int GetExperience() => experience;
    public int GetExperienceToNextLevel() => experienceToNextLevel;
    public int GetAttack() => attack;
    public int GetDefense() => defense;
    public int GetGold() => gold;
    #endregion
}
