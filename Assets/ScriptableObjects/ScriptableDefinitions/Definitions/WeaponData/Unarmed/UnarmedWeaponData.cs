using UnityEngine;

[CreateAssetMenu(fileName = "UnarmedWeaponData", menuName = "Scriptable Objects/WeaponData/UnarmedWeaponData")]
public class UnarmedWeaponData : BaseWeaponData
{
  [Header("Siphon settings")]
  [Tooltip("Max range that the player can be at to start the siphon")] 
  [SerializeField] public float SiphonStartRange;
  
  [Tooltip("Max range that the player can be at to drain health from an enemy")] 
  [SerializeField] public float SiphonDrainRange;
  
  [Tooltip("Amount of health player heals per tick of the siphon")] 
  [SerializeField] public int SiphonHealAmount;
  
  [Tooltip("How often the siphon will heal (seconds between siphons)")] 
  [SerializeField] public float SiphonTickInterval;
}