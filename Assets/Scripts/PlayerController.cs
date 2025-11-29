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

    [Header("Audio")]
    public AudioClip footstepSound;
    public AudioClip rollSound;
    public AudioClip landSound;
    public AudioClip[] hurtSounds;
    public AudioClip deathSound;
    private AudioSource audioSource;

    private Animator animator;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;
    private bool isSitting = false;

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
    private int rollHash;
    private int victoryHash;
    private int startSittingHash;
    private int standUpHash;
    private int sittingMoodHash;
    private int sitVictoryHash;
    private int sitThumbsUpHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

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
        rollHash = Animator.StringToHash("Roll");
        victoryHash = Animator.StringToHash("Victory");
        startSittingHash = Animator.StringToHash("StartSitting");
        standUpHash = Animator.StringToHash("StandUp");
        sittingMoodHash = Animator.StringToHash("SittingMood");
        sitVictoryHash = Animator.StringToHash("SitVictory");
        sitThumbsUpHash = Animator.StringToHash("SitThumbsUp");
    }

    void Update()
    {
        if (isDead) return;

        if (isDying)
        {
            HandleDying();
            return;
        }

        if (isSitting)
        {
            HandleSitting();
            return;
        }

        HandleMovement();
        HandleJump();
        HandleCombat();
        HandleActions();
        HandleTestInputs();
    }

    void HandleMovement()
    {
        isGrounded = characterController.isGrounded;
        animator.SetBool(isGroundedHash, isGrounded);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        animator.SetFloat(moveXHash, horizontal);
        animator.SetFloat(moveYHash, vertical);

        isRunning = Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);
        animator.SetBool(isRunningHash, isRunning);

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;

        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();

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

        if (Input.GetMouseButtonDown(0))
        {
            if (animator.GetBool(isAimingHash))
                animator.SetTrigger(shootHash);
            else
                animator.SetTrigger(meleeAttackHash);
        }
    }

    void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
            animator.SetTrigger(rollHash);

        if (Input.GetKeyDown(KeyCode.F))
        {
            isSitting = true;
            animator.SetTrigger(startSittingHash);
            animator.SetFloat(sittingMoodHash, 0f);
        }
    }

    void HandleSitting()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isSitting = false;
            animator.SetTrigger(standUpHash);
            return;
        }

        float currentMood = animator.GetFloat(sittingMoodHash);

        if (Input.GetKey(KeyCode.Q))
            animator.SetFloat(sittingMoodHash, Mathf.Clamp01(currentMood + Time.deltaTime * 0.5f));

        if (Input.GetKey(KeyCode.R))
            animator.SetFloat(sittingMoodHash, Mathf.Clamp01(currentMood - Time.deltaTime * 0.5f));

        if (Input.GetKeyDown(KeyCode.Alpha1)) animator.SetFloat(sittingMoodHash, 0f);
        if (Input.GetKeyDown(KeyCode.Alpha2)) animator.SetFloat(sittingMoodHash, 0.5f);
        if (Input.GetKeyDown(KeyCode.Alpha3)) animator.SetFloat(sittingMoodHash, 1f);
        if (Input.GetKeyDown(KeyCode.Alpha4)) animator.SetTrigger(sitVictoryHash);
        if (Input.GetKeyDown(KeyCode.Alpha5)) animator.SetTrigger(sitThumbsUpHash);
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
        }
    }

    void HandleTestInputs()
    {
        if (Input.GetKeyDown(KeyCode.T))
            TakeDamage(20f);

        if (Input.GetKeyDown(KeyCode.H))
            Heal(20f);

        if (Input.GetKeyDown(KeyCode.V))
            animator.SetTrigger(victoryHash);
    }


    public void OnFootstep()
    {
        if (audioSource != null && footstepSound != null)
            audioSource.PlayOneShot(footstepSound);
    }

    public void OnRollStart()
    {
        if (audioSource != null && rollSound != null)
            audioSource.PlayOneShot(rollSound);
    }

    public void OnLand()
    {
        if (audioSource != null && landSound != null)
            audioSource.PlayOneShot(landSound);
    }

    void PlayRandomHurtSound()
    {
        if (audioSource != null && hurtSounds.Length > 0)
        {
            AudioClip clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }

    void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);
    }

    // Public Methods
    public void TakeDamage(float damage)
    {
        if (isDead || isDying) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        Debug.Log($"Took {damage} damage! Health: {currentHealth}");

        if (isSitting)
        {
            isSitting = false;
            animator.SetTrigger(standUpHash);
        }

        if (currentHealth <= 0)
            Die();
        else
        {
            animator.SetTrigger(hitHash);
            PlayRandomHurtSound();
        }
    }

    public void Heal(float amount)
    {
        if (isDead || isDying) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log($"Healed {amount}! Health: {currentHealth}");
    }

    void Die()
    {
        if (characterController.isGrounded)
        {
            isDead = true;
            animator.SetTrigger(dieHash);
            animator.SetLayerWeight(1, 0f);
            characterController.enabled = false;
            PlayDeathSound();
            Debug.Log("Player died!");
        }
        else
        {
            isDying = true;
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead || isDying;
    public bool IsSitting() => isSitting;

    public void PlayVictory()
    {
        if (isDead || isDying) return;
        animator.SetTrigger(victoryHash);
    }
}