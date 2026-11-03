using System.Collections;
using UnityEngine;

public class DaggerHeavyAttackBehaviour : MonoBehaviour
{
    [SerializeField] private DaggerHeavyAttackData data;

    private bool _chainExtended = false;
    
    private bool enemyHit = false;
    
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
       _chainExtended = true;
       _lineRenderer.enabled = true;
       
       while (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= data.maxHeavyRange && enemyHit == false)
       {
           _lineRenderer.SetPosition(0, PlayerController.Instance.transform.position);
           _lineRenderer.SetPosition(1, transform.position);
           transform.position += throwDir * (Time.deltaTime * data.throwSpeedMult);
           yield return null;
       }
       _chainExtended = false;
       
       _lineRenderer.enabled = false;
       PlayerController.Instance.SetWeaponEnabled(true);
       PlayerController.Instance.SetWeaponVisible(true);
       Destroy(gameObject);
      

   }

   private void OnTriggerEnter(Collider other)
   {
       if (other.gameObject.CompareTag("Enemy"))
       {
           var enemy = other.gameObject.GetComponent<BaseEnemy>();
           enemy.TakeDamage(data.attackDamage);
           enemyHit = true;
       }
   }
}
