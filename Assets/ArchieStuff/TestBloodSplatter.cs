using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;


public class TestBloodSplatter : MonoBehaviour
{

 [Header("Input")]
 private PlayerInputA _playerInput;
 private PlayerInputA.PlayerDefaultActions _playerDefaultActions;

 public Material bloodSplatterMaterial;

 public void Awake()
 {
  _playerInput = new PlayerInputA();
  _playerDefaultActions = _playerInput.PlayerDefault;
 }

 private void OnEnable()
 {
  _playerDefaultActions.Enable();

  _playerDefaultActions.WeaponSpecial.started += RandomiseBloodSplatter;

 }



 private void OnDisable()
 {
  _playerDefaultActions.Disable();
  _playerDefaultActions.WeaponSpecial.started -= RandomiseBloodSplatter;
 }

 private void RandomiseBloodSplatter(InputAction.CallbackContext obj)
 {
  bloodSplatterMaterial.SetFloat("_Seed", Random.Range(10f, 30f));
  bloodSplatterMaterial.SetFloat("_Size", Random.Range(0.3f, 0.5f));
  bloodSplatterMaterial.SetInt("_bigNumber",Random.Range(0,2147483647));

 }

 
}
