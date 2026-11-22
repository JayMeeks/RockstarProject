using UnityEngine;

[CreateAssetMenu(fileName = "HollowStats", menuName = "Scriptable Objects/EnemyStats/HollowStats")]
public class HollowStats : BaseEnemyStats
{
    [Header("Hollow stats stats")]
   
    [Header("Attack stats")]
    [Tooltip("How long the ranged attack should charge for before firing")]
    [SerializeField] public float shotChargeTime;
    
    [Tooltip("How many shots should be fired per burst")]
    [SerializeField] public int shotCount;
    
    [Tooltip("How much time between each shot in a burst")]
    [SerializeField] public float shotInterval;
    
    [Header("Behaviour stats")]
    [Tooltip("Range at which the enemy should shoot if beyond or flee if too close")]
    [SerializeField] public float playerCheckDistance;
    
    [Tooltip("How far away the enemy should try to run if enemy is too close")]
    [SerializeField] public float fleeDistance;
    
    [Header("Bullet stats")]
    [Tooltip("How much damage each bullet should do to the player")]
    [SerializeField] public int bulletDamage;
    
    [Tooltip("Initial move speed of bullet")]
    [SerializeField] public float initialMoveSpeed;
    
    [Tooltip("How fast the bullet should accelerate")]
    [SerializeField] public float acceleration;
    
    [Tooltip("How strong should the homing effect be")]
    [SerializeField] public float homingStrength;
    
    [Header("Status effect stats")]
    [Tooltip("How much  of incoming stun time should the enemy be stunned for")]
    [SerializeField] public float stunTimeMultiplier;
    
    [Tooltip("How much force should the enemy receive from knockback effects")]
    [SerializeField] public float knockbackMultiplier;
}
