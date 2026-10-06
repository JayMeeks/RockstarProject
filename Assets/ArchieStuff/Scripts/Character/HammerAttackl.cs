using System;
using UnityEngine;

public class HammerAttackl : MonoBehaviour
{
    private BoxCollider col;
    private float lifespan = 0.1f;
   
    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    public void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag($"Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            int damage = PlayerController.Instance.GetHPInvestedIntoWeapon();
            if (damage == 0)
            {
                damage += 5;
            }
            enemy.TakeDamage(damage);
        }
    }
}
