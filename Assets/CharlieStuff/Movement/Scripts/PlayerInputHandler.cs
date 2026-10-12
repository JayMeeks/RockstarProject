using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerControls Controls { get; private set; }

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool SprintHeld { get; private set; }
    public bool JumpPressed { get; private set; }

    private void Awake()
    {
        Controls = new PlayerControls();
    }

    private void OnEnable()
    {
        Controls.Enable();

        Controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        Controls.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        Controls.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        Controls.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        Controls.Player.Sprint.performed += _ => SprintHeld = true;
        Controls.Player.Sprint.canceled += _ => SprintHeld = false;

        Controls.Player.Jump.performed += _ => JumpPressed = true;
    }

    private void OnDisable()
    {
        Controls.Disable();
    }

    public bool ConsumeJump()
    {
        if (!JumpPressed)
            return false;

        JumpPressed = false;
        return true;
    }
}