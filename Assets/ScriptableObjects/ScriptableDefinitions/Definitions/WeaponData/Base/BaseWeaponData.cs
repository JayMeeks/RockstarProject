using UnityEngine;

[CreateAssetMenu(fileName = "BaseWeaponData", menuName = "Scriptable Objects/WeaponData/BaseWeapon")]
public class BaseWeaponData : ScriptableObject
{
    [Header("Base weapon settings")]
    
    [Tooltip("Weapon start durability")]
    [SerializeField] public int maxDurability;
    
    [Tooltip("Hp amount player will gain if they are close enough to enemy on successful hit")]
    [SerializeField] public int playerHpGainOnHit;
    
    [Tooltip("Maximum range at which the player can regain hp from a hit")]
    [SerializeField] public int playerHpGainMaxRange;
    
    [Tooltip("Cost in health to initiate heavy attack")]
    [SerializeField] public int heavyAttackHpCost;
    
    [Tooltip("Cost in health to initiate special attack")]
    [SerializeField] public int specialAttackHpCost;
}
