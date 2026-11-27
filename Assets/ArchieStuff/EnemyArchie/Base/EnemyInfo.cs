using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class EnemyInfo : MonoBehaviour, IDamageable, IStatusEffectable, IDropsBloodPool
{
    
    private List<StatusEffectBase> _activeStatusEffects = new List<StatusEffectBase>();
    private Queue<StatusEffectBase> _statusRemoveQueue = new Queue<StatusEffectBase>();


    protected int CurrentHealth, MaxHealth;
    protected float MoveSpeed;
    protected int bloodPoolValue;
    
    
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

        for (int i = 0; i < _statusRemoveQueue.Count; i++)
        {
            _activeStatusEffects.Remove(_statusRemoveQueue.Dequeue());
        }
    }

    public void RemoveEffect(StatusEffectBase effect)
    { 
        _statusRemoveQueue.Enqueue(effect);
    }

    public void AddBloodValue(int bloodValue)
    {
        bloodPoolValue += bloodValue;
    }

    public void CreateBloodPool(int bloodPoolValue)
    {
        var blood = Instantiate(Resources.Load<GameObject>("Prefabs/BloodPool"),transform.position,Quaternion.identity);
        var bloodPool = blood.GetComponent<BloodPool>();
        bloodPool.SetBloodValue(bloodPoolValue);
    }
}
