using System;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float speed = 10.0f;

    private void Start()
    {
        Destroy(this.gameObject, 10.0f);
    }

    void Update()
    {
       transform.position += (PlayerMovement.Instance.transform.position - transform.position).normalized * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthSystem.Instance.TakeDamage(5);
            Destroy(gameObject);
        }
    }
}
