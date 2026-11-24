using UnityEngine;

[CreateAssetMenu(fileName = "HealthPickupData", menuName = "Pickups/Health Pickup")]
public class HealthPickupData : ScriptableObject
{
    [Header("Healing")]
    [Min(1)] public int healAmount = 25;
    [Min(0f)] public float healDuration = 3f;
}
