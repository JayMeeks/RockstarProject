using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class RoomController : MonoBehaviour
{
    [Tooltip("The total number of waves in this room.")]
    [SerializeField] int waveCount;
    [SerializeField] private GameObject entryDoor;

    public event Action OnRoomActivated;
    public event Action OnRoomDeactivated;
    public event Action OnWavesFinished;

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

    public void NextWave()
    {
        
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
}
