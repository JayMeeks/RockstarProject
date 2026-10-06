using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class WeaponCreator : MonoBehaviour
{
    public Slider healthInputSlider;
    public Slider currentHealthSlider;


    public Vector2 MousePos;

    private float sliderVal;

    private float snapVal = 0.25f;

    public float sliderValSnap;
    
    [Header("Input")]
    private PlayerInput _playerInput;
    private PlayerInput.PlayerDefaultActions _playerDefaultActions;





    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerDefaultActions = _playerInput.PlayerDefault;
        
        
    }

    private void OnEnable()
    {
        _playerDefaultActions.Enable();

        _playerDefaultActions.Mouse1.started += CreateWeapon;
    }

    private void OnDisable()
    {
        _playerDefaultActions.Disable();

        _playerDefaultActions.Mouse1.started -= CreateWeapon;
    }


    public void Update()
    {

        float currentHealthSliderVal = PlayerController.Instance.GetCurrentHealth() / 100f;
        currentHealthSlider.value = currentHealthSliderVal;
    
    
        //Get mouse delta
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 mouseDelta = mousePosition - center;
       
       
        //Get raw slider value
        sliderVal = 1 - (Mathf.Abs((Mathf.Atan2(mouseDelta.y,mouseDelta.x) * Mathf.Rad2Deg))/180); 
        
        
        //Snap slider value to interval
        sliderValSnap = Mathf.Round(sliderVal/snapVal)*snapVal;
        
        //Clamp to be <= player hp
        while (sliderValSnap > currentHealthSliderVal)
        {
            sliderValSnap -= 0.25f;
        }
        
     
        
        healthInputSlider.value = sliderValSnap;

    }

    private void CreateWeapon(InputAction.CallbackContext ctx)
    {
        float hpCost = sliderValSnap * 100f;
        Debug.Log("Hpcost: " + hpCost);
        Debug.Log("Weapon created using hp:" + hpCost);
 
        PlayerController.Instance.SetWeaponVariables((int)hpCost);
       
        
        PlayerController.Instance.currentWeapon.SetActive(true);
        //I think I miss my swizzling :(
        PlayerController.Instance.currentWeapon.transform.localScale = new Vector3(
            sliderVal + 1,
            sliderVal + 1 ,
            sliderVal + 1);
            PlayerController.Instance.SetCanLook(true);
            
            
        
        PlayerController.Instance.DissolveAnimator.PlayAnimation();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        gameObject.SetActive(false);

    }


}
