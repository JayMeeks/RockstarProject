using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    private MeleeEnemyData melee;
    
    protected override bool CanAttack(float distance)
    {
        return distance <= melee.attackRange;
    }

    protected override void PerformAttack()
    {
        
    }
}
