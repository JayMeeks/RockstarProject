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
 
    
    
    protected void Awake()
    {
        _playerInput = new PlayerInputA();
        _playerDefaultActions = _playerInput.PlayerDefault;
        
        _weaponMaterial.SetFloat("_saturationAlpha", 1.0f);
        currentDurability = weaponData.maxDurability;
        
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
        if (PlayerController.Instance.TrySpendHealth(weaponData.heavyAttackHpCost, out var healthUsed) == false)
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
        ReduceDurability();
        UpdateMatProps();

        var attackObj = Instantiate(LightAttack);
        attackObj.GetComponent<HammerLightAttackBehaviour>().Attack(PlayerController.Instance.transform.position,Camera.main.transform.rotation,Camera.main.transform.forward,out bool shouldHeal, weaponData.playerHpGainMaxRange);
        if (shouldHeal) { PlayerController.Instance.HealPlayer(weaponData.playerHpGainOnHit);}
        
        if(currentDurability == 0){OnWeaponBreak();}
    }

    protected override void OnUseHeavyAttack() //Probably called from animation event?
    {
        if (PlayerController.Instance.TrySpendHealth(weaponData.heavyAttackHpCost, out var healthUsed) == false)
        {
            return;
        }
        PlayerController.Instance.DamagePlayer(healthUsed);

        Vector3 spawnPos = new Vector3(PlayerController.Instance.transform.position.x,
            PlayerController.Instance.transform.position.y - 1f, PlayerController.Instance.transform.position.z);

       Quaternion startRot = Quaternion.LookRotation(PlayerController.Instance.GetPlayerForward(), Vector3.up);


        for (int i = 0; i < 12; i++)
        {
          
            startRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y + i * (360f/12f), 0);
           
            var attackObj = Instantiate(HeavyAttack,spawnPos,startRot);
            attackObj.GetComponent<HammerHeavyAttackBehaviour>().Attack();

        }
       
        
        ReduceDurability();
        UpdateMatProps();
       
        
  
        if(currentDurability == 0){OnWeaponBreak();}
        
    }

    protected override void OnUseWeaponSpecial()
    {
        throw new NotImplementedException();
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
