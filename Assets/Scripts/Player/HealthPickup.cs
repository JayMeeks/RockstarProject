using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Data")]
    [SerializeField] private HealthPickupData pickupData;

    private bool pickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp || pickupData == null)
            return;

        PlayerHealthSystem playerHealth =
            other.GetComponentInParent<PlayerHealthSystem>();

        if (playerHealth == null ||
            playerHealth != PlayerHealthSystem.Instance)
        {
            return;
        }

        bool healed = playerHealth.TryHealOverTime(
            pickupData.healAmount,
            pickupData.healDuration
        );

        if (!healed)
            return;

        pickedUp = true;
        Destroy(gameObject);
    }
}
