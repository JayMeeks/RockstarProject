using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class RangedEnemy : EnemyBase
    {
        private RangedEnemyData _rangedData;

        protected override void Awake()
        {
            base.Awake();
            _rangedData = enemyData as RangedEnemyData;
        }
    
        protected override bool CanAttack(float distance)
        {
            return distance <= _rangedData.attackRange && CanSeePlayer();
            //ensures that player is both in range and that the enemy can see the player and there is a clear path between the enemy and player (no walls) and the enemy is facing the player 
        }
    
        protected override IEnumerator ResetAttack()
        {
            yield return new WaitForSeconds(_rangedData.attackCooldown);
            yield return base.ResetAttack();
        }
        
        protected override void PerformAttack()
        {
            base.PerformAttack();
        
            if (!hasAttacked)
            {
                //Spawns a projectile aimed at the player and then populates with the data from the EnemyData scriptable object 
                var _bullet = Instantiate(_rangedData.projectile, transform.position, Quaternion.identity);
                _bullet.SetStats(_rangedData.damage, _rangedData.projectileSpeed);
                
                
                hasAttacked = true;
                StartCoroutine(ResetAttack());
            }
        }


    }
}
