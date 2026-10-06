using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public interface IDamageable
{
    void TakeDamage(int damage);
}

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    protected int currentHp;
    protected int maxHp;
    public abstract void TakeDamage(int damage);
}

public class TestEnemy : BaseEnemy, IDamageable
{
    public Slider slider;
    public Canvas uiCanvas;
    
    private float timeSinceLastHit;

    public GameObject BloodOrb;
    
    private void Awake()
    {
        maxHp = 100;
        currentHp = maxHp;
    }
    
    
   
    void Update()
    {
        GameObject target = PlayerController.Instance.gameObject;
        Vector3 targetPos = target.transform.position;
        
       //uiCanvas.transform.LookAt(transform.position - (targetPos - transform.position));
       uiCanvas.transform.rotation = Quaternion.LookRotation(transform.position - targetPos);
       slider.value = (float)currentHp/(float)maxHp;
       
       timeSinceLastHit += Time.deltaTime;

       while (timeSinceLastHit > 1.5f & currentHp < maxHp)
       {
           currentHp++;
       }
      
        
    }


    public override void TakeDamage(int damage)
    {
        Debug.Log(damage);
        timeSinceLastHit = 0;
        currentHp -= damage;
        if (damage >= 50)
        {
            SpawnBloodOrb();
        }

        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void SpawnBloodOrb()
    {
        

       
     
        Vector3 spawnOffset = new Vector3(
            Random.Range(-1.0f, 1.0f),
            0,
            Random.Range(-1.0f, 1.0f));
        
        spawnOffset = Vector3.Normalize(spawnOffset);
        
        
        GameObject.Instantiate(BloodOrb, gameObject.transform.position + spawnOffset, Quaternion.identity);


    }
    
}
