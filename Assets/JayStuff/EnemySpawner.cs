using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private RoomController roomController;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float radius;
    [SerializeField] private float spawnCooldown;

    [SerializeField] private List<WaveClass> waves = new List<WaveClass>();

    public int aliveEnemies = 0;

    public event Action OnSubWavesFinished;

    private int waveNumber = 0;
    private int subWaveNumber = 0;

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
        Debug.Log(SpaceAboveSpawner());
    }

    private bool SpaceAboveSpawner()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        return hits.Length < 0;
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
            foreach(EnemyClass enemy in waves[waveNumber].subwaves[subWaveNumber].enemies)
            {
                while (!SpaceAboveSpawner() && isActiveAndEnabled)
                {
                    yield return null;
                }

                Instantiate(enemy.enemyPrefab);
                aliveEnemies++;

                yield return new WaitForSeconds(spawnCooldown);
            }

            while (aliveEnemies > 0)
            {
                yield return null;
            }

            subWaveNumber++;
        }

        waveNumber++;
        OnSubWavesFinished?.Invoke();
    }

    private void StartNextWave()
    {
        StartCoroutine(NextWave());
    }
}
