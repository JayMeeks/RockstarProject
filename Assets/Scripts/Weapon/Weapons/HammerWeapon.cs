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
    private bool _isGrounded;
 
    
    
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
        //Updates animator variables
        //I'm sure there's a better way of doing this, but this animator setup is temporary anyway
        _chargeTimer += _isChargingHammerLight || _isChargingHammerHeavy ? Time.deltaTime : 0; //Could be moved out of update loop? Not sure if there's a way to do "On mouse held", fine for now
        hammerAnimator.SetBool("IsChargingLight", _isChargingHammerLight);//^^^
        hammerAnimator.SetBool("IsChargingHeavy", _isChargingHammerHeavy);
        hammerAnimator.SetBool("IsGrounded", PlayerMovement.Instance.IsGrounded());

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

    
    //Called from animation event
    protected override void OnUseLightAttack() 
    {
        ReduceDurability();
        UpdateMatProps();

        var attackObj = Instantiate(LightAttack);
        attackObj.GetComponent<HammerLightAttackBehaviour>().Attack(PlayerMovement.Instance.transform.position,Camera.main.transform.rotation,Camera.main.transform.forward,out bool shouldHeal, weaponData.playerHpGainMaxRange);
        if (shouldHeal) { PlayerHealthSystem.Instance.HealPlayer(weaponData.playerHpGainOnHit);}
        
        if(currentDurability == 0){OnWeaponBreak();}
    }

    //Called from animation event
    protected override void OnUseHeavyAttack() 
    {
        PlayerStamina stamina = PlayerStamina.Instance;

        // Check whether the player has enough stamina for a heavy attack
        if (stamina == null || !stamina.CanSpend(stamina.heavyAttackCost))
            return;

        // Check the health cost before spending stamina
        if (!PlayerHealthSystem.Instance.TrySpendHealth(
            weaponData.heavyAttackHpCost, out var healthUsed))
        {
            return;
        }

        if (!stamina.TrySpend(stamina.heavyAttackCost))
            return;

        PlayerHealthSystem.Instance.DamagePlayer(healthUsed);
        
        
        
        //Gets player position 
        Vector3 spawnPos = new Vector3(
            PlayerMovement.Instance.transform.position.x,
            PlayerMovement.Instance.transform.position.y,
            PlayerMovement.Instance.transform.position.z
        );

        Quaternion startRot;
        
        
        
        //If the player has enhanced attack available, spawns 12 waves around the player
        //X and Z rotation is ignored
        if (useEnhancedHeavy)
        {
            for (int i = 0; i < 12; i++)
            {
                startRot = Quaternion.Euler(
                    0,
                    transform.rotation.eulerAngles.y + i * (360f / 12f),
                    0
                );

                var attackObj = Instantiate(HeavyAttack, spawnPos, startRot);
                attackObj.GetComponent<HammerHeavyAttackBehaviour>().StartAttack(useEnhancedHeavy);
            }
        }
        //Otherwise, just spawns the one just ahead of the player
        else
        {
            startRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            var attackObj = Instantiate(HeavyAttack, spawnPos, startRot);
            attackObj.GetComponent<HammerHeavyAttackBehaviour>().StartAttack(useEnhancedHeavy);
        }

        useEnhancedHeavy = false;

        ReduceDurability();
        UpdateMatProps();

        if (currentDurability == 0)
            OnWeaponBreak();
    }


    //Applies upwards force to player
    //Enables the heavy attack for a short time (until grounded again)
    protected override void OnUseWeaponSpecial()
    {
        PlayerStamina stamina = PlayerStamina.Instance;

        if (stamina == null)
            return;

        if (useEnhancedHeavy)
            return;

        // Spend 20 stamina to activate the hammer special
        if (!stamina.TrySpend(stamina.hammerSpecialCost))
            return;

        PlayerMovement.Instance.ApplyImpulse(Vector3.up, 1);
        useEnhancedHeavy = true;
        StartCoroutine(WaitUntilGrounded());
    }


    protected override void OnWeaponBreak()
    {
        PlayerWeaponSystem.Instance.EquipUnarmedWeapon();
        Destroy(gameObject);
    }


    //Called by animation event when the hammer is ready to be swung in the heavy animation
    //Adds a force to the player downwards 
    private void StartSlam()
    {
        PlayerMovement.Instance.ApplyImpulse(Vector3.down, 1);
    }


    //Called after weapon special is used
    //Disables the special attack after the player has returned to the ground
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
    
    
    /// <summary>
    /// Plays a simple shader-driven dissolve in animation
    /// </summary>

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
    
    
    /// <summary>
    /// Updates the shader material values for the discolouration
    /// </summary>
    private void UpdateMatProps()
    { 
        _weaponMaterial.SetFloat("_saturationAlpha", (float)currentDurability/weaponData.maxDurability);
    }

    /// <summary>
    /// Reduces durability of equipped weapon
    /// Has an integer overload if you want to reduce durability by more than one
    /// </summary>
    private void ReduceDurability()
    {
        currentDurability--;
    }
    
    private void ReduceDurability(int durability)
    {
        currentDurability -= durability;
    }
}
