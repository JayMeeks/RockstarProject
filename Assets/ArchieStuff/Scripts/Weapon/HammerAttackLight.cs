using System;
using UnityEngine;

public class HammerAttackLight : MonoBehaviour
{

    private BoxCollider col;
    private float lifespan = 0.1f;
    public int damage, hpRestored;
   
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
            enemy.TakeDamage(damage);

            if (Vector3.Distance(PlayerController.Instance.transform.position, other.transform.position) < 2f)
            {
                PlayerController.Instance.HealPlayer(hpRestored);
            }
            
        }
    }
}
