using UnityEngine;

[CreateAssetMenu(fileName = "BaseWeaponAttackData", menuName = "Scriptable Objects/WeaponAttackData/BaseWeaponAttackData")]
public abstract class BaseWeaponAttackData : ScriptableObject
{
    [Tooltip("Amount of damage attack will do at base")]
    [SerializeField] public int attackDamage;



}
