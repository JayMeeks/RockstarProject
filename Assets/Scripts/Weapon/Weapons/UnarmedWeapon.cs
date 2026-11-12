using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class UnarmedWeapon : BaseWeapon
{
    [SerializeField] private UnarmedWeaponData data;
    [SerializeField] private LineRenderer siphonTetherLine;
    
    [SerializeField] private Material activeTetherMaterial, inactiveTetherMaterial;
    private bool siphonActive;

    [SerializeField] private LayerMask _enemyLayer;
    
    [Header("Input")]
    private PlayerInputA _playerInput;
    private PlayerInputA.PlayerDefaultActions _playerDefaultActions;

    private void Awake()
    {
        _playerInput = new PlayerInputA();
        _playerDefaultActions = _playerInput.PlayerDefault;
        
    }

    private void OnEnable()
    {
        siphonTetherLine.enabled = false;
        siphonActive = false;
        
        _playerDefaultActions.Enable();


        _playerDefaultActions.WeaponSpecial.started  += StartSpecial;
    }
    
    private void OnDisable()
    {
        _playerDefaultActions.Disable();
        _playerDefaultActions.WeaponSpecial.started  -= StartSpecial;
    }
    
    private void StartSpecial(InputAction.CallbackContext obj)
    {
        OnUseWeaponSpecial();
    }

   

    protected override void OnUseLightAttack()
    {
        return;
    }

    protected override void OnUseHeavyAttack()
    {
        return;
    }
    

    protected override void OnUseWeaponSpecial()
    {
        //Check if the player is looking at an enemy and within range
        //Also requires another tether to not already be active and player must not have enough health to form a weapon
        if (Physics.Raycast(
           origin: PlayerMovement.Instance.transform.position, 
           direction: Camera.main.transform.forward,
           hitInfo:   out RaycastHit hit, 
           maxDistance:  data.SiphonStartRange) 
           && siphonActive == false && PlayerHealthSystem.Instance.GetCurrentHealth() < 25.0f)
        {
            var enemy = hit.transform.gameObject.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                StartCoroutine(StartSiphon(enemy));
            }
            else
            {
                Debug.Log("Enemy component not found");
            }
     
        }
        else
        {
            Debug.Log("Siphon failed");
        }
    }



    private IEnumerator StartSiphon(EnemyBase enemy)
    {
        //Enable line renderer and set bool for siphon active
        siphonTetherLine.enabled = true;
        siphonActive = true;
        
        //Time variable
        float t = 0.0f;
        
        
        //Check enemy is alive and player cannot create a weapon yet
        while (enemy && PlayerHealthSystem.Instance.GetCurrentHealth() < 25.0f)
        {
            //Set line renderer start and end point to player and enemy
            siphonTetherLine.SetPosition(0, PlayerMovement.Instance.transform.position);
            siphonTetherLine.SetPosition(1,enemy.transform.position);
            
            float distance = Vector3.Distance(PlayerMovement.Instance.transform.position,enemy.transform.position);
    
    
            //If within range:
            //Change line colour
            //Check if time passed exceeds interval
            //If yes, heal player and reset timer
            if (distance <= data.SiphonDrainRange)
            {
                siphonTetherLine.material = activeTetherMaterial;
                if (t >= data.SiphonTickInterval)
                {
                    PlayerHealthSystem.Instance.HealPlayer(data.SiphonHealAmount);
                    t = 0.0f;
                }
            }
            //If not within range:
            //Change line colour
            //Reset timer
            else
            {
                siphonTetherLine.material = inactiveTetherMaterial;
                t = 0.0f;
            }
            t += Time.deltaTime;
            yield return null;
        }
        //Disable when enemy dies or player has enough health to create a weapon
        siphonTetherLine.enabled = false;
        siphonActive = false;

    }

    protected override void OnWeaponBreak()
    {
        return;
    }
}
