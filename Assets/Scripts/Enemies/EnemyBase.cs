using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public enum EnemyState
{
    Patrolling,
    Following,
    Attacking
}

public abstract class EnemyBase : MonoBehaviour
{
    protected int currentHp;
    protected int maxHp;

    [Header("Health Bar")]
    public Slider slider;
    public Canvas uiCanvas;

    [Header("Layers")] 
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    
    [SerializeField] protected EnemyData enemyData;
    
    [Header("References")] 
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform player;
    
    private const float PatrolPointRange = 10f;
    private NavMeshAgent _agent;
    private int _currentPatrolIndex;
    private bool _isWaiting;
    private EnemyState _state = EnemyState.Following;
    private float _timeSinceLostPlayer;
    private EnemySpawner spawner;
    
    protected bool _hasAttacked;
    public bool paused = false;

    protected virtual void Awake()
    {
     
        _agent = GetComponent<NavMeshAgent>();
        maxHp = 40;
        currentHp = maxHp;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      //  player = PlayerMovement.Instance.transform;
        FollowPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

      //  uiCanvas.transform.LookAt(transform.position - (player.position - transform.position));
      //  uiCanvas.transform.rotation = Quaternion.LookRotation(transform.position - player.position);
        slider.value = (float)currentHp / (float)maxHp;


        switch (_state)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (distanceToPlayer <= enemyData.detectionRange && CanSeePlayer())
                {
                    _state = EnemyState.Following;
                }
                break;

            case EnemyState.Following:
                FollowPlayer();
                if (CanAttack(distanceToPlayer))
                {
                    _state = EnemyState.Attacking;
                }
                break;

            case EnemyState.Attacking:
                PerformAttack();
                if (!_hasAttacked && !CanAttack(distanceToPlayer))
                {
                    _state = EnemyState.Following;
                    _agent.isStopped = false;
                }

                break;
        }
    }

    public void TakeDamage(int damage)
    {
   
       
        currentHp -= damage;

   
        if (currentHp <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
     if(spawner != null){   spawner.EnemyDead();}
        Destroy(gameObject);
    }

    
    protected abstract bool CanAttack(float distance);
    
    protected virtual void PerformAttack()
    {
        _agent.isStopped = true;
        var direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        
    }
    
    protected virtual IEnumerator ResetAttack()
    { 
        yield return new WaitUntil(() => !paused);
        _agent.isStopped = false;
        _state = EnemyState.Following;
        _hasAttacked = false;
    }
    
    private void FollowPlayer()
    {
        var direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        _agent.SetDestination(player.position);
        
    }
    
    private void Patrol()
    { 
        if (_isWaiting) return;
        
        if (!_agent.pathPending && _agent.remainingDistance <= enemyData.stopAtDistance) 
        { 
            StartCoroutine(WaitAtPatrolPoint());
        }
    }
    
    private IEnumerator WaitAtPatrolPoint() 
    { 
        _isWaiting = true; 
        _agent.isStopped = true;
        
        yield return new WaitForSeconds(enemyData.patrolWaitTime);
        yield return new WaitUntil(() => !paused);
        
        _agent.isStopped = false; 
        GoToRandomPatrolPoint();
        //GoToNextPatrolPoint(); 
        _isWaiting = false;
    }
    
    private void GoToRandomPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        float randomZ = Random.Range(-PatrolPointRange, PatrolPointRange);
        float randomX = Random.Range(-PatrolPointRange, PatrolPointRange);
            
        var randomPatrolPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(randomPatrolPoint, -transform.up, 2f, whatIsGround))
        {
        }
        _agent.SetDestination(randomPatrolPoint);
            
    }
    
    protected bool CanSeePlayer()
    {
        return IsFacingPlayer() && HasClearPathToPlayer();
    }

    private bool IsFacingPlayer()
    {
        var dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= enemyData.viewAngle / 2f;
    }

    private bool HasClearPathToPlayer()
    {
        var dirToPlayer = player.position - transform.position;

        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }

        return true;
    }

    public void SetSpawner(EnemySpawner enemySpawner)
    {
        spawner = enemySpawner;
    }

    private bool GroundCheck()
    {
        return Physics.CheckCapsule(transform.position, new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z), whatIsGround);
    }
}
