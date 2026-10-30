using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private List<RoomController> rooms;

    private int waveNumber = 1;
    private int roomNumber = 0;
    private RoomController activeRoom;

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
            waveNumber++;
            if (waveNumber > activeRoom.GetWaveCount())
            {
                ActivateRoom(rooms[roomNumber]);
            }
            else
            {
                activeRoom.NextWave();
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
}
