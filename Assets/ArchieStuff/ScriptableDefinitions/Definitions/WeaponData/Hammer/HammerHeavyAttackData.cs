using UnityEngine;

[CreateAssetMenu(fileName = "HammerHeavyAttackData", menuName = "Scriptable Objects/WeaponAttackData/HammerHeavyAttackData")]
public class HammerHeavyAttackData : BaseWeaponAttackData
{
    [Header("Hammer Settings")] 
    [Tooltip("Speed at which the wave travels")] 
    [SerializeField] public float WaveMoveSpeed;
    
    [Tooltip("Speed at which the wave grows")] 
    [SerializeField] public float WaveGrowSpeedWidth,WaveGrowSpeedHeight;
    
    [Tooltip("How long wave exists")] 
    [SerializeField] public float WaveLifespan;
    
    [Tooltip("How strong of a force should be used to send the enemy upwards")] 
    [SerializeField] public float ForceMultiplier;



}
