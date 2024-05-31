using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Wall Running")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;

    public float wallRunForce, maxWallRunTime;
    public float wallClimbSpeed;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;

    [Header("Reference")]
    public Transform orientation;

    [Header("Input")]
    public KeyCode downWardsRunKey = KeyCode.E;

    private PlayerMovement pm;

    private Rigidbody rb;

    private RaycastHit forwardWallHit;

    private bool wallLeft;
    private bool wallRight;
    private bool wallForward;
    private bool upwardsRunning, downwardsRunning;

    private float _X, _Y;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallRunning)
            WallRunningMovement();
    }

    void CheckForWall()
    {
        wallForward = Physics.Raycast(transform.position, orientation.forward, out forwardWallHit, wallCheckDistance, whatIsWall);

        if (wallForward)
        {
            upwardsRunning = WallTouched();
        }
        else
        {
            upwardsRunning = false;
        }
    }

    void StateMachine()
    {
        _X = Input.GetAxisRaw("Horizontal");
        _Y = Input.GetAxisRaw("Vertical");

        downwardsRunning = Input.GetKey(downWardsRunKey);

        if ((wallLeft || wallRight || wallForward) && _Y > 0 && AboveGround())
        {
            if (!pm.wallRunning)
                StartWallRun();
        }
        else
        {
            if (pm.wallRunning)
                StopWallRun();
        }
    }

    void StartWallRun()
    {
        pm.wallRunning = true;
    }

    void WallRunningMovement()
    {
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        Vector3 wallNormal = forwardWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallClimbSpeed, rb.linearVelocity.z);

        if (downwardsRunning)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbSpeed, rb.linearVelocity.z);

        if (!(wallLeft && _X > 0) && !(wallRight && _X < 0))
            rb.AddForce(-wallForward * 100, ForceMode.Force);

        Vector3 lookDirection = -wallNormal;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        }
    }

    void StopWallRun()
    {
        pm.wallRunning = false;
        rb.useGravity = true;
    }

    bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    bool WallTouched()
    {
        float distanceToWall = forwardWallHit.distance;
        if (distanceToWall < 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position,forwardWallHit.point);
    }
}
