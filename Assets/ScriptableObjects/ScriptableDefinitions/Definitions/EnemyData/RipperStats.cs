using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "RipperStats", menuName = "Scriptable Objects/EnemyStats/RipperStats")]
public class RipperStats : BaseEnemyStats
{
    [Header("Ripper stats")]
    
    [Header("Attack stats")]
    [Tooltip("How close the enemy needs to be to try to attack")]
    [SerializeField] public float attackStartRange;
    
    [Tooltip("How long it takes for the melee attack to windup")]
    [SerializeField] public float attackWindup;
    
    [Tooltip("How many times should the melee attack be performed")]
    [SerializeField] public int attacksToPerform;
    
    [Tooltip("How long between each swipe of the ripper attack")]
    [SerializeField] public float attackInterval;
    
    [Tooltip("How long after attacking before the enemy tries to act again")]
    [SerializeField] public float attackCooldown;
    
    [Tooltip("Damage dealt by attack")]
    [SerializeField] public int attackDamage;
    
    [Tooltip("Size of the attack hitbox")]
    [SerializeField] public Vector3 attackSize;
    
    [Header("Status effect stats")]
    [Tooltip("How much  of incoming stun time should the enemy be stunned for")]
    [SerializeField] public float stunTimeMultiplier;
    
    [Tooltip("How much force should the enemy receive from knockback effects")]
    [SerializeField] public float knockbackMultiplier;
    


    

    
    
}
