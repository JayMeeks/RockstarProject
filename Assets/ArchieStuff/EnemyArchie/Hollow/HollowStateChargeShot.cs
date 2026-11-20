using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateChargeShot : State
{
    private float chargeTime;
    

    public HollowStateChargeShot(EnemyStateController controller, NavMeshAgent agent) : base(controller, agent)
    {
    }

    public override void Enter()
    {
        chargeTime = 0.5f;
    }

    public override void Tick()
    {
        chargeTime -= Time.deltaTime;
        if (chargeTime <= 0.0f)
        {
            controller.ChangeState(new HollowStateShoot(controller, agent ));
        }
    }

    public override void Exit()
    {
       
    }
}
