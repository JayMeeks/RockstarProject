using System;
using UnityEngine;

public class PunchingBagData : EnemyInfo
{
    [SerializeField] private PunchingBagStats _stats;
   
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
