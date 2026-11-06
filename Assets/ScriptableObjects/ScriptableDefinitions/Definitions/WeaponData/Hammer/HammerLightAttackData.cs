using UnityEngine;

[CreateAssetMenu(fileName = "HammerLightAttackData", menuName = "Scriptable Objects/WeaponAttackData/HammerLightAttackData")]
public class HammerLightAttackData : BaseWeaponAttackData
{
    [Header("Size Settings")] 
    [Tooltip("Size of the attack overlap box. Z is range, X Y are width and height")]
    [SerializeField] public Vector3 AttackSize;

    [Tooltip("How strong of a force should be used to send the enemy upwards")] 
    [SerializeField] public float ForceMultiplier;

    [Tooltip("Set true to spawn a box representing attack area")]
    [SerializeField] public bool DebugView;
  

    [Header("Debug Settings")]
    [SerializeField] public GameObject DebugCube;
    
    [Tooltip("How long the debug cube should remain")]
    [SerializeField] public float DebugCubeLifespan;
}
