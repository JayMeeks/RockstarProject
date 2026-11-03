using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float currentSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float wallRideSpeed;
    public float maxVerticalSpeed;
    public float accelerationMultiplier;
    public float slopeAccelerationMultiplier;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airControl;
    private bool canJump;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    public bool isDashing { get; private set; }

    private float dashTimer;
    private float nextDashTime;
    private Vector3 dashDirection;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeInfo;
    private bool leavingSlope;

    [Header("References")]
    public Transform orientation;

    private float moveX;
    private float moveZ;
    private Vector3 movementDirection;
    private Rigidbody body;
    private PlayerControls controls;

    public MovementState state;

    public enum MovementState
    {
        Walking,
        Sprinting,
        WallRiding,
        Air,
        Dashing
    }

    public bool wallRiding;

    private float targetSpeed;
    private float previousTargetSpeed;

    private void Awake()
    {
        controls = new PlayerControls();
        body = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        isDashing = false;
    }

    private void Start()
    {
        body.freezeRotation = true;
        canJump = true;
        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down,
            playerHeight * 0.5f + 0.2f, groundLayer);

        ReadInput();
        ReadDashInput();
        HandleState();
        LimitSpeed();

        body.linearDamping = isGrounded ? groundDrag : 0f;
    }

    private void FixedUpdate()
    {
        if (isDashing)
            ApplyDash();
        else
            ApplyMovement();
    }

    private void ReadInput()
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        moveX = moveInput.x;
        moveZ = moveInput.y;

        if (controls.Player.Jump.IsPressed() && canJump && isGrounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ReadDashInput()
    {
        if (controls.Player.Dash.WasPressedThisFrame() &&
            !isDashing && Time.time >= nextDashTime)
        {
            StartDash();
        }
    }

    private void HandleState()
    {
        if (isDashing)
        {
            state = MovementState.Dashing;
            return;
        }
        else if (wallRiding)
        {
            state = MovementState.WallRiding;
            targetSpeed = wallRideSpeed;
        }
        else if (isGrounded && controls.Player.Sprint.IsPressed())
        {
            state = MovementState.Sprinting;
            targetSpeed = runSpeed;
        }
        else if (isGrounded)
        {
            state = MovementState.Walking;
            targetSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.Air;
            targetSpeed = previousTargetSpeed > walkSpeed ? runSpeed : walkSpeed;
        }

        if (Mathf.Abs(targetSpeed - previousTargetSpeed) > 0.1f)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeSpeedSmoothly());
        }

        previousTargetSpeed = targetSpeed;
    }

    private IEnumerator ChangeSpeedSmoothly()
    {
        float elapsed = 0f;
        float difference = Mathf.Abs(targetSpeed - currentSpeed);
        float startingSpeed = currentSpeed;

        if (difference <= 0.01f)
        {
            currentSpeed = targetSpeed;
            yield break;
        }

        while (elapsed < difference)
        {
            currentSpeed = Mathf.Lerp(startingSpeed, targetSpeed, elapsed / difference);

            float acceleration = accelerationMultiplier;

            if (IsOnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeInfo.normal);
                float slopeFactor = 1f + slopeAngle / 90f;

                acceleration *= slopeAccelerationMultiplier * slopeFactor;
            }

            elapsed += Time.deltaTime * acceleration;
            yield return null;
        }

        currentSpeed = targetSpeed;
    }

    private void StartDash()
    {
        Vector3 direction = orientation.forward * moveZ + orientation.right * moveX;

        if (direction.sqrMagnitude < 0.01f)
            direction = orientation.forward;

        direction.y = 0f;
        dashDirection = direction.normalized;

        isDashing = true;
        dashTimer = dashDuration;
        nextDashTime = Time.time + dashCooldown;
    }

    private void ApplyDash()
    {
        Vector3 velocity = body.linearVelocity;

        body.linearVelocity = new Vector3(
            dashDirection.x * dashSpeed,
            velocity.y,
            dashDirection.z * dashSpeed
        );

        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer <= 0f)
            StopDash();
    }

    private void StopDash()
    {
        isDashing = false;
    }

    private void ApplyMovement()
    {
        movementDirection = orientation.forward * moveZ + orientation.right * moveX;

        if (IsOnSlope() && !leavingSlope)
        {
            body.AddForce(GetSlopeDirection(movementDirection) *
                currentSpeed * 20f, ForceMode.Force);

            if (body.linearVelocity.y > 0f)
                body.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (isGrounded)
        {
            body.AddForce(movementDirection.normalized *
                currentSpeed * 10f, ForceMode.Force);
        }
        else
        {
            body.AddForce(movementDirection.normalized *
                currentSpeed * 10f * airControl, ForceMode.Force);
        }

        if (!wallRiding)
            body.useGravity = !IsOnSlope();
    }

    private void LimitSpeed()
    {
        if (isDashing)
            return;

        if (IsOnSlope() && !leavingSlope)
        {
            if (body.linearVelocity.magnitude > currentSpeed)
                body.linearVelocity = body.linearVelocity.normalized * currentSpeed;
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(
                body.linearVelocity.x, 0f, body.linearVelocity.z
            );

            if (horizontalVelocity.magnitude > currentSpeed)
            {
                Vector3 limitedVelocity = horizontalVelocity.normalized * currentSpeed;

                body.linearVelocity = new Vector3(
                    limitedVelocity.x,
                    body.linearVelocity.y,
                    limitedVelocity.z
                );
            }
        }

        if (maxVerticalSpeed != 0f && body.linearVelocity.y > maxVerticalSpeed)
        {
            body.linearVelocity = new Vector3(
                body.linearVelocity.x,
                maxVerticalSpeed,
                body.linearVelocity.z
            );
        }
    }

    private void Jump()
    {
        leavingSlope = true;

        body.linearVelocity = new Vector3(
            body.linearVelocity.x, 0f, body.linearVelocity.z
        );

        body.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
        leavingSlope = false;
    }

    public bool IsOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeInfo,
            playerHeight * 0.5f + 0.3f, groundLayer))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, slopeInfo.normal);
            return slopeAngle > 0f && slopeAngle < maxSlopeAngle;
        }

        return false;
    }

    public Vector3 GetSlopeDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeInfo.normal).normalized;
    }
}
