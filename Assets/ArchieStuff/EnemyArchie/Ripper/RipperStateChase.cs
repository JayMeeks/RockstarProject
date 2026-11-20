using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class RipperStateChase : State
{
    public RipperStateChase(EnemyStateController controller, NavMeshAgent agent) : base(controller, agent)
    {
    
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
        if (distanceToPlayer < 1.5f) //Attack range for ripper TODO: Add scriptable object data
        {
            controller.ChangeState(new RipperStateAttack(controller, agent));
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
