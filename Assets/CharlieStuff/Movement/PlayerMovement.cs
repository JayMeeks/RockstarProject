/*using System.Collections;
using UnityEngine;

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

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeInfo;
    private bool leavingSlope;

    public Transform orientation;

    private float moveX;
    private float moveZ;

    private Vector3 movementDirection;
    private Rigidbody body;

    public MovementState state;

    public enum MovementState
    {
        Walking,
        Sprinting,
        WallRiding,
        Air
    }

    public bool wallRiding;

    private float targetSpeed;
    private float previousTargetSpeed;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;

        canJump = true;
        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.2f,
            groundLayer
        );

        ReadInput();
        HandleState();
        LimitSpeed();

        if (isGrounded)
            body.drag = groundDrag;
        else
            body.drag = 0f;
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ReadInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && canJump && isGrounded)
        {
            canJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void HandleState()
    {
        if (wallRiding)
        {
            state = MovementState.WallRiding;
            targetSpeed = wallRideSpeed;
        }
        else if (isGrounded && Input.GetKey(sprintKey))
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

            if (previousTargetSpeed > walkSpeed)
                targetSpeed = runSpeed;
            else
                targetSpeed = walkSpeed;
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
            currentSpeed = Mathf.Lerp(
                startingSpeed,
                targetSpeed,
                elapsed / difference
            );

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

    private void ApplyMovement()
    {
        movementDirection =
            orientation.forward * moveZ +
            orientation.right * moveX;

        if (IsOnSlope() && !leavingSlope)
        {
            body.AddForce(
                GetSlopeDirection(movementDirection) * currentSpeed * 20f,
                ForceMode.Force
            );

            if (body.velocity.y > 0f)
                body.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (isGrounded)
        {
            body.AddForce(
                movementDirection.normalized * currentSpeed * 10f,
                ForceMode.Force
            );
        }
        else
        {
            body.AddForce(
                movementDirection.normalized *
                currentSpeed *
                10f *
                airControl,
                ForceMode.Force
            );
        }

        if (!wallRiding)
            body.useGravity = !IsOnSlope();
    }

    private void LimitSpeed()
    {
        if (IsOnSlope() && !leavingSlope)
        {
            if (body.velocity.magnitude > currentSpeed)
            {
                body.velocity =
                    body.velocity.normalized * currentSpeed;
            }
        }
        else
        {
            Vector3 horizontalVelocity =
                new Vector3(body.velocity.x, 0f, body.velocity.z);

            if (horizontalVelocity.magnitude > currentSpeed)
            {
                Vector3 limitedVelocity =
                    horizontalVelocity.normalized * currentSpeed;

                body.velocity = new Vector3(
                    limitedVelocity.x,
                    body.velocity.y,
                    limitedVelocity.z
                );
            }
        }

        if (maxVerticalSpeed != 0f &&
            body.velocity.y > maxVerticalSpeed)
        {
            body.velocity = new Vector3(
                body.velocity.x,
                maxVerticalSpeed,
                body.velocity.z
            );
        }
    }

    private void Jump()
    {
        leavingSlope = true;

        body.velocity = new Vector3(
            body.velocity.x,
            0f,
            body.velocity.z
        );

        body.AddForce(
            transform.up * jumpForce,
            ForceMode.Impulse
        );
    }

    private void ResetJump()
    {
        canJump = true;
        leavingSlope = false;
    }

    public bool IsOnSlope()
    {
        if (Physics.Raycast(
            transform.position,
            Vector3.down,
            out slopeInfo,
            playerHeight * 0.5f + 0.3f,
            groundLayer))
        {
            float slopeAngle =
                Vector3.Angle(Vector3.up, slopeInfo.normal);

            return slopeAngle > 0f &&
                   slopeAngle < maxSlopeAngle;
        }

        return false;
    }

    public Vector3 GetSlopeDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(
            direction,
            slopeInfo.normal
        ).normalized;
    }
} */