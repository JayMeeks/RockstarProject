using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class RipperStateDecision : State
{
    private RipperStats stats;
    public RipperStateDecision(EnemyStateController controller, NavMeshAgent agent, RipperStats stats) : base(controller, agent)
    {
        this.stats = stats;
    }

    public override void Enter()
    {
        //Checks distance to player
        //If close, start attack
        //Otherwise, move towards player
        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        float distanceToPlayer = Vector3.Distance(agent.transform.position, playerPos);
        if (distanceToPlayer < stats.attackStartRange) //Attack range for ripper TODO: Add scriptable object data
        {
            controller.ChangeState(new RipperStateAttack(controller, agent,stats));
        }
        else
        {
            controller.ChangeState(new RipperStateChase(controller, agent,stats));
        }
    }

    public override void Tick()
    {
      
    }

    public override void Exit()
    {
    
    }
}
