using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private List<RoomController> rooms;

    private int waveNumber = 1;
    private int roomNumber = 0;
    private RoomController activeRoom;

    private void Start()
    {
        StartFirstWave();
    }

    private void OnEnable()
    {
        foreach (RoomController room in rooms)
        {
            room.OnWavesFinished += NextWave;
        }
    }

    private void OnDisable()
    {
        foreach (RoomController room in rooms)
        {
            room.OnWavesFinished -= NextWave;
        }
    }

    public void NextWave()
    {
        if (activeRoom != null)
        {  
            if (waveNumber > activeRoom.GetWaveCount())
            {
                ActivateRoom(rooms[roomNumber]);
            }
            else
            {
                activeRoom.NextWave();
                waveNumber++;
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

    [ContextMenu("Start Waves")]
    public void StartFirstWave()
    {
        ActivateRoom(rooms[roomNumber]);
        NextWave();
    }
}
