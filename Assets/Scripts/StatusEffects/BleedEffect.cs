using UnityEngine;

public class BleedEffect : StatusEffectBase
{

    private int damage;
    private int tickCount;

    private float damageTimer = 0;


    private IStatusEffectable targetStatuses;
    private IDamageable targetHealth;
    
    




    public BleedEffect(GameObject targetObject, int damagePerTick) : base(targetObject)
    {
        this.damage = damagePerTick;
    }

    public override void OnAdd()
    {
        tickCount = 5;
      
        damageTimer = 1.0f;
        
        targetHealth = targetObject.GetComponent<IDamageable>();
        targetStatuses = targetObject.GetComponent<IStatusEffectable>();

       
    }

    public override void OnReAdd()
    {
        tickCount = 5;
    }

    public override void Tick()
    {

        if (damageTimer <= 0.0f && tickCount > 0)
        {
            targetHealth.TakeDamage(damage);
            damageTimer = 1.0f;
            tickCount--;
        }
        else if (damageTimer >= 0.0f && tickCount > 0)
        {
            damageTimer -= Time.deltaTime;
        }

        if (tickCount <= 0)
        {
            OnRemove();
        }
    }

    public override void OnRemove()
    {
        targetStatuses.RemoveEffect(this);
    }
}
