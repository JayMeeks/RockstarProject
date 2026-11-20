using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class HollowStateShoot : State
{
   
    private float shotInterval = 0.5f;
    private float shotTimer = 0f;
    private int shotCount = 0;
    private int shotsToTake;
    private GameObject bulletPrefab;

   
    public HollowStateShoot(EnemyStateController controller, NavMeshAgent agent) : base(controller, agent)
    {
      this.controller = controller;
    }

    public override void Enter()
    {
        shotTimer = shotInterval;
        shotCount = 0;
        shotsToTake = 2;
        bulletPrefab = Resources.Load<GameObject>("Prefabs/HomingBullet");
    }

    public override void Tick()
    {
        if (shotTimer <= 0.0f)
        {
            Object.Instantiate(bulletPrefab, agent.gameObject.transform.position, Quaternion.identity);
            shotCount++;
            if (shotCount < shotsToTake)
            {
                shotTimer = shotInterval;
            }
            else if (shotCount == shotsToTake)
            {
                controller.ChangeState(new HollowStateDecision(controller, agent));
            }
        }
        else
        {
            shotTimer -= Time.deltaTime;
        }
    }

    public override void Exit()
    {
    
    }

}

    
/*  if (shotTimer <= 0.0f)
    {
        Object.Instantiate(bulletPrefab, agent.transform.position, Quaternion.identity);
        Vector3 playerPos = PlayerMovement.Instance.transform.position;
        float distanceToPlayer = Vector3.Distance(agent.transform.position, playerPos);
        if (distanceToPlayer < 10.0f && distanceToPlayer > 2.5f)
        {
            controller.ChangeState(new HollowStateFlee(controller, agent, 10.0f));
        }
        if (distanceToPlayer < 2.5f)
        {
            controller.ChangeState(new HollowStateMeleeAttack(controller, agent));
        }
        else
        {
            shotTimer = shotInterval;
        }
    }
    else
    {
        shotTimer -= Time.deltaTime;
    }*/
