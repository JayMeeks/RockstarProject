using System.Collections;
using UnityEngine;

public class HammerHeavyAttackBehaviour : MonoBehaviour
{
   public HammerHeavyAttackData data;


   public void Attack(Vector3 waveDir)
   {
      transform.rotation = Quaternion.LookRotation(waveDir, Vector3.up);
      transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
      StartCoroutine(MoveWave(waveDir));
   }

   private IEnumerator MoveWave(Vector3 waveDir)
   {
      float t = 0.0f;
      while (t < data.WaveLifespan)
      {
         t += Time.deltaTime;
         waveDir.Normalize();
         transform.position += transform.forward * (Time.deltaTime * data.WaveMoveSpeed);
         transform.localScale += new Vector3( data.WaveGrowSpeed * Time.deltaTime,  data.WaveGrowSpeed * Time.deltaTime,  data.WaveGrowSpeed * Time.deltaTime);
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
