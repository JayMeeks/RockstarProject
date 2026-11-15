using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
 [Header("Player components")]
 [SerializeField] private Rigidbody playerRigidbody;


 
 [Header("Camera")]
 [SerializeField] private Transform playerCamera;
 
 [Header("UI")]
 [SerializeField] private GameObject playerUI;
 [SerializeField] private Slider hpSlider;
 [SerializeField] private TextMeshProUGUI hpText;

 [SerializeField] private Material vignette;
 
 private float camXrot, camYrot;
 public float camSensX, camSensY;

 [Header("Movement")] 
 public float playerMoveSpeed;
 private Vector3 _moveDirection;
 private Vector2 _moveInput;
 


 private bool CanMove, CanLook;












 private void Update()
 {
  UpdateUI(); //ONLY HERE FOR DEMO -----> move to event based in future
  
 }


 private void FixedUpdate()
 {
////  HandleMovement();
 }

 private void LateUpdate()
 {
//  HandleLook();
 }
 





 private void UpdateUI()
 {
 
 /*
  hpText.text = currentHealth.ToString();
  hpSlider.value = (float)currentHealth / (float)maxHealth;*/
  
  
 }
 

/*
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
  
 }*/
 




//Getters + setters
 public void SetCanLook(bool canLook)
 {
  CanLook = canLook;
 }

 public Vector3 GetPlayerForward()
 {
  return playerCamera.transform.forward;
 }

 public bool IsGrounded()
 {
  Vector3 origin = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z);
  return Physics.Raycast(origin, Vector3.down, .25f);
 }







 private IEnumerator AnimateVignette(Color vignetteColor)
 {
  vignette.SetColor("_VignetteColour", vignetteColor);
  float time  = 0.0f;
  float startRadius = 1.5f;
  float endRadius = 0.75f;
  while (time < 0.2f)
  {
   time += Time.deltaTime;
   vignette.SetFloat("_VignetteRadius",Mathf.Lerp(startRadius, endRadius, time));
   yield return null;
  }
  vignette.SetFloat("_VignetteRadius",endRadius);
  time = 0.0f;
  
  
  while (time < 0.2f)
  {
   time += Time.deltaTime;
   vignette.SetFloat("_VignetteRadius",Mathf.Lerp(endRadius, startRadius, time));
   yield return null;
  }
  vignette.SetFloat("_VignetteRadius",2.0f);

 }


 
}
