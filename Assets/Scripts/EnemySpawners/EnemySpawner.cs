using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private RoomController roomController;
    [SerializeField] private LayerMask enemyLayer;
    private float radius;
    [SerializeField] private float spawnCooldown;

    [SerializeField] private List<WaveClass> waves = new List<WaveClass>();

    public int aliveEnemies = 0;

    public event Action OnSubWavesFinished;

    private int waveNumber = 0;

    private void OnEnable()
    {
        roomController.OnWaveStarted += StartNextWave;
    }

    private void OnDisable()
    {
        roomController.OnWaveStarted -= StartNextWave;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool SpaceAboveSpawner()
    {
        radius = transform.localScale.x / 2;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        return hits.Length == 0;
    }

    public void ActivateSpawner()
    {
        enabled = true;
    }

    public void DeactivateSpawner()
    {
        enabled = false;
    }

    private IEnumerator NextWave()
    {
        List<SubwaveClass> subwaves = waves[waveNumber].subwaves;

        foreach (SubwaveClass subwave in subwaves)
        {
            foreach(EnemyClass enemy in subwave.enemies)
            {
                while (!SpaceAboveSpawner() && isActiveAndEnabled)
                {
                    yield return null;
                }

                GameObject newEnemy = Instantiate(enemy.enemyPrefab, transform.position, transform.rotation);

                EnemyBase enemyScript = newEnemy.GetComponent<EnemyBase>();

                enemyScript.SetSpawner(this);

                aliveEnemies++;

                yield return new WaitForSeconds(spawnCooldown);
            }

            while (aliveEnemies > 0)
            {
                yield return null;
            }
        }

        waveNumber++;
        OnSubWavesFinished?.Invoke();
    }

    private void StartNextWave()
    {
        StartCoroutine(NextWave());
    }

    public void EnemyDead()
    {
        aliveEnemies--;
    }
}
