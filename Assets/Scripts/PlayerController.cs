using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 5.0f;
    public float turnSpeed = 120.0f;

    [Header("Jump Settings")]
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;

    private Animator animator;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;

    private int moveXHash;
    private int moveYHash;
    private int isRunningHash;
    private int jumpHash;
    private int isGroundedHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        moveXHash = Animator.StringToHash("MoveX");
        moveYHash = Animator.StringToHash("MoveY");
        isRunningHash = Animator.StringToHash("IsRunning");
        jumpHash = Animator.StringToHash("Jump");
        isGroundedHash = Animator.StringToHash("IsGrounded");
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger(jumpHash);
        }
    }

    void HandleMovement()
    {
        isGrounded = characterController.isGrounded;
        animator.SetBool(isGroundedHash, isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        animator.SetFloat(moveXHash, horizontal);
        animator.SetFloat(moveYHash, vertical);

        isRunning = Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);
        animator.SetBool(isRunningHash, isRunning);

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(0, mouseX * turnSpeed * Time.deltaTime, 0);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        float animSpeedMultiplier = isRunning ? 2f : 1f;
        animator.SetFloat(moveXHash, horizontal * animSpeedMultiplier, 0.1f, Time.deltaTime);
        animator.SetFloat(moveYHash, vertical * animSpeedMultiplier, 0.1f, Time.deltaTime);
    }
}