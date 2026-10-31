using System.Collections;
using UnityEngine;

public class HammerHeavyAttackBehaviour : MonoBehaviour
{
   public HammerHeavyAttackData data;


   public void Attack( )
   {
      StartCoroutine(MoveWave());
   }

   private IEnumerator MoveWave()
   {
      float t = 0.0f;
      while (t < data.WaveLifespan)
      {
         t += Time.deltaTime;
         transform.position += transform.forward * (Time.deltaTime * data.WaveMoveSpeed);
         transform.localScale += new Vector3( data.WaveGrowSpeedWidth * Time.deltaTime,  data.WaveGrowSpeedHeight * Time.deltaTime,  0);
         yield return null;
      }
      Destroy(gameObject);
     
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.gameObject.CompareTag("Enemy"))
      {
         var enemy = other.gameObject.GetComponent<BaseEnemy>();
         enemy.TakeDamage(data.attackDamage);
     
                
         Rigidbody rb = enemy.gameObject.GetComponent<Rigidbody>();
         if (rb == null){Debug.Log("Rigidbody missing from: " + enemy.gameObject.name);} 
              
         rb.AddForce(Vector3.up * data.ForceMultiplier, ForceMode.Impulse);
      }
   }
}
