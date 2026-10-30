using System;
using UnityEngine;

public class WeaponAttackDebugCube : MonoBehaviour
{
   private float lifespan;

   private void Update()
   {
       lifespan -= Time.deltaTime;
       if (lifespan <= 0)
       {
           Destroy(gameObject);
       }
   }

   public void SetLifespan(float lifespan)
   {
      this.lifespan = lifespan;
   }
}
