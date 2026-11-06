using System;
using System.Collections;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class HammerWeapon : BaseWeapon
{
    public HammerWeaponData weaponData;
   
    [Header("Animation stuff")]
    [SerializeField] Animator hammerAnimator;
    private float _chargeTimer;
    private bool _isChargingHammerLight, _isChargingHammerHeavy;



    [Header("Material")] 
    [SerializeField]private Material _weaponMaterial;
    private float _dissolveAlpha;
    
    
    [Header("Input")]
    private PlayerInputA _playerInput;
    private PlayerInputA.PlayerDefaultActions _playerDefaultActions;

    [Header("Attacks")]
    public GameObject LightAttack,HeavyAttack;

    private bool useEnhancedHeavy;
 
    
    
    protected void Awake()
    {
        _playerInput = new PlayerInputA();
        _playerDefaultActions = _playerInput.PlayerDefault;
        
        _weaponMaterial.SetFloat("_saturationAlpha", 1.0f);
        currentDurability = weaponData.maxDurability;

        useEnhancedHeavy = false;
        
        StartCoroutine(DissolveIn());
    }

    protected void OnEnable()
    {
        _playerDefaultActions.Enable();
        _playerDefaultActions.Mouse1.started += StartLightHammerCharge;
        _playerDefaultActions.Mouse1.canceled += EndLightHammerCharge;
        
        _playerDefaultActions.Mouse2.started += StartHeavyHammerCharge;
        _playerDefaultActions.Mouse2.canceled += EndHeavyHammerCharge;
        
        _playerDefaultActions.WeaponSpecial.started  += StartSpecial;
    }

    protected void OnDisable()
    {
        _playerDefaultActions.Disable();
        _playerDefaultActions.Mouse1.started -= StartLightHammerCharge;
        _playerDefaultActions.Mouse1.canceled -= EndLightHammerCharge;
        
        _playerDefaultActions.Mouse2.started -= StartHeavyHammerCharge;
        _playerDefaultActions.Mouse2.canceled -= EndHeavyHammerCharge;
        
        _playerDefaultActions.WeaponSpecial.started  -= StartSpecial;
    }

    protected void Update()
    {
        _chargeTimer += _isChargingHammerLight || _isChargingHammerHeavy ? Time.deltaTime : 0; //Could be moved out of update loop? Not sure if there's a way to do "On mouse held", fine for now
        hammerAnimator.SetBool("IsChargingLight", _isChargingHammerLight);//^^^
        hammerAnimator.SetBool("IsChargingHeavy", _isChargingHammerHeavy);

    }
    
    private void StartLightHammerCharge(InputAction.CallbackContext ctx)
    {
        _isChargingHammerLight = true;
    }
    
    private void EndLightHammerCharge(InputAction.CallbackContext ctx)
    {
        _isChargingHammerLight = false;
        _chargeTimer = 0;
    }
    
    private void StartHeavyHammerCharge(InputAction.CallbackContext ctx)
    {
        if (PlayerHealthSystem.Instance.TrySpendHealth(weaponData.heavyAttackHpCost, out var healthUsed) == false)
        {
            return;
        }
        
        _isChargingHammerHeavy = true;
    }
    
    private void EndHeavyHammerCharge(InputAction.CallbackContext ctx)
    {
        _isChargingHammerHeavy = false;
        _chargeTimer = 0;
         
    }
    
    private void StartSpecial(InputAction.CallbackContext ctx)
    {
        OnUseWeaponSpecial();
    }


    protected override void OnUseLightAttack() //Probably called from animation event?
    {
        ReduceDurability();
        UpdateMatProps();

        var attackObj = Instantiate(LightAttack);
        attackObj.GetComponent<HammerLightAttackBehaviour>().Attack(PlayerMovement.Instance.transform.position,Camera.main.transform.rotation,Camera.main.transform.forward,out bool shouldHeal, weaponData.playerHpGainMaxRange);
        if (shouldHeal) { PlayerHealthSystem.Instance.HealPlayer(weaponData.playerHpGainOnHit);}
        
        if(currentDurability == 0){OnWeaponBreak();}
    }

    protected override void OnUseHeavyAttack() //Probably called from animation event?
    {
        if (PlayerHealthSystem.Instance.TrySpendHealth(weaponData.heavyAttackHpCost, out var healthUsed) == false)
        {
            return;
        }
        PlayerHealthSystem.Instance.DamagePlayer(healthUsed);

        Vector3 spawnPos = new Vector3(
            PlayerMovement.Instance.transform.position.x,
            PlayerMovement.Instance.transform.position.y , 
            PlayerMovement.Instance.transform.position.z);

        Quaternion startRot;// = Quaternion.LookRotation(PlayerController.Instance.GetPlayerForward(), Vector3.up);

       if (useEnhancedHeavy)
       {
           for (int i = 0; i < 12; i++)
           {
               startRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y + i * (360f/12f), 0);
               var attackObj = Instantiate(HeavyAttack,spawnPos,startRot);
               attackObj.GetComponent<HammerHeavyAttackBehaviour>().Attack(useEnhancedHeavy);

           }
       }
       else
       {
           startRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
           var attackObj = Instantiate(HeavyAttack, spawnPos, startRot);
           attackObj.GetComponent<HammerHeavyAttackBehaviour>().Attack(useEnhancedHeavy);
       }
       useEnhancedHeavy = false;


    
       
        
        ReduceDurability();
        UpdateMatProps();
       
        
  
        if(currentDurability == 0){OnWeaponBreak();}
        
    }

    protected override void OnUseWeaponSpecial()
    {
       PlayerMovement.Instance.ApplyImpulse(Vector3.up,1);
       useEnhancedHeavy = true;
       StartCoroutine(WaitUntilGrounded());
    }
    
    protected override void OnWeaponBreak()
    {
        PlayerWeaponSystem.Instance.EquippedWeapon = null;
        Destroy(gameObject);
    }

    private IEnumerator WaitUntilGrounded()
    {
        //Wait a small period so that player has time to leave ground
        yield return new WaitForSeconds(0.1f);
        //Wait for player to hit the ground
        while (PlayerMovement.Instance.IsGrounded() == false)
        {
            yield return null;
        }
        //Small grace period between hitting the ground and disabling enhanced heavy
        yield return new WaitForSeconds(0.35f);
        //Disable enhanced heavy
        if(useEnhancedHeavy){useEnhancedHeavy=false;}
    }

    private IEnumerator DissolveIn()
    {
        _dissolveAlpha = 0;
        while (_dissolveAlpha < 1)
        {
            _dissolveAlpha += Time.deltaTime;
            _weaponMaterial.SetFloat("_dissolveAlphaThreshold", _dissolveAlpha);
            yield return null;
        }
    }
    
    private void UpdateMatProps()
    { 
        _weaponMaterial.SetFloat("_saturationAlpha", (float)currentDurability/weaponData.maxDurability);
    }

    private void ReduceDurability()
    {
        currentDurability--;
    }
    
    private void ReduceDurability(int durability)
    {
        currentDurability -= durability;
    }
}
