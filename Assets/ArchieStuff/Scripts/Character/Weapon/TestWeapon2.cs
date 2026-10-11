using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestWeapon2 : BaseWeapon
{
    [Header("Settings")] 
    [SerializeField] private float throwSpeedMult;
    [SerializeField] private float maxRange;
   
    [Header("Material")] 
    [SerializeField]private Material _weaponMaterial;
    private float _dissolveAlpha;
    
    
    [Header("Input")]
    private PlayerInput _playerInput;
    private PlayerInput.PlayerDefaultActions _playerDefaultActions;
    
    [Header("Attacks")]
    [SerializeField] private GameObject knifeLightAttackHitbox;
    [SerializeField] private BoxCollider knifeHeavyAttackHitbox;
    private bool _chainExtended = false;
    private bool enemyHit = false;
    
    [Header("Line renderer")]
    [SerializeField]private LineRenderer _lineRenderer;
    


    protected void Awake()
    {
        _playerInput = new PlayerInput();
        _playerDefaultActions = _playerInput.PlayerDefault;
        
        _weaponMaterial.SetFloat("_saturationAlpha", 1.0f);
        
        _lineRenderer.material = _weaponMaterial;
   
        

    
        
        StartCoroutine(DissolveIn());
    }

    protected void OnEnable()
    {
        _playerDefaultActions.Enable();

        _playerDefaultActions.Mouse1.started += StartLightAttack;
        _playerDefaultActions.Mouse2.started += StartHeavyAttack;

    }

    protected void OnDisable()
    {
        _playerDefaultActions.Disable();
        
        _playerDefaultActions.Mouse1.started -= StartLightAttack;
        _playerDefaultActions.Mouse2.started -= StartHeavyAttack;

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
    

    protected override void OnUseLightAttack()
    {
        if (_chainExtended) { return;}
        currentDurability--;
        _weaponMaterial.SetFloat("_saturationAlpha", (float)currentDurability/maxDurability);
        Debug.Log("Light attack used");
        
        GameObject weapon = Instantiate(knifeLightAttackHitbox, transform.position, transform.rotation);
        KnifeAttackLight attackData = weapon.GetComponent<KnifeAttackLight>();
        attackData.damage = lightAttackDamage;
        attackData.hpRestored = playerHpGainOnHit;
        
        
        if(currentDurability == 0){OnWeaponBreak();}
    }

    protected override void OnUseHeavyAttack()
    {
        if (_chainExtended) { return;}
        if (PlayerController.Instance.TrySpendHealth(heavyAttackHpCost, out var healthUsed) == false) { return; }
        PlayerController.Instance.DamagePlayer(healthUsed);
        currentDurability--;
        _weaponMaterial.SetFloat("_saturationAlpha", (float)currentDurability/maxDurability);

        Vector3 throwDir = PlayerController.Instance.GetPlayerForward();
        Vector3 startPos = transform.localPosition;
        Quaternion startLocalRot = transform.localRotation;
        Quaternion startWorldRot = transform.rotation;
        transform.rotation = Quaternion.LookRotation(throwDir, Vector3.up);
        transform.SetParent(null);
        StartCoroutine(ThrowDagger(startPos, throwDir,startLocalRot,startWorldRot));
     
       // transform.position = startPos;
        
        
        Debug.Log("Heavy attack used");
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

    private IEnumerator ThrowDagger(Vector3 startPos, Vector3 throwDir, Quaternion startlocalRot, Quaternion startWorldRot)
    {
        _chainExtended = true;
        _lineRenderer.enabled = true;
        knifeHeavyAttackHitbox.enabled = true;
        
        while (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= maxRange && enemyHit == false)
        {
            _lineRenderer.SetPosition(0, PlayerController.Instance.transform.position);
            _lineRenderer.SetPosition(1, transform.position);
            transform.position += throwDir * (Time.deltaTime * throwSpeedMult);
            yield return null;
        }
        _chainExtended = false;
        enemyHit = false;
        _lineRenderer.enabled = false;
        knifeHeavyAttackHitbox.enabled = false;
        transform.SetParent(PlayerController.Instance.GetWeaponHolder());
        transform.localPosition = startPos;
        transform.rotation = startWorldRot;
        transform.localRotation = startlocalRot;
       
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag($"Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            enemy.TakeDamage(heavyAttackDamage);
            enemyHit = true;
        }
    }
}
