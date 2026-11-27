using System;
using UnityEngine;
using UnityEngine.UI;

public class HollowData : EnemyInfo
{
    [SerializeField] private HollowStats _stats;

    private void Start()
    {
        Initialize();
    }



    protected override void Initialize()
    {
        MaxHealth = _stats.maxHealth;
        CurrentHealth = MaxHealth;
        
        AddBloodValue(_stats.healthOnKill);
    }

    public override void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    protected override void OnDeath()
    {
        CreateBloodPool(bloodPoolValue);
        Destroy(gameObject);
    }
}
