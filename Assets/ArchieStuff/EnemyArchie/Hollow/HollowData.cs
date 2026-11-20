using System;
using UnityEngine;

public class HollowData : EnemyInfo
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
        Debug.Log(damage);
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
