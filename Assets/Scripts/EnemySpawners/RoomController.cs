using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class RoomController : MonoBehaviour
{
    [Tooltip("The total number of waves in this room.")]
    [SerializeField] int waveCount;
    [SerializeField] private GameObject entryDoor;
    [SerializeField] private List<WaveClass> waves = new List<WaveClass>();

    private int waveNumber = 0;
    private int aliveEnemies = 0;
    private List<Transform> spawnPositions = new List<Transform>();
    private WaveSystem waveSystem;

    public event Action OnRoomActivated;
    public event Action OnRoomDeactivated;
    public event Action OnWavesFinished;

    private void OnValidate()
    {
        spawnPositions = new List<Transform>(GetComponentsInChildren<Transform>());
        spawnPositions.Remove(transform);
        waveSystem = GetComponentInParent<WaveSystem>();
    }

    public void ActivateRoom()
    {
        entryDoor.SetActive(false);
        OnRoomActivated?.Invoke();
    }

    public void DeactivateRoom()
    {
        OnRoomDeactivated?.Invoke();
    }

    public int GetWaveCount()
    {
        return waveCount;
    }

    public IEnumerator NextWave()
    {
        List<SubwaveClass> subwaves = waves[waveNumber].subwaves;

        foreach (SubwaveClass subwave in subwaves)
        {
            foreach (EnemyClass enemy in subwave.enemies)
            {
                GameObject enemyPrefab = waveSystem.GetGameObjectForEnemyType(enemy.enemyType);

                //NEXT THING TO DO IS TO ADD LOCATION MARKER AS AN IENUMERATOR AND AS A VARIABLE TO ENEMYCLASS
                GameObject newEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);

                //EnemyBase enemyScript = newEnemy.GetComponent<EnemyBase>();

                //enemyScript.SetSpawner(this);

                aliveEnemies++;
            }

            while (aliveEnemies > 0)
            {
                yield return null;
            }
        }

        waveNumber++;
    }

    private void AllWavesFinished()
    {
        Debug.Log("Wave finished");
        OnWavesFinished?.Invoke();
    }

    public void OnRoomEnter()
    {
        entryDoor.SetActive(true);
        NextWave();
    }

    public void EnemyDead()
    {
        aliveEnemies--;
    }
}
