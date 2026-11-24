using System;
using System.Collections;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health")]
    private int maxHealth = 100;
    public int currentHealth;

    private static PlayerHealthSystem _instance;
    public static PlayerHealthSystem Instance { get { return _instance; } }

    public static event Action<int, int> OnHealthChanged;

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

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
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

    public bool TryHealOverTime(int healAmount, float duration)
    {
        if (healAmount <= 0 || currentHealth >= maxHealth)
            return false;

        StartCoroutine(HealOverTime(healAmount, duration));
        return true;
    }



    private IEnumerator HealOverTime(int healAmount, float duration)
    {
        if (duration <= 0f)
        {
            HealPlayer(healAmount);
            yield break;
        }

        float elapsed = 0f;
        int amountApplied = 0;

        while (elapsed < duration && currentHealth < maxHealth)
        {
            elapsed = Mathf.Min(
                elapsed + Time.deltaTime,
                duration
            );

            int targetAmount = Mathf.FloorToInt(
                healAmount * (elapsed / duration)
            );

            int amountToHeal = targetAmount - amountApplied;

            if (amountToHeal > 0)
            {
                HealPlayer(amountToHeal);
                amountApplied = targetAmount;

                if (currentHealth >= maxHealth)
                {
                    yield break;
                }
            }

            yield return null;
        }
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
