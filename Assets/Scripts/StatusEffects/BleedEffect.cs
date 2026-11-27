using UnityEngine;

public class BleedEffect : StatusEffectBase
{

    private int damage;
    private int tickCount;

    private float damageTimer = 0;
    private bool hasBeenRehit = false;




    private IStatusEffectable targetStatuses;
    private IDamageable targetHealth;
    private IDropsBloodPool targetBloodPool;
    
    




    public BleedEffect(GameObject targetObject, int damagePerTick) : base(targetObject)
    {
        this.damage = damagePerTick;
    }

    public override void OnAdd()
    {
        tickCount = 5;
        damageTimer = 1.0f;
        hasBeenRehit = false;

        
        targetHealth = targetObject.GetComponentInParent<IDamageable>();
        targetStatuses = targetObject.GetComponentInParent<IStatusEffectable>();
        targetBloodPool = targetObject.GetComponentInParent<IDropsBloodPool>();

       
    }

    public override void OnReAdd()
    {
       if(hasBeenRehit){return;}
       tickCount = 5;
       damage *= 2;
       hasBeenRehit = true;
    }

    public override void Tick()
    {

        if (damageTimer <= 0.0f && tickCount > 0)
        {
            targetBloodPool?.AddBloodValue(damage);
            targetHealth?.TakeDamage(damage);
           
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
        targetStatuses?.RemoveEffect(this);
    }
}
