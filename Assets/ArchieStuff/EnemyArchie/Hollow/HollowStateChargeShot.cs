using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateChargeShot : State
{
    private float chargeTimer;
    private HollowStats stats;
    

    public HollowStateChargeShot(EnemyStateController controller, NavMeshAgent agent, HollowStats stats) : base(controller, agent)
    {
        this.stats = stats;
    }

    public override void Enter()
    {
        chargeTimer = stats.shotChargeTime;
    }

    public override void Tick()
    {
        chargeTimer -= Time.deltaTime;
        if (chargeTimer <= 0.0f)
        {
            controller.ChangeState(new HollowStateShoot(controller, agent,stats ));
        }
    }

    public override void Exit()
    {
       
    }
}
