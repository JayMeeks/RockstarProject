using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateStunned : State
{
    private float _stunTimer, _stunDuration;
    private HollowStats stats;
  
    public HollowStateStunned(EnemyStateController controller, NavMeshAgent agent, float stunDuration, HollowStats stats) : base(controller, agent)
    {
      _stunDuration = stunDuration;
      this.stats = stats;
    }

    public override void Enter()
    {
        _stunTimer = _stunDuration;
    }

    public override void Tick()
    {
        if (_stunTimer <= 0)
        {
            controller.ChangeState(new HollowStateDecision(controller, agent,stats));
        }
        else
        {
            _stunTimer -= Time.deltaTime;
        }
       
    }

    public override void Exit()
    {
        
    }
}
