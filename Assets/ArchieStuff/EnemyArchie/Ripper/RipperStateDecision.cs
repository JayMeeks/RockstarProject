using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class RipperStateDecision : State
{
    public RipperStateDecision(EnemyStateController controller, NavMeshAgent agent) : base(controller, agent)
    {
    }

    public override void Enter()
    {
        //Checks distance to player
        //If close, start attack
        //Otherwise, move towards player
        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        float distanceToPlayer = Vector3.Distance(agent.transform.position, playerPos);
        if (distanceToPlayer < 1.5f) //Attack range for ripper TODO: Add scriptable object data
        {
            controller.ChangeState(new RipperStateAttack(controller, agent));
        }
        else
        {
            controller.ChangeState(new RipperStateChase(controller, agent));
        }
    }

    public override void Tick()
    {
      
    }

    public override void Exit()
    {
    
    }
}
