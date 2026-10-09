using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WeaponCreator2 : MonoBehaviour
{
    [SerializeField] private RawImage weapon1image,weapon2image,weapon3image;
    [SerializeField] private BaseWeapon weapon1, weapon2, weapon3;

    private float angle;
    
    [Header("Input")]
    private PlayerInput _playerInput;
    private PlayerInput.PlayerDefaultActions _playerDefaultActions;

    public void Awake()
    {
        _playerInput = new PlayerInput();
        _playerDefaultActions = _playerInput.PlayerDefault;
    }

    public void OnEnable()
    {
        _playerInput.Enable();
        _playerDefaultActions.Mouse1.started += CreateWeapon;
    }



    public void OnDisable()
    {
        _playerInput.Disable();
        _playerDefaultActions.Mouse1.started -= CreateWeapon;
    }


    public void Update()
    {
        //Get mouse delta
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 center = new Vector2((float)Screen.width / 2,(float)Screen.height / 2);
        Vector2 mouseDelta = mousePosition - center;
        
        mouseDelta.x *= (float)Screen.height/Screen.width; //Account for aspect ratio


        //Get angle between mouse and center of screen to determine screen region
        angle = ((Mathf.Atan2(mouseDelta.y, mouseDelta.x) * Mathf.Rad2Deg));
        
        
        //This code kind of ugly, will be improved at later date. 
        //Functions well enough for prototype
        if (angle is > 45f and < 135f)
        {
           
            weapon1image.rectTransform.localScale = new Vector3(1.25f, 1.25f, 1f);
            weapon2image.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            weapon3image.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (angle is > -90f and < 45f)
        {
      
            weapon1image.rectTransform.localScale = new Vector3(1, 1, 1f);
            weapon2image.rectTransform.localScale = new Vector3(1.25f, 1.25f, 1f);
            weapon3image.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {

            weapon1image.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            weapon2image.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            weapon3image.rectTransform.localScale = new Vector3(1.25f, 1.25f, 1f);
        }

    }
    
    private void CreateWeapon(InputAction.CallbackContext obj)
    {
        Debug.Log("Weapon creation initiated");

        int weaponCost = 25;


        if ( PlayerController.Instance.TrySpendHealth(weaponCost,out var healthUsed) == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerController.Instance.SetCanLook(true);
            gameObject.SetActive(false);
            return;
        }
        
       PlayerController.Instance.DamagePlayer(healthUsed);
        
        
        
        //Again, kind of ugly. Can be improved later if needed
        if (angle is > 45f and < 135f)
        { 
          PlayerController.Instance.EquippedWeapon = Instantiate(weapon1, PlayerController.Instance.GetWeaponHolder(), false);
            

        }
        else if (angle is > -90f and < 45f)
        {
            PlayerController.Instance.EquippedWeapon = Instantiate(weapon2, PlayerController.Instance.GetWeaponHolder(), false);
      
        }
        else
        {
            PlayerController.Instance.EquippedWeapon = Instantiate(weapon3, PlayerController.Instance.GetWeaponHolder(), false);
            
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerController.Instance.SetCanLook(true);
        gameObject.SetActive(false);
    }
    
    

}
