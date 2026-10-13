using System;
using Enemies;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public interface IDamageable
{
    void TakeDamage(int damage);
}

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    protected int currentHp;
    protected int maxHp;
    public abstract void TakeDamage(int damage);
}

public class TestEnemy : BaseEnemy, IDamageable
{
    public Slider slider;
    public Canvas uiCanvas;
    
    private float timeSinceLastHit;

    public GameObject BloodOrb;

    public LayerMask groundLayer;
    private EnemyController enemyController;
    private NavMeshAgent nma;
    private bool leftFloor = false;
    private Rigidbody rb;
    
    private void Awake()
    {
        maxHp = 100;
        currentHp = maxHp;
        enemyController = GetComponent<EnemyController>();
        nma = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }
    
    
   
    void Update()
    {
        GameObject target = PlayerController.Instance.gameObject;
        Vector3 targetPos = target.transform.position;
        
       //uiCanvas.transform.LookAt(transform.position - (targetPos - transform.position));
       uiCanvas.transform.rotation = Quaternion.LookRotation(transform.position - targetPos);
       slider.value = (float)currentHp/(float)maxHp;
       
       timeSinceLastHit += Time.deltaTime;

       while (timeSinceLastHit > 1.5f & currentHp < maxHp)
       {
           currentHp++;
       }

       if (nma.enabled == false)
        {

            bool grounded = GroundCheck();
            if (grounded)
            {
                if (leftFloor)
                {
                    nma.enabled = true;
                    enemyController.enabled = true;
                    enemyController.paused = false;
                    rb.isKinematic = true;
                    leftFloor = false;
                    rb.position = getClosestPointToNavMesh();
                }

            }
            else
            {
                if (!leftFloor)
                {
                    leftFloor = true;
                }
            }
        }
    }


    public override void TakeDamage(int damage)
    {
        Debug.Log(damage);
        timeSinceLastHit = 0;
        currentHp -= damage;
        if (damage >= 50)
        {
            SpawnBloodOrb();
        }

        if (currentHp <= 0)
        {
          //  Destroy(gameObject);
        }
    }

    private void SpawnBloodOrb()
    {
        

       
     
        Vector3 spawnOffset = new Vector3(
            Random.Range(-1.0f, 1.0f),
            0,
            Random.Range(-1.0f, 1.0f));
        
        spawnOffset = Vector3.Normalize(spawnOffset);
        
        
        GameObject.Instantiate(BloodOrb, gameObject.transform.position + spawnOffset, Quaternion.identity);


    }

    private bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position, 1.2f, groundLayer);
    }

    private Vector3 getClosestPointToNavMesh()
    {
        Vector3 closestPoint = Vector3.zero;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            closestPoint = hit.position;
        }
        else
        {
            Debug.Log("no navmesh found");
        }

        return closestPoint;
    }
}
