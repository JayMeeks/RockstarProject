using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class RangedEnemy : EnemyBase
    {
        private RangedEnemyData rangedData;

        protected override void Awake()
        {
            base.Awake();
            rangedData = enemyData as RangedEnemyData;
        }
    
        protected override bool CanAttack(float distance)
        {
            return distance <= rangedData.attackRange && CanSeePlayer();
        }
    
        protected override IEnumerator ResetAttack()
        {
            yield return new WaitForSeconds(rangedData.attackCooldown);
            yield return base.ResetAttack();
        }

        protected override void PerformAttack()
        {
            base.PerformAttack();
        
            if (!_hasAttacked)
            {
                var bullet = Instantiate(rangedData.projectile, transform.position, Quaternion.identity);
                bullet.SetStats(rangedData.damage, rangedData.projectileSpeed);
                //GameObject bullet = Instantiate(projectile, transform.position, Quaternion.identity);
                
                
                _hasAttacked = true;
                StartCoroutine(ResetAttack());
            }
        }


    }
}
