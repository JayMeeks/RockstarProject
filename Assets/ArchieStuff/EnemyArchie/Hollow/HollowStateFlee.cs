using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateFlee : State
{
    private float fleeDistance;
    private Vector3 targetPosition;
    
    public HollowStateFlee(EnemyStateController controller, NavMeshAgent agent, Vector3 targetPosition) : base(controller, agent)
    {
      this.targetPosition = targetPosition;
    }

    public override void Enter()
    {
        agent.destination = targetPosition;
    }

    public override void Tick()
    {
    
        if (agent.enabled && agent.remainingDistance <= agent.stoppingDistance)
        {
            controller.ChangeState(new HollowStateChargeShot(controller, agent));
        }
    }

    public override void Exit()
    {
        
    }
}
