using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class RoomController : MonoBehaviour
{
    [Tooltip("The total number of waves in this room.")]
    [SerializeField] int waveCount;
    [SerializeField] private List<EnemySpawner> spawners;

    public event Action OnRoomActivated;
    public event Action OnRoomDeactivated;
    public event Action OnWaveStarted;
    public event Action OnWavesFinished;

    private int finishedSpawners;

    private void OnEnable()
    {
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.OnSubWavesFinished += SpawnerFinished;
        }
    }

    private void OnDisable()
    {
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.OnSubWavesFinished -= SpawnerFinished;
        }
    }

    public void ActivateRoom()
    {
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

    public void NextWave()
    {
        OnWaveStarted?.Invoke();
    }

    private void SpawnerFinished()
    {
        finishedSpawners++;

        if (finishedSpawners >= spawners.Count)
        {
            AllSpawnersFinished();
        }
    }

    private void AllSpawnersFinished()
    {
        OnWavesFinished?.Invoke();
    }
}
