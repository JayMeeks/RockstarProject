using UnityEngine;

public class LocationMarker : MonoBehaviour
{
    [SerializeField] private string locationName = "Spawn Location";

    public string LocationName => locationName;
}

