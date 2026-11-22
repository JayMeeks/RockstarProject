using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateDecision : State
{
    private HollowStats stats;
    
    public HollowStateDecision(EnemyStateController controller, NavMeshAgent agent, HollowStats stats) : base(controller, agent)
    {
         this.stats = stats;
    }

    public override void Enter()
    {
        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        float distanceToPlayer = Vector3.Distance(agent.transform.position, playerPos);
        if (distanceToPlayer > stats.playerCheckDistance)
        {
          //  Debug.Log("Enemy is far from player. Shooting");
            controller.ChangeState(new HollowStateChargeShot(controller, agent,stats));
        }
        else
        {
            Vector3 horizontalDirectionToPlayer = new Vector3(
                playerPos.x - agent.transform.position.z,
                0,
                playerPos.z - agent.transform.position.z);
            Vector3 horizontalFleeDirection = -horizontalDirectionToPlayer;
            horizontalFleeDirection = horizontalFleeDirection.normalized;
            Vector3 initialGuess = agent.transform.position + (horizontalFleeDirection * stats.fleeDistance);

            if (NavMesh.SamplePosition(initialGuess, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                Vector3 targetPosition = hit.position;

                if (Vector3.Distance(agent.transform.position, targetPosition) < distanceToPlayer)
                {
                  //  Debug.Log("Enemy in corner. Shooting");
                    controller.ChangeState(new HollowStateChargeShot(controller, agent,stats));
                }
                else
                {
                  //  Debug.Log("Enemy has room to flee. Running away");
                    controller.ChangeState(new HollowStateFlee(controller, agent, targetPosition,stats));
                }
                
            }
            else
            {
               // Debug.Log("Suitable flee location not found. Shooting");
                controller.ChangeState(new HollowStateChargeShot(controller, agent,stats));
            }
   
        }
    }

    public override void Tick()
    {
        
    }

    public override void Exit()
    {
        
    }
}
