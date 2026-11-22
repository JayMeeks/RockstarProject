using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class RipperStateChase : State
{
    private RipperStats stats;
  
    public RipperStateChase(EnemyStateController controller, NavMeshAgent agent, RipperStats stats) : base(controller, agent)
    {
        this.stats = stats;
    }

    public override void Enter()
    {
      
    }

    public override void Tick()
    {
        //Checks distance to player
        //If close, start attack
        //Otherwise, move towards player
        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        float distanceToPlayer = Vector3.Distance(agent.transform.position, playerPos);
        if (distanceToPlayer < stats.attackStartRange) //Attack range for ripper 
        {
            controller.ChangeState(new RipperStateAttack(controller, agent,stats));
        }
        else
        {
            if (agent.enabled)
            {
                agent.SetDestination(playerPos);
            }
        }
    }

    public override void Exit()
    {
        
    }
}
