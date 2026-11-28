using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationMarker : MonoBehaviour
{
    [SerializeField] private string locationName = "Spawn Location";
    [SerializeField] public List<NewWaveClass> waves;
    [SerializeField] public float radius = 1;

    private RoomController roomController;
    public string LocationName => locationName;

    private void OnValidate()
    {
        roomController = GetComponentInParent<RoomController>();

        if (roomController == null || waves == null)
        {
            return;
        }

        while (waves.Count < roomController.waveCount)
        {
            waves.Add(new NewWaveClass());
        }

        while (waves.Count > roomController.waveCount)
        {
            waves.RemoveAt(waves.Count - 1);
        }

        transform.localScale = new Vector3(radius, 0.1f, radius);
    }

    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }
}

