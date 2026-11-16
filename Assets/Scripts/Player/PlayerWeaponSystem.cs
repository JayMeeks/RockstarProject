using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponSystem : MonoBehaviour
{
    [Header("Weapon")]
    private GameObject weaponCreator;
    public GameObject weaponCreatorPrefab;
    public BaseWeapon EquippedWeapon;
    public Transform weaponHolder;
    
    [Header("Input")]
    private PlayerInputA _playerInput;
    private PlayerInputA.PlayerDefaultActions _playerDefaultActions;



    [SerializeField] private BaseWeapon unarmedWeapon;

    private bool hasWeapon;
     
    private static PlayerWeaponSystem _instance; 
    public static PlayerWeaponSystem Instance {get{return _instance;}}

    
    


    private void Awake()
    {   
        //Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            _playerInput = new PlayerInputA();
            _playerDefaultActions = _playerInput.PlayerDefault;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            //Create weapon creator menu. This should maybe be somewhere else?
            weaponCreator = Instantiate(weaponCreatorPrefab);
            weaponCreator.SetActive(false);
            
            //Set default weapon to unarmed
            EquipUnarmedWeapon();
        }
    }

    private void OnEnable()
    {
        _playerDefaultActions.Enable();

        _playerDefaultActions.CreateWeapon.started += OpenCreateWeaponUI;

    }



    private void OnDisable()
    {
        _playerDefaultActions.Disable();
        _playerDefaultActions.CreateWeapon.started -= OpenCreateWeaponUI;
    }

    
     

    

    //Opens menu to create weapon
    private void OpenCreateWeaponUI(InputAction.CallbackContext obj)
    {
        //Prevents opening menu if player already has a weapon, or not enough health to create one
        if (hasWeapon) { return; }
        if (PlayerHealthSystem.Instance.currentHealth < 25.0f) { return;}
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        weaponCreator.SetActive(true);
    }
    
    public BaseWeapon GetEquippedWeapon()
    {
        return EquippedWeapon;
    }

    public void EquipWeapon(BaseWeapon weapon)
    {
        this.EquippedWeapon = weapon;
    }

    public Transform GetWeaponHolder()
    {
        return weaponHolder;
    }

    //Sets weapon to unarmed
    public void EquipUnarmedWeapon()
    {
        hasWeapon = false;
        EquippedWeapon = Instantiate(unarmedWeapon, GetWeaponHolder(), false);
    }

    public void SetHasWeapon(bool hasWeapon)
    {
        this.hasWeapon = hasWeapon;
    }
    


    public void SetWeaponEnabled(bool enabled)
    {
        EquippedWeapon.SetWeaponEnabled(enabled);
    }

    public void SetWeaponVisible(bool visible)
    {
        //Could probably cache
        EquippedWeapon.gameObject.GetComponent<MeshRenderer>().enabled = visible;
    }
    
    
}
