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

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;
    private bool isDying = false;

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
    private int meleeAttackHash;
    private int shootHash;
    private int isAimingHash;
    private int hitHash;
    private int dieHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        currentHealth = maxHealth;

        moveXHash = Animator.StringToHash("MoveX");
        moveYHash = Animator.StringToHash("MoveY");
        isRunningHash = Animator.StringToHash("IsRunning");
        jumpHash = Animator.StringToHash("Jump");
        isGroundedHash = Animator.StringToHash("IsGrounded");
        meleeAttackHash = Animator.StringToHash("MeleeAttack");
        shootHash = Animator.StringToHash("Shoot");
        isAimingHash = Animator.StringToHash("IsAiming");
        hitHash = Animator.StringToHash("Hit");
        dieHash = Animator.StringToHash("Die");
    }

    void Update()
    {
        if (isDead) return;

        if (isDying)
        {
            HandleDying();
            return;
        }

        HandleMovement();
        HandleJump();
        HandleCombat();
        HandleTestInputs();
    }

    void HandleDying()
    {
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded)
        {
            isDead = true;
            animator.SetTrigger(dieHash);
            animator.SetLayerWeight(1, 0f);
            characterController.enabled = false;
            Debug.Log("Player hit ground and died!");
        }
    }

    void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
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
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger(jumpHash);
        }
    }

    void HandleCombat()
    {
        if (Input.GetMouseButtonDown(1))
        {
            bool currentAiming = animator.GetBool(isAimingHash);
            animator.SetBool(isAimingHash, !currentAiming);
            animator.SetLayerWeight(1, !currentAiming ? 1f : 0f);
        }

        bool isAiming = animator.GetBool(isAimingHash);

        if (Input.GetMouseButtonDown(0))
        {
            if (isAiming)
            {
                animator.SetTrigger(shootHash);
            }
            else
            {
                animator.SetTrigger(meleeAttackHash);
            }
        }
    }

    void HandleTestInputs()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(20f);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(20f);
        }
    }

    public void OnShootProjectile()
    {
        Debug.Log("Crossbow fired!");
    }

    public void OnMeleeHit()
    {
        Debug.Log("Melee hit!");
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isDying) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log("Took " + damage + " damage! Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger(hitHash);
        }
    }

    public void Heal(float amount)
    {
        if (isDead || isDying) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log("Healed " + amount + "! Health: " + currentHealth);
    }

    void Die()
    {
        if (characterController.isGrounded)
        {
            isDead = true;
            animator.SetTrigger(dieHash);
            animator.SetLayerWeight(1, 0f);
            characterController.enabled = false;
            Debug.Log("Player died!");
        }
        else
        {
            isDying = true;
            Debug.Log("Player is dying, falling to ground...");
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsDead()
    {
        return isDead || isDying;
    }
}