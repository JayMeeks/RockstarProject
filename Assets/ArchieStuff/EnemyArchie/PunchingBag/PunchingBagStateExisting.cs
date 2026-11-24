using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class PunchingBagStateExisting : State
{

    private PunchingBagStats stats;
    
    public PunchingBagStateExisting(EnemyStateController controller, NavMeshAgent agent, PunchingBagStats stats) : base(controller, agent)
    {

        this.stats = stats;
    }

    public override void Enter()
    {

    }

    public override void Tick()
    {
        
    }

    public override void Exit()
    {
        
    }
}