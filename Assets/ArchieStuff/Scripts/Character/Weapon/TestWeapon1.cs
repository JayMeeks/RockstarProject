using System;
using System.Collections;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestWeapon1 : BaseWeapon
{
    [Header("Animation stuff")]
    [SerializeField] Animator hammerAnimator;
    private float _chargeTimer;
    private bool _isChargingHammerLight, _isChargingHammerHeavy;



    [Header("Material")] 
    [SerializeField]private Material _weaponMaterial;
    private float _dissolveAlpha;
    
    
    [Header("Input")]
    private PlayerInput _playerInput;
    private PlayerInput.PlayerDefaultActions _playerDefaultActions;

    [Header("Attacks")]
    [SerializeField] private GameObject hammerLightAttackHitbox;

    [SerializeField] private GameObject hammerHeavyAttackHitbox;
    
    
    protected void Awake()
    {
        _playerInput = new PlayerInput();
        _playerDefaultActions = _playerInput.PlayerDefault;
        
        _weaponMaterial.SetFloat("_saturationAlpha", 1.0f);
        
        StartCoroutine(DissolveIn());
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
        if (PlayerController.Instance.TrySpendHealth(heavyAttackHpCost, out var healthUsed) == false)
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


    protected override void OnUseLightAttack() //Probably called from animation event?
    {
        currentDurability--;
        _weaponMaterial.SetFloat("_saturationAlpha", (float)currentDurability/maxDurability);
        Debug.Log("Light attack used");
        
        //Not happy with how this works atm
        GameObject weapon = Instantiate(hammerLightAttackHitbox, transform.position, transform.rotation);
        HammerAttackLight attackData = hammerLightAttackHitbox.GetComponent<HammerAttackLight>();
        attackData.damage = lightAttackDamage;
        attackData.hpRestored = playerHpGainOnHit;
        if(currentDurability == 0){OnWeaponBreak();}
    }

    protected override void OnUseHeavyAttack() //Probably called from animation event?
    {
        if (PlayerController.Instance.TrySpendHealth(heavyAttackHpCost, out var healthUsed) == false)
        {
            return;
        }
        PlayerController.Instance.DamagePlayer(healthUsed);
        
        currentDurability--;
        _weaponMaterial.SetFloat("_saturationAlpha", (float)currentDurability/maxDurability);
        Debug.Log("Heavy attack used");
        
        GameObject weapon =  Instantiate(hammerHeavyAttackHitbox, transform.position, transform.rotation);
        HammerAttackHeavy weaponData = weapon.GetComponent<HammerAttackHeavy>();
        weaponData.damage = heavyAttackDamage;
        if(currentDurability == 0){OnWeaponBreak();}
        
    }
    
    protected override void OnWeaponBreak()
    {
        PlayerController.Instance.EquippedWeapon = null;
        Destroy(gameObject);
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
}
