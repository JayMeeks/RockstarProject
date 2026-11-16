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
        //Singleton setup
        //Ensures only one can ever exist
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
    }

    private void Start()
    {
        //Set default health
        //After awake loop
        SetCurrentHealth(GetMaxHealth());
    }

    //Used by other scripts that want to damage the player
    //If the player has sufficient health, returns true and sets spent hp to all damage
    //If the player has the exact health, returns true and sets spent hp to all damage but 1
    //If false, returns false and sets spent hp to 0
    //This function does not influence the player health at all
    //Scripts concerned with this function will then damage the player with the out spentHpValue
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
    
    //Damages player and updates UI
    public void DamagePlayer(int damage)
    { 
        currentHealth -= damage; 
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    //Heals player and updates UI
    //Does not allow player to be healed beyond their max HP
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
    
    public void SetCurrentHealth(int currentHealth)
    {
        this.currentHealth = currentHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
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
