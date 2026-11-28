using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.Timeline;

public class RoomController : MonoBehaviour
{
    [Tooltip("The total number of waves in this room.")]
    [SerializeField] public int waveCount;
    //[SerializeField] private GameObject entryDoor;

    private int waveNumber = 0;
    private int aliveEnemies = 0;
    private List<Transform> spawnPositions = new List<Transform>();
    private List<LocationMarker> spawnCounts = new List<LocationMarker>();
    private WaveSystem waveSystem;

    public event Action OnRoomActivated;
    public event Action OnRoomDeactivated;
    public event Action OnWavesFinished;

    private void OnValidate()
    {
        spawnPositions = new List<Transform>(GetComponentsInChildren<Transform>());
        spawnPositions.Remove(transform);
        waveSystem = GetComponentInParent<WaveSystem>();
        spawnCounts = new List<LocationMarker>(GetComponentsInChildren<LocationMarker>());
    }

    public void ActivateRoom()
    {
        //entryDoor.SetActive(false);
        StartCoroutine(StartWaves());
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

    public IEnumerator StartWaves()
    {
        for (int i = 0; i < waveCount; i++)
        {
            foreach (LocationMarker marker in spawnCounts)
            {
                for (int j = 0; j < marker.waves[i].shrikeCount; j++)
                {
                    SpawnEnemy(0, marker);
                }

                for (int j = 0; j < marker.waves[i].ripperCount; j++)
                {
                    SpawnEnemy(1, marker);
                }

            }

            while (aliveEnemies > 0)
            {
                yield return null;
            }
        }

        AllWavesFinished();

    }

    private void AllWavesFinished()
    {
        OnWavesFinished?.Invoke();
    }

    public void OnRoomEnter()
    {
        //entryDoor.SetActive(true);
    }

    public void EnemyDead()
    {
        aliveEnemies--;
    }

    private void SpawnEnemy(int enemyType, LocationMarker marker)
    {
        GameObject enemyPrefab = waveSystem.GetGameObjectForEnemyType(enemyType);

        Vector3 spawnPosition = GetRandomSpawnPoint(marker);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, marker.transform.rotation);

        EnemyBase enemyScript = newEnemy.GetComponent<EnemyBase>();

        enemyScript.SetSpawner(this);

        aliveEnemies++;
    }

    private Vector3 GetRandomSpawnPoint(LocationMarker marker)
    {
        Vector3 centre = marker.transform.position;
        float radius = marker.radius;

        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * radius;

        Vector3 randomSpawnPosition = centre + new Vector3(randomPoint.x, marker.transform.position.y, randomPoint.y);

        return randomSpawnPosition;
    }
}
