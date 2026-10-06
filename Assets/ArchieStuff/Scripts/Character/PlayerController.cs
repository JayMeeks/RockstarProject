using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
 [Header("Player components")]
 [SerializeField] private Rigidbody playerRigidbody;

 
 [Header("Input")]
 private PlayerInput _playerInput;
 private PlayerInput.PlayerDefaultActions _playerDefaultActions;
 
 
 [Header("Camera")]
 [SerializeField] private Camera playerCamera;
 
 [Header("UI")]
 [SerializeField] private GameObject playerUI;
 [SerializeField] private Slider hpSlider;
 [SerializeField] private TextMeshProUGUI hpText;




 private float camXrot, camYrot;
 public float camSensX, camSensY;

 [Header("Movement")] 
 public float playerMoveSpeed;
 private Vector3 _moveDirection;
 private Vector2 _moveInput;
 
 [Header("Health")]
 private int maxHealth = 100;
 public int currentHealth;

 private bool CanMove, CanLook;

[Header("Weapon")]

 [SerializeField] Animator hammerAnimator;

 private float _chargeTimer;
 private bool _isChargingHammer;

 public int hpInvestedIntoWeapon;

 private GameObject weaponCreator;
 public GameObject weaponCreatorPrefab;

 public GameObject currentWeapon;
 public DissolveAnimator DissolveAnimator;

 public GameObject hammerHitbox;

 private static PlayerController _instance; 
 public static PlayerController Instance {get{return _instance;}}




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

   _playerInput = new PlayerInput();
   _playerDefaultActions = _playerInput.PlayerDefault;

   CanMove = CanLook = true;

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
  _playerDefaultActions.Mouse1.started += StartHammerCharge;
  _playerDefaultActions.Mouse1.canceled += EndHammerCharge;
 }



 private void OnDisable()
 {
  _playerDefaultActions.Disable();
  _playerDefaultActions.CreateWeapon.started -= OpenCreateWeaponUI;
  _playerDefaultActions.Mouse1.started -= StartHammerCharge;
  _playerDefaultActions.Mouse1.canceled -= EndHammerCharge;
  

 }


 private void Update()
 {
  UpdateUI(); //ONLY HERE FOR DEMO -----> move to event based in future
  
  _chargeTimer += _isChargingHammer ? Time.deltaTime : 0;

  if (currentWeapon.activeInHierarchy)
  {
   hammerAnimator.SetBool("IsCharging", _isChargingHammer);
  }
  
  
  
 }


 private void FixedUpdate()
 {
  HandleMovement();
 }

 private void LateUpdate()
 {
  HandleLook();
 }
 
 private void StartHammerCharge(InputAction.CallbackContext ctx)
 {
  if(!currentWeapon.activeInHierarchy) {return;}
  _isChargingHammer = true;
 }

 public void CreateWeaponHitbox()
 {
  Debug.Log("Weapon hitbox created");
  Instantiate(hammerHitbox, transform.position, playerCamera.transform.rotation);
 }

 private void EndHammerCharge(InputAction.CallbackContext ctx)
 {
  _isChargingHammer = false;
  _chargeTimer = 0;
 }

 private void OpenCreateWeaponUI(InputAction.CallbackContext obj)
 {
  if (currentWeapon.activeInHierarchy)
  {
   DestroyWeapon();
   currentWeapon.SetActive(false);
   return;
  }
 
 
  weaponCreator.SetActive(true);
  CanLook = false;
  
  Cursor.lockState = CursorLockMode.None;
  Cursor.visible = true;
 }


 private void UpdateUI()
 {
  hpText.text = currentHealth.ToString();
  hpSlider.value = (float)currentHealth / (float)maxHealth;
 }
 

 private void HandleMovement()
 {
  _moveInput = _playerDefaultActions.Move.ReadValue<Vector2>();
  transform.position += _moveDirection * (playerMoveSpeed * Time.deltaTime);
 }

 private void HandleLook()
 {
  if (!CanLook)
  {
   _moveDirection = Vector3.zero;
   return;
  }
  
  
   float mouseX = _playerDefaultActions.Look.ReadValue<Vector2>().x * Time.deltaTime * camSensX;
   float mouseY = _playerDefaultActions.Look.ReadValue<Vector2>().y * Time.deltaTime * camSensY;
   
   camYrot += mouseX;
   camXrot -= mouseY;
   
   camXrot = Mathf.Clamp(camXrot, -90f, 90f);

  
   playerCamera.transform.rotation = Quaternion.Euler(camXrot, camYrot, 0f);
   
   
   
   Vector3 camForward = playerCamera.transform.forward;
   camForward.y = 0; 
   camForward.Normalize();
   
   Vector3 camRight = playerCamera.transform.right;
   camRight.y = 0;
   camRight.Normalize();
   
   _moveDirection = camForward * _moveInput.y + camRight * _moveInput.x;
   _moveDirection.Normalize();
  
 }


//Getters + setters
 public void SetCanLook(bool canLook)
 {
  CanLook = canLook;
 }

 public void SetCurrentHealth(int currentHealth)
 {
  this.currentHealth = currentHealth;
 }

 public int GetCurrentHealth()
 {
  return currentHealth;
 }

 public void SetMaxHealth(int maxHealth)
 {
  this.maxHealth = maxHealth;
 }

 public int GetMaxHealth()
 {
  return maxHealth;
 }

 public int GetHPInvestedIntoWeapon()
 {
  return hpInvestedIntoWeapon;
 }

 public void SetWeaponVariables(int hpInvest)
 {
  Debug.Log("Hp received: " + hpInvest);
  if (hpInvest == currentHealth)
  {
   hpInvestedIntoWeapon = hpInvest;
   SetCurrentHealth(currentHealth - (hpInvest-1));
   
  }
  else
  {
   hpInvestedIntoWeapon = hpInvest;
   SetCurrentHealth(currentHealth - hpInvest);
   

  }
 }

 public void DestroyWeapon()
 {
  if (currentHealth + hpInvestedIntoWeapon > maxHealth)
  {
   currentHealth = maxHealth;
  }
  else
  {
   currentHealth += hpInvestedIntoWeapon;
  }
  
  
  hpInvestedIntoWeapon = 0;
 }
 
}
