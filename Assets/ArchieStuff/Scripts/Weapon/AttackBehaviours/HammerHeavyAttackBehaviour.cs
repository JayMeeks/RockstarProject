using System.Collections;
using UnityEngine;

public class HammerHeavyAttackBehaviour : MonoBehaviour
{
   public HammerHeavyAttackData data;

   private void OnEnable()
   {
      
      if (Physics.Raycast(transform.position, new Vector3(0.0f, -1.0f, 0.0f), out RaycastHit hit))
      {
         transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
      }
   }

   public void Attack()
   {
      StartCoroutine(MoveWave());
   }

   private IEnumerator MoveWave()
   {
      float t = 0.0f;
      while (t < data.WaveLifespan)
      {
         t += Time.deltaTime;
         CheckStep();
         transform.position += transform.forward * (Time.deltaTime * data.WaveMoveSpeed);
         transform.localScale += new Vector3( data.WaveGrowSpeedWidth * Time.deltaTime,  data.WaveGrowSpeedHeight * Time.deltaTime,  0);
         yield return null;
      }
      Destroy(gameObject);
     
   }

   private void CheckStep()
   {
      RaycastHit checkHit, maxStepHit;

      if (Physics.Raycast(transform.position, transform.forward, out checkHit,0.1f))
      {
        
         Vector3 stepOrigin = new Vector3(transform.position.x, transform.position.y + data.MaxStepHeight, transform.position.z);
         if (Physics.Raycast(stepOrigin, transform.forward, out checkHit, 0.1f))
         {
            Destroy(gameObject);
         }
         else
         {
            transform.position = FindNewHeight();
         }
         
        
         
      }
   }

   private Vector3 FindNewHeight()
   {
      Vector3 checkpos = transform.position;
    
      Debug.Log("Finding new height");
      while (Physics.Raycast(checkpos, transform.forward, out RaycastHit checkHit,0.1f))
      {
         checkpos = new Vector3(checkpos.x, checkpos.y + 0.01f, checkpos.z);
        
      }
      return new Vector3(transform.position.x, checkpos.y, transform.position.z);
      
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
