using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

public class BloodOrb : MonoBehaviour
{
   private Vector3 startPos;
   private float waveFrequency = 3;
   private float waveMagnitude = 0.1f;

   private int healAmount = 50;
   
   private float movetowardsSpeed = 1f;

   public bool ChasePlayer = false;
   


   private void Awake()
   {
      startPos = transform.position;
   }

   private void Update()
   {
      
      transform.position = new Vector3
      (transform.position.x, 
      startPos.y + Mathf.Sin(Time.time * waveFrequency) * waveMagnitude,
      transform.position.z);
               
      
      if(ChasePlayer)
      {
         transform.position = Vector3.MoveTowards(transform.position, PlayerController.Instance.transform.position, movetowardsSpeed * Time.deltaTime);
      }
    
      
   }

 

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         int playerHp = PlayerController.Instance.GetCurrentHealth();
         int playerMaxHp = PlayerController.Instance.GetMaxHealth();

         if (playerHp + healAmount >= playerMaxHp)
         {
            PlayerController.Instance.SetCurrentHealth(playerMaxHp);
         }
         else
         {
            PlayerController.Instance.SetCurrentHealth(playerHp + healAmount);
         }
         Destroy(gameObject);
      }
   }


}
