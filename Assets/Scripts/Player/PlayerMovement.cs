using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //<----------------------------Public-Variables-------------------------->
    [Header("Character Values")]
    public float speed;
    public float runSpeed;
    public int jumpForce;

    //<----------------------------Private-Variables-------------------------->
    private Animator anim;
    private Rigidbody rb;

    private float _X, _Y;
    private float currentSpeed;

    private bool inputRun = false;
    private bool inputJump = false;
    private bool groundCheck = true;

    private Vector3 velocity;
    private Vector3 lookDirection;
    //<------------------------------------Variables-------------------------->

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        currentSpeed = speed;
    }

    private void Update()
    {
        _X = Input.GetAxis("Horizontal");
        _Y = Input.GetAxis("Vertical");

        inputRun = Input.GetKey(KeyCode.LeftShift);
        inputJump = Input.GetKeyDown(KeyCode.Space);

        if (inputRun)
            StateFast(runSpeed, 1, 0.5f);
        else
            StateFast(speed, 0, 0.1f);

        if (inputJump && groundCheck)
            Jump();

        anim.SetFloat("velocity", rb.linearVelocity.magnitude, 0.01f, Time.deltaTime);

        Vector3 movement = new Vector3(_X, 0, _Y);
        if (movement.magnitude > 0.1f)
        {
            lookDirection = movement.normalized;
        }
    }

    private void FixedUpdate()
    {
        velocity = new Vector3(_X * currentSpeed * Time.deltaTime, rb.linearVelocity.y, _Y * currentSpeed * Time.deltaTime);

        rb.linearVelocity = velocity;

        if (lookDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void StateFast(float _speed, int _state, float _dampTime)
    {
        currentSpeed = _speed;
        anim.SetFloat("speed", _state, _dampTime, Time.deltaTime);
    }

    void Jump()
    {
        anim.SetBool("Jump", true);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        groundCheck = false;
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
