using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class RipperStateAttack : State
{

    private RipperStats stats;
   
    private float attackWindupTimer;
    private float attackCooldownTimer;
    private float attackIntervalTimer;
    
    private int attacksPerformed = 0;
 
    public RipperStateAttack(EnemyStateController controller, NavMeshAgent agent, RipperStats stats) : base(controller, agent)
    {
        this.stats = stats;
    }

    public override void Enter()
    {
        //Sets cooldown timers
        attackCooldownTimer = stats.attackCooldown;
        attackWindupTimer = stats.attackWindup;
        attackIntervalTimer = stats.attackInterval;
        //Also will probably be replaced by animation timings?


        //Stops enemy from moving when initiating attack
        //Probably temporary? not sure how this will play out in game
        //For now, stops enemy sliding around awkwardly
        agent.isStopped = true; //Maybe change to agent.speed = 0?
       
    }

    public override void Tick()
    { 
        //Wait for initial attack windup
        if(attackWindupTimer > 0.0f){ attackWindupTimer -= Time.deltaTime; return; }
        
        
        //Perform all attacks 
        if (attackIntervalTimer <= 0.0f && attacksPerformed < stats.attacksToPerform)
        {
            PerformMeleeAttack();
            attacksPerformed++;
            attackIntervalTimer = stats.attackInterval;
        }
        else if(attackIntervalTimer > 0.0f && attacksPerformed < stats.attacksToPerform)
        {
            attackIntervalTimer -= Time.deltaTime;
        }
        
        //Wait for cooldown after finishing attack string
        if (attacksPerformed == stats.attacksToPerform && attackCooldownTimer <= 0.0f)
        {
            controller.ChangeState(new RipperStateDecision(controller, agent,stats));
        }
        else if( attacksPerformed == stats.attacksToPerform && attackCooldownTimer > 0.0f)
        {
            attackCooldownTimer -= Time.deltaTime;
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
        float forwardOffset = stats.attackSize.z * 0.5f;
        Vector3 originPosition = agent.transform.position;
        originPosition += agent.transform.forward * forwardOffset;
        
        Collider[] hits = new Collider[1];
        
        Physics.OverlapBoxNonAlloc(
        center: originPosition,
        halfExtents: stats.attackSize,
        orientation: agent.transform.rotation,
        results: hits,
        mask: LayerMask.GetMask("Player") //Will only ever find the player
       );

        if (hits[0])
        {
            PlayerHealthSystem.Instance.DamagePlayer(stats.attackDamage); //TODO: Scriptable object stats:
        }
    }
}
