using UnityEngine;

public abstract class StatusEffectBase
{
    protected GameObject targetObject;
    protected StatusEffectBase(GameObject targetObject)
    {
    this.targetObject = targetObject;
    }

    public abstract void OnAdd();
    public abstract void OnReAdd();
    
    public abstract void Tick();
    public abstract void OnRemove();
    
}
