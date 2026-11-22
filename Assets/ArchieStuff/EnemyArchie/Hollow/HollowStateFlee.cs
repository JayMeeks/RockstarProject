using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateFlee : State
{
    private Vector3 targetPosition;
    private HollowStats stats;
    
    public HollowStateFlee(EnemyStateController controller, NavMeshAgent agent, Vector3 targetPosition, HollowStats stats) : base(controller, agent)
    {
      this.targetPosition = targetPosition;
      this.stats = stats;
    }

    public override void Enter()
    {
        agent.destination = targetPosition;
    }

    public override void Tick()
    {
    
        if (agent.enabled && agent.remainingDistance <= agent.stoppingDistance)
        {
            controller.ChangeState(new HollowStateChargeShot(controller, agent,stats));
        }
    }

    public override void Exit()
    {
        
    }
}
