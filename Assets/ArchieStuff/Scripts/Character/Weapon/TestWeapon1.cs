using System;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestWeapon1 : BaseWeapon
{
    [Header("Animation stuff")]
    [SerializeField] Animator hammerAnimator;
    private float _chargeTimer;
    private bool _isChargingHammerLight, _isChargingHammerHeavy;
    
    [Header("Input")]
    private PlayerInput _playerInput;
    private PlayerInput.PlayerDefaultActions _playerDefaultActions;
    
    
    protected void Awake()
    {
        _playerInput = new PlayerInput();
        _playerDefaultActions = _playerInput.PlayerDefault;
    }

    protected void OnEnable()
    {
        _playerDefaultActions.Enable();
        _playerDefaultActions.Mouse1.started += StartLightHammerCharge;
        _playerDefaultActions.Mouse1.canceled += EndLightHammerCharge;
        
        _playerDefaultActions.Mouse2.started += StartHeavyHammerCharge;
        _playerDefaultActions.Mouse2.canceled += EndHeavyHammerCharge;
    }

    protected void OnDisable()
    {
        _playerDefaultActions.Disable();
        _playerDefaultActions.Mouse1.started -= StartLightHammerCharge;
        _playerDefaultActions.Mouse1.canceled -= EndLightHammerCharge;
        
        _playerDefaultActions.Mouse2.started -= StartHeavyHammerCharge;
        _playerDefaultActions.Mouse2.canceled -= EndHeavyHammerCharge;
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
        _isChargingHammerHeavy = true;
    }
    
    private void EndHeavyHammerCharge(InputAction.CallbackContext ctx)
    {
        _isChargingHammerHeavy = false;
        _chargeTimer = 0;
    }


    protected override void OnUseLightAttack() //Probably called from animation event?
    {
        currentDurability--;
        Debug.Log("Light attack used");
        
                    /* public void CreateWeaponHitbox()     --TEMP, stolen from player controller, here as a reminder to myself lol - mid refactor
             {
              Debug.Log("Weapon hitbox created");
              Instantiate(hammerHitbox, transform.position, playerCamera.transform.rotation);
             }*/
        
        if(currentDurability == 0){OnWeaponBreak();}
    }

    protected override void OnUseHeavyAttack() //Probably called from animation event?
    {
        Debug.Log("Heavy attack used");
    }
    
    protected override void OnWeaponBreak()
    {
        PlayerController.Instance.EquippedWeapon = null;
        Destroy(gameObject);
    }
}
