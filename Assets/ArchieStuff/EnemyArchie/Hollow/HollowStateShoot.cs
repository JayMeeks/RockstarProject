using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateShoot : State
{
    private HollowStats stats;
   

    private float shotIntervalTimer = 0f;
    private int shotCount;

    private GameObject bulletPrefab;

   
    public HollowStateShoot(EnemyStateController controller, NavMeshAgent agent, HollowStats stats) : base(controller, agent)
    {
      this.stats = stats;
    }

    public override void Enter()
    {
        shotIntervalTimer = stats.shotInterval;

        bulletPrefab = Resources.Load<GameObject>("Prefabs/HomingBullet");
    }

    public override void Tick()
    {
        if (shotIntervalTimer <= 0.0f)
        {
            Object.Instantiate(bulletPrefab, agent.gameObject.transform.position, Quaternion.identity);
            shotCount++;
            if (shotCount < stats.shotCount)
            {
                shotIntervalTimer = stats.shotInterval;
            }
            else if (shotCount == stats.shotCount)
            {
                controller.ChangeState(new HollowStateDecision(controller, agent,stats));
            }
        }
        else
        {
            shotIntervalTimer -= Time.deltaTime;
        }
    }

    public override void Exit()
    {
    
    }

}

    
