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
    [SerializeField] private Transform playerTransform;
    
    private const float _patrolPointRange = 10f;
    private NavMeshAgent _agent;
    private int _currentPatrolIndex;
    private bool _isWaiting;
    private EnemyState _state = EnemyState.Following;
    private float _timeSinceLostPlayer;
    private EnemySpawner _spawner;
    
    protected bool hasAttacked;
    public bool paused = false;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        //Health system will be changed later
        maxHp = 40;
        currentHp = maxHp;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = PlayerMovement.Instance.transform;
        FollowPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        var _distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        uiCanvas.transform.LookAt(transform.position - (playerTransform.position - transform.position));
        uiCanvas.transform.rotation = Quaternion.LookRotation(transform.position - playerTransform.position);
        slider.value = (float)currentHp / (float)maxHp;


        //State Controller - will be changed at later date into a separate state machine script (potentially own scripts for each state)
        switch (_state)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (_distanceToPlayer <= enemyData.detectionRange && CanSeePlayer())
                {
                    _state = EnemyState.Following;
                }
                break;

            case EnemyState.Following:
                FollowPlayer();
                if (CanAttack(_distanceToPlayer))
                {
                    _state = EnemyState.Attacking;
                }
                break;

            case EnemyState.Attacking:
                PerformAttack();
                if (!hasAttacked && !CanAttack(_distanceToPlayer))
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
        _spawner.EnemyDead();
        Destroy(gameObject);
    }

    //For use with each child class to check whether the player is in range of the enemy
    protected abstract bool CanAttack(float distance);
    
    //Child class overrides for respective attacks, base function ensures that the enemy is facing the player before attacking
    protected virtual void PerformAttack()
    {
        _agent.isStopped = true;
        var _direction = (playerTransform.position - transform.position).normalized;
        _direction.y = 0f;

        if (_direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_direction);
        }
        
        
    }
    
    //Attack cooldown - child can override with their respective time for cooldown
    protected virtual IEnumerator ResetAttack()
    { 
        yield return new WaitUntil(() => !paused);
        _agent.isStopped = false;
        _state = EnemyState.Following;
        hasAttacked = false;
    }
    
    private void FollowPlayer()
    {
        var _direction = (playerTransform.position - transform.position).normalized;
        _direction.y = 0f;

        if (_direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_direction);
        }
        _agent.SetDestination(playerTransform.position);
        
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

        float _randomZ = Random.Range(-_patrolPointRange, _patrolPointRange);
        float _randomX = Random.Range(-_patrolPointRange, _patrolPointRange);
            
        var _randomPatrolPoint = new Vector3(transform.position.x + _randomX, transform.position.y, transform.position.z + _randomZ);

        if (Physics.Raycast(_randomPatrolPoint, -transform.up, 2f, whatIsGround))
        {
        }
        _agent.SetDestination(_randomPatrolPoint);
            
    }
    
    protected bool CanSeePlayer()
    {
        return IsFacingPlayer() && HasClearPathToPlayer();
    }

    private bool IsFacingPlayer()
    {
        var _dirToPlayer = (playerTransform.position - transform.position).normalized;
        var _angle = Vector3.Angle(transform.forward, _dirToPlayer);
        return _angle <= enemyData.viewAngle / 2f;
    }

    //Checks no obstacles are in the way between the enemy and player (Ranged enemy uses this to prevent projectiles being fired at walls) 
    private bool HasClearPathToPlayer()
    {
        var _dirToPlayer = playerTransform.position - transform.position;

        if (Physics.Raycast(transform.position, _dirToPlayer.normalized, out RaycastHit _hit, _dirToPlayer.magnitude))
        {
            return _hit.transform == playerTransform;
        }

        return true;
    }

    public void SetSpawner(EnemySpawner enemySpawner)
    {
        _spawner = enemySpawner;
    }

    private bool GroundCheck()
    {
        return Physics.CheckCapsule(transform.position, new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z), whatIsGround);
    }
}
