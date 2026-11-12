using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DaggerHeavyAttack", menuName = "Scriptable Objects/WeaponAttackData/DaggerHeavyAttackData")]
public class DaggerHeavyAttackData : BaseWeaponAttackData
{
    [Header("Dagger Settings")] 
    [Tooltip("How fast the dagger is thrown after using special attack")]
    [SerializeField] public float throwSpeedMult;
    
    [Tooltip("Maximum range the dagger can be thrown before returning")]
    [SerializeField] public float maxHeavyRange;
    
}
