using UnityEngine;

[CreateAssetMenu(fileName = "DaggerWeaponData", menuName = "Scriptable Objects/WeaponData/DaggerWeaponData")]
public class DaggerWeaponData : BaseWeaponData
{
    [Header("Special settings")]
  
    [Tooltip("Force multiplier for special attack")]
    [SerializeField] public float specialForceMultiplier;
    
   
}
