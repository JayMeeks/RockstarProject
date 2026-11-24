using UnityEngine;

namespace ArchieStuff.EnemyArchie
{
    public interface IStateController
    {
        public void ChangeState(IState newState);
    }

    public interface IStunnable
    {
        public void OnStunned(float stunTime);
    }

    public interface IKnockbackable
    {
        public void OnReceiveKnockback(Vector3 forceDirection, float forceMultiplier);
    }

    public abstract class EnemyStateController : MonoBehaviour , IStateController
    {
        public abstract void ChangeState(IState newState);
    }
}