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
    /// <param name="shouldHeal">Whether the player should heal off of the hit</param>
    /// <param name="maxHealRange">Range at which the player should regain health after hit</param>
    public void Attack(Vector3 originPosition, Quaternion originRotation, Vector3 playerForward , out bool shouldHeal, float maxHealRange)
    {
        bool _shouldHeal = false;
        float forwardOffset = data.AttackSize.z * 0.5f;
        originPosition += playerForward * forwardOffset;
        
        Collider[] hits = Physics.OverlapBox(originPosition, data.AttackSize,originRotation);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.CompareTag("Enemy"))
            {
                BaseEnemy enemy = hits[i].gameObject.GetComponent<BaseEnemy>();
                enemy.TakeDamage(data.attackDamage);
                if (Vector3.Distance(originPosition, hits[i].gameObject.transform.position) < maxHealRange)
                {
                    _shouldHeal = true;
                }
                
                Rigidbody rb = hits[i].gameObject.GetComponent<Rigidbody>();
                if (rb == null){Debug.Log("Rigidbody missing from: " + hits[i].gameObject.name);} 
              
                rb.AddForce(Vector3.up * data.ForceMultiplier, ForceMode.Impulse);
            }
        }
        
        shouldHeal = _shouldHeal;
        
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
