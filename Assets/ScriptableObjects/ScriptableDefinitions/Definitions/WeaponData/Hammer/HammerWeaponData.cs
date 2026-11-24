using UnityEngine;

[CreateAssetMenu(fileName = "HammerWeaponData", menuName = "Scriptable Objects/WeaponData/HammerWeaponData")]
public class HammerWeaponData : BaseWeaponData
{
  [Header("Special settings")]
  
  [Tooltip("Force multiplier for special attack")]
  [SerializeField] public float specialForceMultiplier;
  
  [Tooltip("Force multiplier for downwards force applied when attacks are charged")]
  [SerializeField] public float slamForceMultiplier;
}
