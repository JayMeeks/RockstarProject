using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateDecision : State
{
    private float fleeDistance = 10.0f;
    
    public HollowStateDecision(EnemyStateController controller, NavMeshAgent agent) : base(controller, agent)
    {
    }

    public override void Enter()
    {
        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        float distanceToPlayer = Vector3.Distance(agent.transform.position, playerPos);
        if (distanceToPlayer > fleeDistance)
        {
          //  Debug.Log("Enemy is far from player. Shooting");
            controller.ChangeState(new HollowStateChargeShot(controller, agent));
        }
        else
        {
            Vector3 horizontalDirectionToPlayer = new Vector3(
                playerPos.x - agent.transform.position.z,
                0,
                playerPos.z - agent.transform.position.z);
            Vector3 horizontalFleeDirection = -horizontalDirectionToPlayer;
            horizontalFleeDirection = horizontalFleeDirection.normalized;
            Vector3 initialGuess = agent.transform.position + (horizontalFleeDirection * fleeDistance);

            if (NavMesh.SamplePosition(initialGuess, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                Vector3 targetPosition = hit.position;

                if (Vector3.Distance(agent.transform.position, targetPosition) < distanceToPlayer)
                {
                  //  Debug.Log("Enemy in corner. Shooting");
                    controller.ChangeState(new HollowStateChargeShot(controller, agent));
                }
                else
                {
                  //  Debug.Log("Enemy has room to flee. Running away");
                    controller.ChangeState(new HollowStateFlee(controller, agent, targetPosition));
                }
                
            }
            else
            {
               // Debug.Log("Suitable flee location not found. Shooting");
                controller.ChangeState(new HollowStateChargeShot(controller, agent));
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
