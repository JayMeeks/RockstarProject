using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class MeleeEnemy : EnemyBase
    {
        private MeleeEnemyData meleeData;

        private bool playerInRange = false;

        protected override void Awake()
        {
            base.Awake();
            meleeData = enemyData as MeleeEnemyData;
        }

        protected override bool CanAttack(float distance)
        {
            return distance <= meleeData.attackRange;
        }

        protected override IEnumerator ResetAttack()
        {
            yield return new WaitForSeconds(meleeData.attackCooldown);
            yield return base.ResetAttack();
        }
        
        protected override void PerformAttack()
        {
            base.PerformAttack();

            if (!_hasAttacked)
            {
                if (playerInRange)
                {
                    PlayerHealthSystem.Instance.DamagePlayer(meleeData.damage);
                }
                
                _hasAttacked = true;
                StartCoroutine(ResetAttack());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            playerInRange = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            playerInRange = false;
        }
    }
}

