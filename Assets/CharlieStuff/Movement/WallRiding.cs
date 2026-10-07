/*using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    public LayerMask wallLayer;
    public LayerMask groundLayer;

    public float wallRunForce = 20f;
    public float wallJumpUpForce = 8f;
    public float wallJumpSideForce = 5f;
    public float wallClimbSpeed = 4f;

    public float maxWallRunTime = 1.5f;

    [Header("Detection")]
    public float wallCheckDistance = 0.7f;
    public float minimumHeight = 1.5f;

    [Header("Exit")]
    public float exitWallTime = 0.2f;

    [Header("Gravity")]
    public bool useGravity = false;
    public float gravityCounterForce = 1f;

    [Header("References")]
    public Transform orientation;
    public PlayerCam cameraController;

    private PlayerMovement movement;
    private Rigidbody rb;

    private RaycastHit leftHit;
    private RaycastHit rightHit;

    private bool wallLeft;
    private bool wallRight;

    private bool exitingWall;

    private float wallRunTimer;
    private float exitTimer;

    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckWalls();
        HandleState();
    }

    private void FixedUpdate()
    {
        if (movement.wallRiding)
            MoveAlongWall();
    }

    private void CheckWalls()
    {
        wallLeft = Physics.Raycast(transform.position,
            -orientation.right,
            out leftHit,
            wallCheckDistance,
            wallLayer);

        wallRight = Physics.Raycast(transform.position,
            orientation.right,
            out rightHit,
            wallCheckDistance,
            wallLayer);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position,
            Vector3.down,
            minimumHeight,
            groundLayer);
    }

    private void HandleState()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if ((wallLeft || wallRight) &&
            verticalInput > 0 &&
            AboveGround() &&
            !exitingWall)
        {
            if (!movement.wallRiding)
                StartWallRun();

            wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0f)
            {
                exitingWall = true;
                exitTimer = exitWallTime;
            }

            if (Input.GetKeyDown(KeyCode.Space))
                WallJump();
        }
        else if (exitingWall)
        {
            if (movement.wallRiding)
                StopWallRun();

            exitTimer -= Time.deltaTime;

            if (exitTimer <= 0f)
                exitingWall = false;
        }
        else
        {
            if (movement.wallRiding)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        movement.wallRiding = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        cameraController.ChangeFOV(90f);

        if (wallLeft)
            cameraController.Tilt(-5f);

        if (wallRight)
            cameraController.Tilt(5f);
    }

    private void StopWallRun()
    {
        movement.wallRiding = false;

        cameraController.ChangeFOV(80f);
        cameraController.Tilt(0f);
    }

    private void MoveAlongWall()
    {
        rb.useGravity = useGravity;

        RaycastHit hit = wallRight ? rightHit : leftHit;
        Vector3 wallNormal = hit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if ((orientation.forward - wallForward).magnitude >
            (orientation.forward + wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (Input.GetKey(KeyCode.LeftShift))
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);

        if (Input.GetKey(KeyCode.LeftControl))
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        if (!(wallLeft && horizontalInput > 0) &&
            !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100f, ForceMode.Force);
        }

        if (useGravity)
            rb.AddForce(Vector3.up * gravityCounterForce, ForceMode.Force);
    }

    private void WallJump()
    {
        exitingWall = true;
        exitTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightHit.normal : leftHit.normal;

        Vector3 jumpForce =
            Vector3.up * wallJumpUpForce +
            wallNormal * wallJumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(jumpForce, ForceMode.Impulse);

        StopWallRun();
    }
}*/