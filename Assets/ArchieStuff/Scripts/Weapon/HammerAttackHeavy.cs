using System.Collections.Generic;
using UnityEngine;

public class HammerAttackHeavy : MonoBehaviour
{

    private float lifespan = 0.1f;
    public int damage;
    public float shockwaveRadius;
    public int ForceMult;
   
    private void Awake()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, shockwaveRadius);
        foreach (Collider collider in colliders)
        { 
            if (collider.gameObject.CompareTag($"Enemy"))
            {
                Debug.Log("Applying force to: " + collider.gameObject.name);
                Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
                if (rb == null){Debug.Log("Rigidbody missing from: " + collider.gameObject.name);} 
                Vector3 dir = collider.transform.position - transform.position;
                Debug.Log(dir);
                dir.Normalize();
                rb.AddForce(Vector3.up * ForceMult, ForceMode.Impulse);

                BaseEnemy enemy = collider.gameObject.GetComponent<BaseEnemy>();
                enemy.TakeDamage(damage);

            }
        }
    }

    public void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0)
        {
            Destroy(gameObject);
        }
    }

}
