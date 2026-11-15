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
            room.OnWavesFinished += StartWaves;
        }
    }

    private void OnDisable()
    {
        foreach (RoomController room in rooms)
        {
            room.OnWavesFinished -= StartWaves;
        }
    }

    public void StartWaves()
    {
        if (activeRoom != null)
        {  
            if (waveNumber > activeRoom.GetWaveCount())
            {
                Debug.Log("Room finished");
                ActivateRoom(rooms[roomNumber]);
            }
            else if (roomNumber <= rooms.Count)
            {
                activeRoom.NextWave();
                waveNumber++;
            }
            else
            {
                Debug.Log("All Rooms Completed");
            }
        }
        else
        {
            ActivateRoom(rooms[roomNumber]);
        }    
    }

    public void ActivateRoom(RoomController room)
    {
        if (activeRoom != null)
        {
            activeRoom.DeactivateRoom();
        }

        activeRoom = room;
        activeRoom.ActivateRoom();
        waveNumber = 1;
        roomNumber++;
    }

    public GameObject GetGameObjectForEnemyType(EnemyClass.EnemyType enemyType)
    {
        if (enemyType is EnemyClass.EnemyType.Melee)
        {
            return enemyPrefabs.meleePrefab;
        }
        else if (enemyType is EnemyClass.EnemyType.Ranged)
        {
            return enemyPrefabs.rangedPrefab;
        }
        else
        {
            return null;
        }
    }

    [ContextMenu("Start Waves")]
    public void StartFirstWave()
    {
        ActivateRoom(rooms[roomNumber]);
        StartWaves();
    }
}
