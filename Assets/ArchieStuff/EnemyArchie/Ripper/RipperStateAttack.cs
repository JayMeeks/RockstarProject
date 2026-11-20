using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class RipperStateAttack : State
{
    private float attackWindup;
    private float attackCooldown;
    private bool hasAttacked;
 
 
    public RipperStateAttack(EnemyStateController controller, NavMeshAgent agent) : base(controller, agent)
    {
    
    }

    public override void Enter()
    {
        //Sets cooldown timers
        attackCooldown = 1.0f; //TODO: Scriptable object stats
        attackWindup = 0.5f; //TODO: Scriptable object stats
        //Also will probably be replaced by animation timings?


        //Stops enemy from moving when initiating attack
        //Probably temporary? not sure how this will play out in game
        //For now, stops enemy sliding around awkwardly
        agent.isStopped = true; //Maybe change to agent.speed = 0?
       
    }

    public override void Tick()
    { 
     //Could be improved when animations are implemented
     
      //If enemy hasn't attacked yet, decrease windup timer until attack
      if (!hasAttacked)
      { 
          if (attackWindup <= 0.0f)
          {
              PerformMeleeAttack(); 
              hasAttacked = true;
          }
          attackWindup -= Time.deltaTime;
      }
      
      //After attack, decrease cooldown timer to create a gap after attack -> chase
      if (hasAttacked)
      {
          if (attackCooldown <= 0.0f && hasAttacked)
          {
              controller.ChangeState(new RipperStateDecision(controller, agent));
          }
          attackCooldown -= Time.deltaTime;
      }
    }

    public override void Exit()
    {
        //Allows enemy to move again
        agent.isStopped = false;
    }

    //Performs an overlap check in the direction the enemy is facing
    //If it hits the player, damages them
    private void PerformMeleeAttack()
    {
        Vector3 attackSize = new Vector3(1, 1, 10); //TODO: Scriptable object stats
        float forwardOffset = attackSize.z * 0.5f;
        Vector3 originPosition = agent.transform.position;
        originPosition += agent.transform.forward * forwardOffset;
        
        Collider[] hits = new Collider[1];
        
        Physics.OverlapBoxNonAlloc(
        center: originPosition,
        halfExtents: attackSize,
        orientation: agent.transform.rotation,
        results: hits,
        mask: LayerMask.GetMask("Player") //Will only ever find the player
       );

        if (hits[0])
        {
            PlayerHealthSystem.Instance.DamagePlayer(10); //TODO: Scriptable object stats:
        }
    }
}
