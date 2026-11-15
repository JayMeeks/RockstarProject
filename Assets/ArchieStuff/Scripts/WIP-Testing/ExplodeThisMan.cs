using UnityEngine;
using UnityEngine.InputSystem;

public class ExplodeThisMan : MonoBehaviour
{
    [Header("Input")]
    private PlayerInputA _playerInput;
    private PlayerInputA.PlayerDefaultActions _playerDefaultActions;

    public GameObject man;

    public int force;

    public Vector3 direction;





    private void Awake()
    {
        _playerInput = new PlayerInputA();
        _playerDefaultActions = _playerInput.PlayerDefault;
        
        Rigidbody[] bones = man.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody bone in bones)
        { bone.isKinematic = true; 
        }
        
      
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerDefaultActions.Enable();

        _playerDefaultActions.Mouse1.started += AddForce;
    }



    private void OnDisable()
    {
        _playerInput.Disable();
        _playerDefaultActions.Disable();
        _playerDefaultActions.Mouse1.started -= AddForce;
    }

    private void AddForce(InputAction.CallbackContext obj)
    {

      
     
            Rigidbody[] bones = man.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody bone in bones)
            {
                bone.isKinematic = false;
                bone.AddForce(direction * force, ForceMode.Impulse);
              // bone.AddExplosionForce(force, hit.point, 15.0f);
            }
        
        
        
          
    }

}
