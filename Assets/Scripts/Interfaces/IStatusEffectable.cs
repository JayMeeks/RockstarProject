using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffectable
{
    public void AddEffect(StatusEffectBase effect);
    public void ApplyEffects(List<StatusEffectBase> effects);
    
    public void RemoveEffect(StatusEffectBase effect);
}
