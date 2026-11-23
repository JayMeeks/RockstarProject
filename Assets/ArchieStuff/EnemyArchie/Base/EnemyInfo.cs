using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class EnemyInfo : MonoBehaviour, IDamageable, IStatusEffectable
{
    
    private List<StatusEffectBase> _activeStatusEffects = new List<StatusEffectBase>();


    protected int CurrentHealth, MaxHealth;
    protected float MoveSpeed;
    protected int HealthGivenOnKill;
    
    //TEMP
    [Header("Health Bar")]
    public Slider slider;
    public Canvas uiCanvas;

    public void Update()
    {
        uiCanvas.transform.LookAt(transform.position - (PlayerMovement.Instance.transform.position - transform.position)); 
        uiCanvas.transform.rotation = Quaternion.LookRotation(transform.position - PlayerMovement.Instance.transform.position);
        slider.value = (float)CurrentHealth / (float)MaxHealth;
        
        ApplyEffects(_activeStatusEffects);
    }
    
  


    protected abstract void Initialize();
    public abstract void TakeDamage(int damage);
    protected abstract void OnDeath();
    
    public void AddEffect(StatusEffectBase effect)
    {
        if (_activeStatusEffects.Any(x => x.GetType() == effect.GetType()))
        {
            StatusEffectBase existingEffect = _activeStatusEffects.FirstOrDefault(x => x.GetType() == effect.GetType());
            if (existingEffect != null)
            {
                existingEffect.OnReAdd();
            }
           
        }
        else
        {
            _activeStatusEffects.Add(effect);
            effect.OnAdd();
        }
       
        
    }

    public void ApplyEffects(List<StatusEffectBase> effects)
    {
        //Debug.Log("Applying " + effects.Count + " status effects");
        foreach (StatusEffectBase effect in _activeStatusEffects)
        {
            effect?.Tick();
        }
    }

    public void RemoveEffect(StatusEffectBase effect)
    { 
        _activeStatusEffects.Remove(effect);
    }
}
