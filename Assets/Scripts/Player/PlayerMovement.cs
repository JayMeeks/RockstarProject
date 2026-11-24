using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStamina))]
public class PlayerMovement : MonoBehaviour
{
    private static PlayerMovement _instance;
    public static PlayerMovement Instance { get { return _instance; } }

    [Header("Movement")]
    private float currentSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float wallRideSpeed;
    public float maxVerticalSpeed;
    public float accelerationMultiplier;
    public float slopeAccelerationMultiplier;
    public float groundDrag;

    [Header("Sprinting")]
    [SerializeField] private bool sprintToggled;

    [Header("Ground Braking")]
    public float groundBraking = 45f;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airControl;
    private bool canJump;
    private bool jumpingFromGround;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 5f;

    public bool isDashing { get; private set; }

    private float dashTimer;
    private float nextDashTime;
    private float dashVelocitySpeed;
    private Vector3 dashDirection;

    private float airborneSpeedLimit;

    [Header("Sliding")]
    [Range(0f, 1f)] public float slideMomentumRetention = 0.95f;
    [Min(0f)] public float runSlideBoost = 1f;
    public float minSlideStartSpeed = 4f;
    public float slideStopSpeed = 2f;
    public float slideDeceleration = 1.5f;
    public float slideSteering = 100f;

    public bool isSliding { get; private set; }

    private Vector3 slideVelocity;

    [Header("Slide Camera")]
    public Transform cameraHolder;
    public float slideCameraDrop = 0.7f;
    public float slideCameraSpeed = 10f;

    private Vector3 originalCameraPosition;

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
    private PlayerStamina stamina;
    private bool canSprint;

    public MovementState state;

    public enum MovementState
    {
        Walking,
        Sprinting,
        WallRiding,
        Air,
        Dashing,
        Sliding
    }

    public bool wallRiding;

    private float targetSpeed;
    private float previousTargetSpeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        controls = new PlayerControls();
        body = GetComponent<Rigidbody>();
        stamina = GetComponent<PlayerStamina>();
    }

    private void OnEnable()
    {
        if (controls != null)
            controls.Player.Enable();
    }

    private void OnDisable()
    {
        if (controls != null)
            controls.Player.Disable();

        isDashing = false;
        isSliding = false;
        sprintToggled = false;

        if (stamina != null)
            stamina.ResetSprintDrain();

        if (cameraHolder != null)
            cameraHolder.localPosition = originalCameraPosition;
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

        if (controls != null)
            controls.Dispose();
    }

    private void Start()
    {
        body.freezeRotation = true;
        canJump = true;
        currentSpeed = walkSpeed;

        if (cameraHolder != null)
            originalCameraPosition = cameraHolder.localPosition;
    }

    private void Update()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.2f,
            groundLayer
        );

        if (wasGrounded && !isGrounded)
        {
            airborneSpeedLimit = Mathf.Max(
                airborneSpeedLimit,
                GetHorizontalVelocity().magnitude
            );
        }
        else if (!wasGrounded && isGrounded)
        {
            if (isSliding)
            {
                Vector3 landingVelocity = GetHorizontalVelocity();

                if (slideVelocity.sqrMagnitude > landingVelocity.sqrMagnitude)
                    landingVelocity = slideVelocity;

                slideVelocity = GetGroundSlideVelocity(landingVelocity);
            }

            airborneSpeedLimit = 0f;
            jumpingFromGround = false;
        }

        ReadInput();
        ReadDashInput();
        HandleSlideInput();
        HandleSprintInput();


        bool moving = Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveZ) > 0.01f;

        // Automatically untoggle sprint when movement stops
        if (!moving && isGrounded && !isSliding)
        {
            sprintToggled = false;
        }



        bool wantsSprint = sprintToggled &&
            moving &&
            isGrounded &&
            !wallRiding &&
            !isDashing &&
            !isSliding;

        if (wantsSprint)
        {
            canSprint = stamina.TryDrainSprint(Time.deltaTime);

 
            if (!canSprint || !stamina.CanSpend(stamina.sprintCost))
                sprintToggled = false;
        }
        else
        {
            stamina.ResetSprintDrain();
            canSprint = false;
        }

        HandleState();
        LimitSpeed();

        body.linearDamping =
            isGrounded &&
            !isDashing &&
            !isSliding &&
            !jumpingFromGround
                ? groundDrag
                : 0f;

        UpdateSlideCamera();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            ApplyDash();
        }
        else if (isSliding)
        {
            ApplySlide();
        }
        else
        {
            ApplyMovement();
        }
    }

    private void ReadInput()
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        moveX = moveInput.x;
        moveZ = moveInput.y;

        if (controls.Player.Jump.IsPressed() &&
            canJump &&
            isGrounded &&
            stamina.TrySpend(stamina.jumpCost))
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }


    private void HandleSprintInput()
    {
        if (controls.Player.Sprint.WasPressedThisFrame() && !wallRiding)
        {
            sprintToggled = !sprintToggled;
        }
    }

    private void ReadDashInput()
    {
        if (controls.Player.Dash.WasPressedThisFrame() &&
            !isDashing &&
            Time.time >= nextDashTime)
        {
            StartDash();
        }
    }

    private void HandleSlideInput()
    {
        if (isSliding)
        {
            if (wallRiding ||
                isDashing ||
                !controls.Player.WallClimbDown.IsPressed() ||
                (isGrounded && slideVelocity.magnitude <= slideStopSpeed))
            {
                StopSlide();
            }

            return;
        }

        if (controls.Player.WallClimbDown.WasPressedThisFrame() &&
            !wallRiding &&
            !isDashing &&
            (!isGrounded || !leavingSlope))
        {
            StartSlide();
        }
    }

    private void StartSlide()
    {
        if (isSliding)
            return;

        Vector3 horizontalVelocity = GetHorizontalVelocity();

        if (isGrounded && horizontalVelocity.magnitude < minSlideStartSpeed)
            return;

        slideVelocity = isGrounded
            ? GetGroundSlideVelocity(horizontalVelocity)
            : horizontalVelocity;

        isSliding = true;
        stamina.ResetSprintDrain();
    }

    private Vector3 GetGroundSlideVelocity(Vector3 velocity)
    {
        float speed = velocity.magnitude;
        float maxRunSlideSpeed = runSpeed + runSlideBoost;

        if (runSpeed > 0f &&
            runSlideBoost > 0f &&
            speed >= runSpeed * 0.95f &&
            speed < maxRunSlideSpeed)
        {
            float boostedSpeed = Mathf.Min(
                speed + runSlideBoost,
                maxRunSlideSpeed
            );

            return velocity.normalized * boostedSpeed;
        }

        return velocity * slideMomentumRetention;
    }

    private void ApplySlide()
    {
        if (!isGrounded)
        {
            Vector3 horizontalVelocity = GetHorizontalVelocity();

            if (horizontalVelocity.sqrMagnitude > slideVelocity.sqrMagnitude)
            {
                slideVelocity = horizontalVelocity;
            }
            else if (horizontalVelocity.sqrMagnitude > 0.01f &&
                     horizontalVelocity.magnitude >= slideVelocity.magnitude * 0.8f)
            {
                slideVelocity =
                    horizontalVelocity.normalized * slideVelocity.magnitude;
            }

            body.useGravity = true;
            return;
        }

        Vector3 desiredDirection =
            orientation.forward * moveZ +
            orientation.right * moveX;

        desiredDirection.y = 0f;

        if (desiredDirection.sqrMagnitude > 0.01f &&
            slideVelocity.sqrMagnitude > 0.001f)
        {
            Vector3 newDirection = Vector3.RotateTowards(
                slideVelocity.normalized,
                desiredDirection.normalized,
                slideSteering * Mathf.Deg2Rad * Time.fixedDeltaTime,
                0f
            );

            slideVelocity = newDirection * slideVelocity.magnitude;
        }

        slideVelocity = Vector3.MoveTowards(
            slideVelocity,
            Vector3.zero,
            slideDeceleration * Time.fixedDeltaTime
        );

        body.linearVelocity = new Vector3(
            slideVelocity.x,
            body.linearVelocity.y,
            slideVelocity.z
        );

        body.useGravity = true;
    }

    private void StopSlide()
    {
        if (!isSliding)
            return;

        isSliding = false;

        if (!isGrounded)
        {
            airborneSpeedLimit = Mathf.Max(
                airborneSpeedLimit,
                GetHorizontalVelocity().magnitude
            );
        }
    }

    private void UpdateSlideCamera()
    {
        if (cameraHolder == null)
            return;

        Vector3 targetPosition = originalCameraPosition;

        if (isSliding)
            targetPosition.y -= slideCameraDrop;

        cameraHolder.localPosition = Vector3.Lerp(
            cameraHolder.localPosition,
            targetPosition,
            Mathf.Clamp01(Time.deltaTime * slideCameraSpeed)
        );
    }

    private Vector3 GetHorizontalVelocity()
    {
        return new Vector3(
            body.linearVelocity.x,
            0f,
            body.linearVelocity.z
        );
    }

    private void HandleState()
    {
        if (isDashing)
        {
            state = MovementState.Dashing;
            return;
        }
        else if (isSliding)
        {
            state = MovementState.Sliding;
            return;
        }
        else if (wallRiding)
        {
            state = MovementState.WallRiding;
            targetSpeed = wallRideSpeed;
        }
        else if (isGrounded && canSprint)
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
            targetSpeed = previousTargetSpeed > walkSpeed
                ? runSpeed
                : walkSpeed;
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
                float slopeAngle = Vector3.Angle(
                    Vector3.up,
                    slopeInfo.normal
                );

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
        if (isSliding)
            StopSlide();

        Vector3 direction =
            orientation.forward * moveZ +
            orientation.right * moveX;

        if (direction.sqrMagnitude < 0.01f)
            direction = orientation.forward;

        direction.y = 0f;
        dashDirection = direction.normalized;

        dashVelocitySpeed = Mathf.Max(
            dashSpeed,
            GetHorizontalVelocity().magnitude
        );

        isDashing = true;
        dashTimer = dashDuration;
        nextDashTime = Time.time + dashCooldown;
    }

    private void ApplyDash()
    {
        dashVelocitySpeed = Mathf.Max(
            dashVelocitySpeed,
            GetHorizontalVelocity().magnitude
        );

        Vector3 velocity = body.linearVelocity;

        body.linearVelocity = new Vector3(
            dashDirection.x * dashVelocitySpeed,
            velocity.y,
            dashDirection.z * dashVelocitySpeed
        );

        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer <= 0f)
            StopDash();
    }

    private void StopDash()
    {
        isDashing = false;

        if (!isGrounded)
        {
            airborneSpeedLimit = Mathf.Max(
                airborneSpeedLimit,
                GetHorizontalVelocity().magnitude
            );
        }

        if (!wallRiding && controls.Player.WallClimbDown.IsPressed())
        {
            StartSlide();
        }
    }

    private void ApplyMovement()
    {
        movementDirection =
            orientation.forward * moveZ +
            orientation.right * moveX;

        if (isGrounded && IsOnSlope() && !leavingSlope)
        {
            body.AddForce(
                GetSlopeDirection(movementDirection) *
                currentSpeed * 20f,
                ForceMode.Force
            );

            if (body.linearVelocity.y > 0f)
            {
                body.AddForce(
                    Vector3.down * 80f,
                    ForceMode.Force
                );
            }
        }
        else if (isGrounded)
        {
            body.AddForce(
                movementDirection.normalized *
                currentSpeed * 10f,
                ForceMode.Force
            );
        }
        else
        {
            body.AddForce(
                movementDirection.normalized *
                currentSpeed * 10f * airControl,
                ForceMode.Force
            );
        }

        if (!wallRiding)
        {
            body.useGravity = !(
                isGrounded &&
                IsOnSlope() &&
                !leavingSlope
            );
        }

        if (isGrounded &&
            !wallRiding &&
            !jumpingFromGround &&
            Mathf.Abs(moveX) < 0.01f &&
            Mathf.Abs(moveZ) < 0.01f)
        {
            Vector3 horizontalVelocity = GetHorizontalVelocity();

            horizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity,
                Vector3.zero,
                groundBraking * Time.fixedDeltaTime
            );

            body.linearVelocity = new Vector3(
                horizontalVelocity.x,
                body.linearVelocity.y,
                horizontalVelocity.z
            );
        }
    }

    private void LimitSpeed()
    {
        if (isDashing || isSliding)
            return;

        float allowedSpeed;

        if (isGrounded && !jumpingFromGround)
        {
            if (state == MovementState.Sprinting)
                allowedSpeed = runSpeed;
            else if (state == MovementState.WallRiding)
                allowedSpeed = wallRideSpeed;
            else
                allowedSpeed = walkSpeed;
        }
        else
        {
            allowedSpeed = Mathf.Max(
                currentSpeed,
                airborneSpeedLimit
            );
        }

        if (isGrounded && IsOnSlope() && !leavingSlope)
        {
            if (body.linearVelocity.magnitude > allowedSpeed)
            {
                body.linearVelocity =
                    body.linearVelocity.normalized * allowedSpeed;
            }
        }
        else
        {
            Vector3 horizontalVelocity = GetHorizontalVelocity();

            if (horizontalVelocity.magnitude > allowedSpeed)
            {
                Vector3 limitedVelocity =
                    horizontalVelocity.normalized * allowedSpeed;

                body.linearVelocity = new Vector3(
                    limitedVelocity.x,
                    body.linearVelocity.y,
                    limitedVelocity.z
                );
            }
        }

        if (maxVerticalSpeed != 0f &&
            body.linearVelocity.y > maxVerticalSpeed)
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
        if (isSliding)
            StopSlide();

        airborneSpeedLimit = Mathf.Max(
            airborneSpeedLimit,
            GetHorizontalVelocity().magnitude
        );

        jumpingFromGround = true;
        leavingSlope = true;

        body.linearVelocity = new Vector3(
            body.linearVelocity.x,
            0f,
            body.linearVelocity.z
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

    public void ApplyImpulse(Vector3 direction, float forceMultiplier)
    {
        body.AddForce(
            direction * forceMultiplier,
            ForceMode.Impulse
        );
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public Vector3 GetPlayerForward()
    {
        return transform.forward;
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
            float slopeAngle = Vector3.Angle(
                Vector3.up,
                slopeInfo.normal
            );

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

    public Rigidbody GetPlayerRigidBody()
    {
        return body;
    }
}
