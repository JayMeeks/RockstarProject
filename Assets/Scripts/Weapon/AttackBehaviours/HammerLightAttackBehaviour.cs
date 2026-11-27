using System.Collections.Generic;
using ArchieStuff.EnemyArchie;
using UnityEngine;

public class HammerLightAttackBehaviour : MonoBehaviour
{
    [SerializeField] private HammerLightAttackData data;

    /// <summary>
    ///  Starts light attack for the Dagger
    /// </summary>
    /// <param name="originPosition">Start position of the attack overlap box</param>
    /// <param name="originRotation">Rotation of the attack overlap box</param>
    /// <param name="playerForward">Forward of the player camera</param>
    /// <param name="maxHealRange">Range at which the player should regain health after hit</param>
    /// <param name="healAmount">Amount player should heal if within range</param>
    public void Attack(Vector3 originPosition, Quaternion originRotation, Vector3 playerForward , float maxHealRange, int healAmount)
    {
 
        
        //Applies an offset in the direction the player is facing so that the attack originates from the player, and does not extend behind them
        float forwardOffset = data.AttackSize.z * 0.5f;
        originPosition += playerForward * forwardOffset;
        
        //Checks for colliders within the attack range of the hammer
        //Applied damage to all enemies within
        Collider[] hits = Physics.OverlapBox(originPosition, data.AttackSize,originRotation);
        List<GameObject> alreadyHitEnemies = new List<GameObject>();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.CompareTag("EnemyCriticalHurtbox") || hits[i].gameObject.CompareTag("EnemyStandardHurtbox"))
            {
                GameObject owningEnemy = hits[i].gameObject.transform.root.gameObject;
                if (alreadyHitEnemies.Contains(owningEnemy)){return;}
              
                var enemyInfo = hits[i].gameObject.GetComponentInParent<EnemyInfo>();
                enemyInfo.TakeDamage(data.attackDamage);
                if (Vector3.Distance(originPosition, hits[i].gameObject.transform.position) < maxHealRange)
                {
                    PlayerHealthSystem.Instance.HealPlayer(healAmount);
                }
                
                var enemyController = hits[i].gameObject.GetComponentInParent<IStunnable>();
                enemyController.OnStunned(10.0f);
                
                alreadyHitEnemies.Add(owningEnemy);
            }
        }
        

        
        if (data.DebugView) { SpawnTestCube(originPosition, originRotation,data.DebugCubeLifespan); }

        Destroy(gameObject);
    }
    

    
    
    
    //DEBUG
    private void SpawnTestCube(Vector3 originPosition,Quaternion originRotation,float duration)
    {
        GameObject debugCube = Instantiate(data.DebugCube, originPosition, originRotation);
        debugCube.transform.localScale = data.AttackSize;
        debugCube.GetComponent<WeaponAttackDebugCube>().SetLifespan(duration);
    }
}
