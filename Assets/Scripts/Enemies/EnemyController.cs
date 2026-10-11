using System.Collections;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public enum EnemyState
    {
        Patrolling,
        Following,
        Attacking,
    }

    public class EnemyController : MonoBehaviour
    { 
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int Attacking = Animator.StringToHash("Attacking");

        [Header("References")] 
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private Transform player;
        [SerializeField] private GameObject projectile;
    
        [Header("Settings")]
        [SerializeField] private float patrolWaitTime = 2f;
        [SerializeField] private float stopAtDistance = 0.5f;
        [SerializeField] private float detectionRange = 5f;
        [SerializeField] private float viewAngle = 60f;
        [SerializeField] private float losePlayerTime = 3f;
        [SerializeField] private float attackRange = 1.2f;

        [Header("Layers")] 
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private LayerMask whatIsPlayer;

        private const float PatrolPointRange = 10f;

        private NavMeshAgent _agent;
        private Animator _animator;
        private int _currentPatrolIndex;
        private bool _isWaiting;
        private EnemyState _state = EnemyState.Patrolling;
        private float _timeSinceLostPlayer;

        private const float TimeBetweenAttacks = 2f;
        //private bool _isAttacking;
        private bool _hasAttacked;


        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            _agent = GetComponent<NavMeshAgent>(); 
            _animator = GetComponent<Animator>();
        }
    
        private void Start() 
        { 
            GoToRandomPatrolPoint();
        }
    
        private void Update() 
        { 
            var distanceToPlayer = Vector3.Distance(player.position, transform.position);

            switch (_state)
            {
                case EnemyState.Patrolling:
                    Patrol();
                    if (distanceToPlayer <= detectionRange && CanSeePlayer())
                    {
                        _state = EnemyState.Following;
                    }
                    break;
                
                case EnemyState.Following:
                    FollowPlayer();
                    if (distanceToPlayer <= attackRange)
                    {
                        _state = EnemyState.Attacking;
                        //StartAttack();
                    }
                    if (!CanSeePlayer())
                    {
                        _timeSinceLostPlayer += Time.deltaTime;
                        if (_timeSinceLostPlayer >= losePlayerTime)
                        {
                            _state = EnemyState.Patrolling;
                            GoToClosestPatrolPoint();
                        }
                    }
                    else
                    {
                        _timeSinceLostPlayer = 0f;
                    }
                    break;
                
                case EnemyState.Attacking:
                    Attack();
                    if (!_hasAttacked && distanceToPlayer > attackRange)
                    {
                        _state = EnemyState.Following;
                        _agent.isStopped = false;
                    }
                    
                    break;
            }

            //UpdateAnimations();
        }

        private void StartAttack()
        {
            _agent.isStopped = true;
            //_isAttacking = true;
            //_animator.SetTrigger(Attacking);
        }

        private void Attack()
        {
            _agent.isStopped = true;
            var direction = (player.position - transform.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (!_hasAttacked)
            {
                GameObject bullet = Instantiate(projectile, transform.position, Quaternion.identity);
                
                
                _hasAttacked = true;
                StartCoroutine(ResetAttack());
            }
        }

        private IEnumerator ResetAttack()
        { 
            yield return new WaitForSeconds(TimeBetweenAttacks);
            _agent.isStopped = false;
            _state = EnemyState.Patrolling;
            _hasAttacked = false;
        }

        private void OnAttackAnimationEnd()
        {
            //_isAttacking = false;
        }
        
        private void FollowPlayer()
        {
            _agent.SetDestination(player.position);
        }
        
        private void Patrol()
        { 
            if (_isWaiting) return;
        
            if (!_agent.pathPending && _agent.remainingDistance <= stopAtDistance) 
            { 
                StartCoroutine(WaitAtPatrolPoint());
            }
        }
    
        private IEnumerator WaitAtPatrolPoint() 
        { 
            _isWaiting = true; 
            _agent.isStopped = true;
        
            yield return new WaitForSeconds(patrolWaitTime);
        
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
        
        private void GoToClosestPatrolPoint()
        {
            if (patrolPoints.Length == 0) return;
            var closestIndex = 0;
            var closestDistance = float.MaxValue;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                var distance = Vector3.Distance(transform.position, patrolPoints[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            _currentPatrolIndex = closestIndex;
            _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
        }
        
        private void GoToNextPatrolPoint() 
        { 
            if (patrolPoints.Length == 0) return;
        
            _agent.SetDestination(patrolPoints[_currentPatrolIndex].position); 
            _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
        }
    
        private void UpdateAnimations() 
        { 
            var isWalking = _agent.velocity.sqrMagnitude > 0.01f; 
            _animator.SetBool(IsWalking, isWalking);
        }

        private bool CanSeePlayer()
        {
            return IsFacingPlayer() && HasClearPathToPlayer();
        }

        private bool IsFacingPlayer()
        {
            var dirToPlayer = (player.position - transform.position).normalized;
            var angle = Vector3.Angle(transform.forward, dirToPlayer);
            return angle <= viewAngle / 2f;
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
    }
}