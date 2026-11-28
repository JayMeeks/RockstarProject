using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private List<RoomController> rooms;
    [SerializeField] protected EnemyPrefabs enemyPrefabs;

    private int waveNumber = 1;
    private int roomNumber = 0;
    private RoomController activeRoom;

    private void OnValidate()
    {
        rooms = new List<RoomController>(GetComponentsInChildren<RoomController>());
    }

    private void Start()
    {
        StartFirstWave();
    }

    private void OnEnable()
    {
        foreach (RoomController room in rooms)
        {
            room.OnWavesFinished += ActivateRoom;
        }
    }

    private void OnDisable()
    {
        foreach (RoomController room in rooms)
        {
            room.OnWavesFinished -= ActivateRoom;
        }
    }

    public void ActivateRoom()
    {
        if (activeRoom != null)
        {
            activeRoom.DeactivateRoom();
        }

        activeRoom = rooms[roomNumber];
        activeRoom.ActivateRoom();
        roomNumber++;
    }

    public GameObject GetGameObjectForEnemyType(int enemyType)
    {
        if (enemyType == 0)
        {
            return enemyPrefabs.shrikePrefab;
        }
        else if (enemyType == 1)
        {
            return enemyPrefabs.ripperPrefab;
        }
        else
        {
            return null;
        }
    }

    [ContextMenu("Start Waves")]
    public void StartFirstWave()
    {
        ActivateRoom();
    }
}
