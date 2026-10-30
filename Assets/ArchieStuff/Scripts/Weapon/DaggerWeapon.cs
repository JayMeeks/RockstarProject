using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class DaggerWeapon : BaseWeapon
{
    [Header("Data")]
    public DaggerWeaponData weaponData;

    public GameObject LightAttack,HeavyAttack;
    
    [Header("Material")] 
    [SerializeField]private Material _weaponMaterial;
    private float _dissolveAlpha;

    
    
    [Header("Input")]
    private PlayerInputA _playerInput;
    private PlayerInputA.PlayerDefaultActions _playerDefaultActions;
    
    [Header("Attacks")]
    [SerializeField] private GameObject knifeLightAttackHitbox;
    [SerializeField] private BoxCollider knifeHeavyAttackHitbox;
  
    


    protected void Awake()
    {
        _playerInput = new PlayerInputA();
        _playerDefaultActions = _playerInput.PlayerDefault;
        
        _weaponMaterial.SetFloat("_saturationAlpha", 1.0f);
        
        
        
        currentDurability = weaponData.maxDurability;
        weaponEnabled = true;

        StartCoroutine(DissolveIn());
    }

    protected void OnEnable()
    {
        _playerDefaultActions.Enable();

        _playerDefaultActions.Mouse1.started += StartLightAttack;
        _playerDefaultActions.Mouse2.started += StartHeavyAttack;
        _playerDefaultActions.WeaponSpecial.started  += StartSpecial;

    }

    protected void OnDisable()
    {
        _playerDefaultActions.Disable();
        
        _playerDefaultActions.Mouse1.started -= StartLightAttack;
        _playerDefaultActions.Mouse2.started -= StartHeavyAttack;
        _playerDefaultActions.WeaponSpecial.started  -= StartSpecial;

    }




    //Temp setup because I don't have animations
    private void StartLightAttack(InputAction.CallbackContext ctx)
    {
        OnUseLightAttack();
    }
    
    private void StartHeavyAttack(InputAction.CallbackContext ctx)
    {
        OnUseHeavyAttack();
    }

    private void StartSpecial(InputAction.CallbackContext ctx)
    {
        OnUseWeaponSpecial();
    }
    

    protected override void OnUseLightAttack()
    {
        //Prevents using light attack if the dagger has been thrown already
        if (!weaponEnabled) { return;}
        ReduceDurability();
        UpdateMatProps();

        
        var attackObj = Instantiate(LightAttack); 
        attackObj.GetComponent<DaggerLightAttackBehaviour>().Attack(PlayerController.Instance.transform.position,Camera.main.transform.rotation,Camera.main.transform.forward,out bool shouldHeal, weaponData.playerHpGainMaxRange);
        if (shouldHeal) { PlayerController.Instance.HealPlayer(weaponData.playerHpGainOnHit);}
        
        if(currentDurability == 0){OnWeaponBreak();}
    }

    protected override void OnUseHeavyAttack()
    {
        if (!weaponEnabled) { return;}
        //Move behaviour to player?
        if (PlayerController.Instance.TrySpendHealth(weaponData.heavyAttackHpCost, out var healthUsed) == false) { return; }
        PlayerController.Instance.DamagePlayer(healthUsed);
        
        ReduceDurability();
        UpdateMatProps();

        weaponEnabled = false;
        PlayerController.Instance.SetWeaponVisible(false);
        
        var attackObj = Instantiate(HeavyAttack, PlayerController.Instance.transform.position, transform.rotation); 
        attackObj.GetComponent<DaggerHeavyAttackBehaviour>().Attack(PlayerController.Instance.GetPlayerForward());

        
        if(currentDurability == 0){OnWeaponBreak();}
    }
    
    protected override void OnUseWeaponSpecial()
    {
        /*
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, weaponData.maxSpecialRange))
        {
            Debug.Log("Raycast hit");
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, PlayerController.Instance.transform.position);
            _lineRenderer.SetPosition(1, hit.point);
            
            Vector3 launchDir = (hit.point - transform.position).normalized;
            PlayerController.Instance.GetComponent<Rigidbody>().AddForce(launchDir * weaponData.specialForceMult, ForceMode.Impulse);
        }*/
        
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
    
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag($"Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            enemy.TakeDamage(weaponData.heavyAttackDamage);
            enemyHit = true;
        }
    }*/
}
