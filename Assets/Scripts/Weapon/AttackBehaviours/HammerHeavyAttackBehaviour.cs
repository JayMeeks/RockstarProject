using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class HammerHeavyAttackBehaviour : MonoBehaviour
{
   [SerializeField] public HammerHeavyAttackData standardData, enhancedData;
   private HammerHeavyAttackData activeData;


   private bool isEnhanced = false;

   private void OnEnable()
   {
      
      //Moves wave to the ground if player is over a ledge
      //TODO: Add max distance
      if (Physics.Raycast(transform.position, new Vector3(0.0f, -1.0f, 0.0f), out RaycastHit hit))
      {
         transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
      }
   }

   public void StartAttack(bool enhanced)
   {
      activeData = isEnhanced ? enhancedData : standardData;
      StartCoroutine(MoveWave());
   }

   private IEnumerator MoveWave()
   {
      //Time varible
      float t = 0.0f;
      
      
      while (t < activeData.WaveLifespan)
      {
         t += Time.deltaTime;
         //Checks if there's a wall in front of the wave
         CheckStep();
         //Moves the wave forwards
         transform.position += transform.forward * (Time.deltaTime * activeData.WaveMoveSpeed);
         //Changes scale of wave 
         transform.localScale += new Vector3( activeData.WaveGrowSpeedWidth * Time.deltaTime,  activeData.WaveGrowSpeedHeight * Time.deltaTime,  0);
         yield return null;
      }
      //Destroys wave at end of its lifespan if it hasn't been destroyed already
      Destroy(gameObject);
     
   }

   private void CheckStep()
   {
      RaycastHit checkHit;
      
      //Check for wall in front of player
      if (Physics.Raycast(transform.position, transform.forward, out checkHit,0.1f))
      {
         //Prevent trying to step over enemy
         if (checkHit.transform.gameObject.CompareTag("Enemy")) { return;}
         
         //Checks if wall is beyond the max step height
         Vector3 stepOrigin = new Vector3(transform.position.x, transform.position.y + activeData.MaxStepHeight, transform.position.z);
         if (Physics.Raycast(stepOrigin, transform.forward, out checkHit, 0.1f))
         {
            //If wall too high, destroy wave
            Destroy(gameObject);
         }
         else
         {
            //Otherwise, move wave to the top of the wall
            transform.position = FindNewHeight();
         }
         
        
         
      }
   }

   private Vector3 FindNewHeight()
   {
      Vector3 checkpos = transform.position;
    
      //Starts a raycast at the wave position
      //Moves the raycast up until it hits nothing, meaning top of wall has been reached
      while (Physics.Raycast(checkpos, transform.forward, out RaycastHit checkHit,0.1f))
      {
         checkpos = new Vector3(checkpos.x, checkpos.y + 0.01f, checkpos.z);
        
      }
      //Move wave up
      return new Vector3(transform.position.x, checkpos.y, transform.position.z);
      
   }
   
   

   private void OnTriggerEnter(Collider other)
   {
     
      if (other.gameObject.CompareTag("Enemy"))
      {
         var enemy = other.gameObject.GetComponent<EnemyBase>();
         enemy.TakeDamage(activeData.attackDamage);
     
                
         Rigidbody rb = enemy.gameObject.GetComponent<Rigidbody>();
         if (rb == null){Debug.Log("Rigidbody missing from: " + enemy.gameObject.name);} 
              
         rb.AddForce(Vector3.up * activeData.ForceMultiplier, ForceMode.Impulse);
      }
   }
}
