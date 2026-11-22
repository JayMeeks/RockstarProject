using UnityEngine;

public class DaggerLightAttackBehaviour : MonoBehaviour
{
    [SerializeField] private DaggerLightAttackData data;


  
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
        float forwardOffset = data.AttackSize.z * 0.5f;
        originPosition += playerForward * forwardOffset;
        
        //Checks for colliders within the attack range of the dagger
        //Applied damage to all enemies within
        Collider[] hits = Physics.OverlapBox(originPosition, data.AttackSize,originRotation);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.CompareTag("Enemy"))
            {
                var enemy = hits[i].gameObject.GetComponent<EnemyInfo>();
                enemy.TakeDamage(data.attackDamage);
                if (Vector3.Distance(originPosition, hits[i].gameObject.transform.position) < maxHealRange)
                {
                    PlayerHealthSystem.Instance.HealPlayer(healAmount);
                }
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
