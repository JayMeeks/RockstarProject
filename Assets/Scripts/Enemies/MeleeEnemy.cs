using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class MeleeEnemy : EnemyBase
    {
        private MeleeEnemyData _meleeData;

        private bool _playerInRange = false;

        protected override void Awake()
        {
            base.Awake();
            _meleeData = enemyData as MeleeEnemyData;
        }

        protected override bool CanAttack(float distance)
        {
            return distance <= _meleeData.attackRange;
        }

        protected override IEnumerator ResetAttack()
        {
            yield return new WaitForSeconds(_meleeData.attackCooldown);
            yield return base.ResetAttack();
        }
        
        protected override void PerformAttack()
        {
            base.PerformAttack();

            if (!hasAttacked)
            {
                if (_playerInRange)
                {
                    PlayerHealthSystem.Instance.DamagePlayer(_meleeData.damage);
                }
                
                hasAttacked = true;
                StartCoroutine(ResetAttack());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = false;
        }
    }
}

