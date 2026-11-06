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
    
     
    private static PlayerWeaponSystem _instance; 
    public static PlayerWeaponSystem Instance {get{return _instance;}}

    
    


    private void Awake()
    {
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
            
            weaponCreator = Instantiate(weaponCreatorPrefab);
            weaponCreator.SetActive(false);
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

    
     
    [Header("Input")]
    private PlayerInputA _playerInput;
    private PlayerInputA.PlayerDefaultActions _playerDefaultActions;
    
    

    
    private void OpenCreateWeaponUI(InputAction.CallbackContext obj)
    {
        if (EquippedWeapon != null) { return; }
        
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
