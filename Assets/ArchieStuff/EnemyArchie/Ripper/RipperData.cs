using System;
using UnityEngine;

public class RipperData : EnemyInfo
{
    private void Start()
    {
        Initialize();
    }


    protected override void Initialize()
    {
        MaxHealth = 100;
        CurrentHealth = MaxHealth;
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
        Destroy(gameObject);
    }
}
