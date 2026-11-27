using UnityEngine;

public class BloodPool : MonoBehaviour
{
    private int bloodValue;

  


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool healed = PlayerHealthSystem.Instance.TryHealOverTime(
                bloodValue,
                3
            );

            if (!healed)
                return;
            
            Destroy(gameObject);
        }
        
    }

    public void SetBloodValue(int value)
    {
        this.bloodValue = value;
        Debug.Log(this.bloodValue);
    }
}
