using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DaggerLightAttack", menuName = "Scriptable Objects/WeaponAttackData/DaggerLightAttackData")]
public class DaggerLightAttackData : BaseWeaponAttackData
{
    [Header("Size Settings")] 
    [Tooltip("Size of the attack overlap box. Z is range, X Y are width and height")]
    [SerializeField] public Vector3 AttackSize;

    [Tooltip("Set true to spawn a box representing attack area")]
    [SerializeField] public bool DebugView;
    public Vector3 debugPos;

    [Header("Debug Settings")]
    [SerializeField] public GameObject DebugCube;
    
    [Tooltip("How long the debug cube should remain")]
    [SerializeField] public float DebugCubeLifespan;
    
    


  
}
