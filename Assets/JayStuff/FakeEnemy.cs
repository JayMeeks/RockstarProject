using UnityEngine;

public class FakeEnemy : MonoBehaviour
{
    private EnemySpawner spawner;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x - Time.deltaTime, transform.position.y, transform.position.z);

        if (transform.position.x < -5)
        {
            Die();
        }
    }

    private void Die()
    {
        spawner.EnemyDead();

        Destroy(gameObject);
    }

    public void SetSpawner(EnemySpawner enemySpawner)
    {
        spawner = enemySpawner;
    }
}
