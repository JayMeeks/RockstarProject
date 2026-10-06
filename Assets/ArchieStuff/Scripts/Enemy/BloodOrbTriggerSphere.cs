using UnityEngine;

public class BloodOrbTriggerSphere : MonoBehaviour
{

    
    [SerializeField] BloodOrb bloodOrb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("outer trigger hit");

            bloodOrb.ChasePlayer = true;
        }
    }
}
