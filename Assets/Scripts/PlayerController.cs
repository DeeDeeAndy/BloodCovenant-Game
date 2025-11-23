using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;

    [Header("State")]
    private bool isCombatMode = false;
    private bool isSitting = false;
    private float verticalVelocity = 0f;
    private float gravity = -9.81f;

    [Header("References")]
    private Animator animator;
    private CharacterController characterController;

    [Header("Animation Parameters")]
    private int speedXParam;
    private int speedZParam;
    private int isGroundedParam;
    private int isMovingParam;
    private int attackParam;
    private int meleeAttackParam;
    private int isCombatModeParam;
    private int jumpParam;
    private int rollParam;
    private int getHitParam;
    private int dieParam;
    private int startSittingParam;
    private int standUpParam;
    private int sittingMoodParam;
    private int sitClapParam;
    private int sitVictoryParam;
    private int sitThumbsUpParam;
    private int victoryParam;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        speedXParam = Animator.StringToHash("SpeedX");
        speedZParam = Animator.StringToHash("SpeedZ");
        isGroundedParam = Animator.StringToHash("IsGrounded");
        isMovingParam = Animator.StringToHash("IsMoving");
        attackParam = Animator.StringToHash("Attack");
        meleeAttackParam = Animator.StringToHash("MeleeAttack");
        isCombatModeParam = Animator.StringToHash("IsCombatMode");
        jumpParam = Animator.StringToHash("Jump");
        rollParam = Animator.StringToHash("Roll");
        getHitParam = Animator.StringToHash("GetHit");
        dieParam = Animator.StringToHash("Die");
        startSittingParam = Animator.StringToHash("StartSitting");
        standUpParam = Animator.StringToHash("StandUp");
        sittingMoodParam = Animator.StringToHash("SittingMood");
        sitClapParam = Animator.StringToHash("SitClap");
        sitVictoryParam = Animator.StringToHash("SitVictory");
        sitThumbsUpParam = Animator.StringToHash("SitThumbsUp");
        victoryParam = Animator.StringToHash("Victory");
    }

    void Update()
    {
        if (!isSitting)
        {
            HandleMovement();
            HandleCombat();
            HandleActions();
        }

        HandleSitting();
        TestControls();
    }

    void HandleMovement()
    {
        bool isGrounded = characterController.isGrounded;
        animator.SetBool(isGroundedParam, isGrounded);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 movement = new Vector3(horizontal, 0, vertical);
        float magnitude = movement.magnitude;

        if (magnitude > 1f)
        {
            movement.Normalize();
            magnitude = 1f;
        }

        Vector3 moveVector = movement * currentSpeed;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveVector.y = verticalVelocity;
        characterController.Move(moveVector * Time.deltaTime);

        if (movement.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(movement.x, 0, movement.z), Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        float normalizedSpeed = magnitude * (Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f);
        animator.SetFloat(speedXParam, horizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(speedZParam, vertical * normalizedSpeed, 0.1f, Time.deltaTime);
        animator.SetBool(isMovingParam, magnitude > 0.1f);
    }

    void HandleCombat()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isCombatMode = !isCombatMode;
            animator.SetBool(isCombatModeParam, isCombatMode);
            animator.SetLayerWeight(1, isCombatMode ? 1f : 0f);
        }

        if (Input.GetMouseButtonDown(0) && isCombatMode)
        {
            animator.SetTrigger(attackParam);
        }

        if (Input.GetKeyDown(KeyCode.E) || (Input.GetMouseButtonDown(0) && !isCombatMode))
        {
            animator.SetTrigger(meleeAttackParam);
        }
    }

    void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
            animator.SetTrigger(jumpParam);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetTrigger(rollParam);
        }
    }

    void HandleSitting()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isSitting)
        {
            isSitting = true;
            animator.SetTrigger(startSittingParam);
            animator.SetFloat(sittingMoodParam, 0f);
        }

        if (Input.GetKeyDown(KeyCode.F) && isSitting)
        {
            isSitting = false;
            animator.SetTrigger(standUpParam);
        }

        if (isSitting)
        {
            float currentMood = animator.GetFloat(sittingMoodParam);

            if (Input.GetKey(KeyCode.Q))
            {
                currentMood += Time.deltaTime * 0.5f;
                currentMood = Mathf.Clamp01(currentMood);
                animator.SetFloat(sittingMoodParam, currentMood);
            }

            if (Input.GetKey(KeyCode.E))
            {
                currentMood -= Time.deltaTime * 0.5f;
                currentMood = Mathf.Clamp01(currentMood);
                animator.SetFloat(sittingMoodParam, currentMood);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) animator.SetFloat(sittingMoodParam, 0f);
            if (Input.GetKeyDown(KeyCode.Alpha2)) animator.SetFloat(sittingMoodParam, 0.5f);
            if (Input.GetKeyDown(KeyCode.Alpha3)) animator.SetFloat(sittingMoodParam, 1f);
            if (Input.GetKeyDown(KeyCode.Alpha4)) animator.SetTrigger(sitClapParam);
            if (Input.GetKeyDown(KeyCode.Alpha5)) animator.SetTrigger(sitVictoryParam);
            if (Input.GetKeyDown(KeyCode.Alpha6)) animator.SetTrigger(sitThumbsUpParam);
        }
    }

    void TestControls()
    {
        if (Input.GetKeyDown(KeyCode.H)) animator.SetTrigger(getHitParam);
        if (Input.GetKeyDown(KeyCode.K)) animator.SetTrigger(dieParam);
        if (Input.GetKeyDown(KeyCode.V)) animator.SetTrigger(victoryParam);
    }

    public void OnFootstep()
    {
        Debug.Log("Footstep!");
    }

    public void OnShootProjectile()
    {
        Debug.Log("Fire crossbow!");
    }

    public void OnMeleeHit()
    {
        Debug.Log("Melee attack!");
    }

    public void OnLandingImpact()
    {
        Debug.Log("Landing!");
    }

    public void OnRollStart()
    {
        Debug.Log("Roll!");
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Took " + damage + " damage!");
        animator.SetTrigger(getHitParam);
    }

    public void Die()
    {
        animator.SetTrigger(dieParam);
        enabled = false;
    }

    public void PlayVictory()
    {
        animator.SetTrigger(victoryParam);
    }
}