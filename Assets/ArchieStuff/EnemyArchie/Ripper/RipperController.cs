using System.Collections;
using ArchieStuff.EnemyArchie;
using UnityEngine;
using UnityEngine.AI;

public class RipperController : EnemyStateController, IStunnable, IKnockbackable
{
    private NavMeshAgent _agent;
    private IState _currentState;
    private Rigidbody _rigidbody;
  
    public void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        
        _agent.enabled = true;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        
        ChangeState(new RipperStateDecision(this,_agent));
    }

    public void Update()
    {
        _currentState?.Tick();
    }
    
    public override void ChangeState(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        newState?.Enter();
    }

    public void OnStunned(float stunTime)
    {
        ChangeState(new RipperStateStunned(this,_agent, stunTime));
    }

    public void OnReceiveKnockback(Vector3 forceDirection, float forceMultiplier)
    {
        if (_agent.enabled)
        {
           _agent.enabled = false;
           _rigidbody.useGravity = true;
           _rigidbody.isKinematic = false;
           _rigidbody.AddForce(forceDirection * forceMultiplier, ForceMode.Impulse);
           StartCoroutine(WaitUntilGrounded());

        }
    }

    private IEnumerator WaitUntilGrounded()
    {
        //Wait a small period so that enemy has time to leave ground
        yield return new WaitForSeconds(0.1f);
        //Wait for enemy to hit the ground
        while (GetEnemyIsGrounded() == false)
        {
            yield return null;
        }
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _agent.enabled = true;
        _agent.Warp(transform.position);
    }

    private bool GetEnemyIsGrounded()
    {
        return Physics.Raycast(
            origin: _rigidbody.transform.position,
            direction: Vector3.down,
            maxDistance: 1.0f
        );
    }

}

