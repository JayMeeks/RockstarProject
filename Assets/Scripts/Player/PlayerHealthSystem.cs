using System;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health")]
    private int maxHealth = 100;
    public int currentHealth;
    
    private static PlayerHealthSystem _instance; 
    public static PlayerHealthSystem Instance {get{return _instance;}}
    
    public static event Action<int,int> OnHealthChanged;

  


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetCurrentHealth(GetMaxHealth());
        }
    }
    
    
    
    public bool TrySpendHealth(int tryHpValue, out int spentHpValue)
    {
        if (GetCurrentHealth() > tryHpValue)
        {
            spentHpValue = tryHpValue;
            return true;
        }
        else if (GetCurrentHealth() == tryHpValue)
        {
            spentHpValue = tryHpValue - 1;
            return true;
        }
        else
        {
            spentHpValue = 0;
            return false;
        }
    }
    
    
    public void SetCurrentHealth(int currentHealth)
    {
        this.currentHealth = currentHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void DamagePlayer(int damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void HealPlayer(int heal)
    {
        if (currentHealth + heal >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += heal;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetMaxHealth()
    {
        return maxHealth;

    }
}
