using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyInfo : MonoBehaviour
{
   
    //Move to scriptable object
    protected int CurrentHealth, MaxHealth;
    protected float MoveSpeed;
    protected int HealthGivenOnKill;
    
    
    
    
    
    
    protected abstract void Initialize();
    public abstract void TakeDamage(int damage);
    protected abstract void OnDeath();
}
