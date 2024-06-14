using UnityEngine;

public enum MovementState { idle, walk, sprinting, wallrunning,spider }

public class PlayerMovement : MonoBehaviour
{
    //<----------------------------Public-Variables-------------------------->
    [Header("Character Values")]
    public float speed;
    public float runSpeed;
    public float wallRunSpeed;

    public int jumpForce;

    public bool wallRunning = false;

    [Header("State")]
    public MovementState state;

    [Header("Components")]
    //<----------------------------Private-Variables-------------------------->
    public Animator anim;
    private Rigidbody rb;

    private float _X, _Y;
    private float currentSpeed;

    private bool inputRun = false;
    private bool inputJump = false;
    private bool groundCheck = true;

    private Vector3 velocity;
    private Vector3 lookDirection;

    private Transform cameraTransform;
    //<------------------------------------Variables-------------------------->

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        currentSpeed = speed;

        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        _X = Input.GetAxis("Horizontal");
        _Y = Input.GetAxis("Vertical");

        inputRun = Input.GetKey(KeyCode.LeftShift);
        inputJump = Input.GetKeyDown(KeyCode.Space);

        if (inputRun)
            StateFast(runSpeed, 1, 0.5f,MovementState.sprinting);
        else
            StateFast(speed, 0, 0.1f, MovementState.walk);
        
        if (inputJump && groundCheck)
            Jump();

        if (wallRunning)
            WallRunning();

        anim.SetFloat("velocity", rb.linearVelocity.magnitude, 0, Time.deltaTime);

        anim.SetBool("climb", wallRunning);

        if (Movement().magnitude > 0.1f)
            lookDirection = Movement().normalized;
        else
            Ýdle();
    }

    private void FixedUpdate()
    {
        if (lookDirection == Vector3.zero) 
            return;

        rb.linearVelocity = Velocity();

        if(!wallRunning) 
            rb.rotation = Rotation();
    }

    void StateFast(float _speed, int _state, float _dampTime,MovementState _mState)
    {
        state = _mState;
        currentSpeed = _speed;
        anim.SetFloat("speed", _state, _dampTime, Time.deltaTime);
    }

    void Jump()
    {
        anim.SetBool("Jump", true);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        groundCheck = false;
    }

    void Ýdle()
    {
        lookDirection = Vector3.zero;
        state = MovementState.idle;
    }

    void WallRunning()
    {
        state = MovementState.wallrunning;
        currentSpeed = wallRunSpeed;
    }

    Vector3 Movement()
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        return camForward * _Y + camRight * _X;
    }

    Quaternion Rotation()
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        return Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f);
    }

    Vector3 Velocity()
    {
        velocity = lookDirection * currentSpeed * Time.deltaTime;
        velocity.y = rb.linearVelocity.y;

        return velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            anim.SetBool("Jump", false);
            groundCheck = true;
        }
    }
}
