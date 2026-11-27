using System.Collections;
using UnityEngine;

public class DaggerHeavyAttackBehaviour : MonoBehaviour
{
    [SerializeField] private DaggerHeavyAttackData data;


    
    private bool colliderHit = false;
    
    [Header("Line renderer")]
    [SerializeField]private LineRenderer _lineRenderer;
    [SerializeField]private Material _lineMaterial;

    private void OnEnable()
    {
        _lineRenderer.material = _lineMaterial;
    }

   public void Attack(Vector3 throwDir)
   {
       transform.rotation = Quaternion.LookRotation(throwDir, Vector3.up);
       transform.SetParent(null);

       StartCoroutine(ThrowDagger(throwDir));
   }

   private IEnumerator ThrowDagger(Vector3 throwDir)
   {
       _lineRenderer.enabled = true;
       
       //Moves dagger projectile towards facing direction
       //Returns dagger when it hits something or reaches max range
       while (Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= data.maxHeavyRange && colliderHit == false)
       {
           _lineRenderer.SetPosition(0, PlayerMovement.Instance.transform.position);
           _lineRenderer.SetPosition(1, transform.position);
           transform.position += throwDir * (Time.deltaTime * data.throwSpeedMult);
           yield return null;
       }

       
       _lineRenderer.enabled = false;
       PlayerWeaponSystem.Instance.SetWeaponEnabled(true);
       PlayerWeaponSystem.Instance.SetWeaponVisible(true);
       Destroy(gameObject);
      

   }

   private void OnTriggerEnter(Collider other)
   {
       if(other.CompareTag("Player")){return;}
       if (other.gameObject.CompareTag("EnemyCriticalHurtbox"))
       {
           var enemy = other.gameObject.GetComponentInParent<EnemyInfo>();
           enemy.TakeDamage(data.criticalHitDamage);
           colliderHit = true;
       }
       else if (other.gameObject.CompareTag("EnemyStandardHurtbox"))
       {
           var enemy = other.gameObject.GetComponentInParent<EnemyInfo>();
           enemy.TakeDamage(data.attackDamage);
           colliderHit = true;
       }
       PlayerHealthSystem.Instance?.HealPlayer(data.hpRegainOnHit);
       
   }
}
