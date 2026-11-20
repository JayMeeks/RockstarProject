using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateStunned : State
{
    private float _stunTimer, _stunDuration;
  
    public HollowStateStunned(EnemyStateController controller, NavMeshAgent agent, float stunDuration) : base(controller, agent)
    {
      _stunDuration = stunDuration;
    }

    public override void Enter()
    {
        _stunTimer = _stunDuration;
    }

    public override void Tick()
    {
        if (_stunTimer <= 0)
        {
            controller.ChangeState(new HollowStateDecision(controller, agent));
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
