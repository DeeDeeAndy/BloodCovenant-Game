using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Data")]
    public EnemyData enemyData;

    [Header("Audio")]
    public AudioClip[] idleSounds;
    public AudioClip[] attackSounds;
    public AudioClip[] hitSounds;
    public AudioClip[] deathSounds;
    public AudioClip[] footstepSounds;
    public AudioClip screamSound;
    [Range(0f, 1f)] public float idleSoundChance = 0.3f;
    public float minIdleSoundInterval = 3f;
    public float maxIdleSoundInterval = 8f;
    private AudioSource audioSource;
    private float nextIdleSoundTime;

    private float currentHealth;
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isCrawling = false;

    private int speedHash;
    private int attackHash;
    private int attack2Hash;
    private int attack3Hash;
    private int hitHash;
    private int dieHash;
    private int isCrawlingHash;
    private int standUpHash;
    private int screamHash;
    private bool hasSecondAttack = false;
    private bool hasThirdAttack = false;
    private bool hasStandUp = false;
    private bool hasScream = false;
    private float lastScreamTime;
    private float screamInterval;

    void Start()
    {
        if (enemyData == null)
        {
            Debug.LogError("No EnemyData assigned to " + gameObject.name);
            return;
        }

        currentHealth = enemyData.maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 20f;

        speedHash = Animator.StringToHash("Speed");
        attackHash = Animator.StringToHash("Attack");
        attack2Hash = Animator.StringToHash("Attack2");
        attack3Hash = Animator.StringToHash("Attack3");
        hitHash = Animator.StringToHash("Hit");
        dieHash = Animator.StringToHash("Die");
        isCrawlingHash = Animator.StringToHash("IsCrawling");
        standUpHash = Animator.StringToHash("StandUp");
        screamHash = Animator.StringToHash("Scream");

        if (animator != null)
        {
            hasSecondAttack = HasParameter("Attack2");
            hasThirdAttack = HasParameter("Attack3");
            hasStandUp = HasParameter("StandUp");
            hasScream = HasParameter("Scream");

            if (enemyData.startsCrawling)
            {
                isCrawling = true;
                animator.SetBool(isCrawlingHash, true);
            }
        }

        screamInterval = Random.Range(8f, 15f);
        lastScreamTime = Time.time;
        nextIdleSoundTime = Time.time + Random.Range(minIdleSoundInterval, maxIdleSoundInterval);

        if (agent != null)
        {
            agent.speed = enemyData.moveSpeed;
            agent.stoppingDistance = enemyData.attackRange * 0.8f;
        }
    }

    bool HasParameter(string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    void Update()
    {
        if (isDead || player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= enemyData.attackRange)
        {
            agent.SetDestination(transform.position);
            TryAttack();
        }
        else
        {
            agent.SetDestination(player.position);
        }

        if (animator != null)
            animator.SetFloat(speedHash, agent.velocity.magnitude);

        if (distanceToPlayer <= enemyData.attackRange * 1.5f)
        {
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 10f * Time.deltaTime);
        }

        TryScream();
        TryIdleSound();
    }

    void TryScream()
    {
        if (!hasScream) return;

        if (Time.time - lastScreamTime >= screamInterval)
        {
            lastScreamTime = Time.time;
            screamInterval = Random.Range(8f, 15f);
            animator.SetTrigger(screamHash);
            PlayScreamSound();
        }
    }

    void TryIdleSound()
    {
        if (Time.time >= nextIdleSoundTime && idleSounds.Length > 0)
        {
            if (Random.value <= idleSoundChance)
            {
                PlayRandomSound(idleSounds);
            }
            nextIdleSoundTime = Time.time + Random.Range(minIdleSoundInterval, maxIdleSoundInterval);
        }
    }

    void PlayRandomSound(AudioClip[] clips)
    {
        if (clips.Length == 0 || audioSource == null) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip);
    }

    void PlayScreamSound()
    {
        if (screamSound != null && audioSource != null)
            audioSource.PlayOneShot(screamSound);
    }

    // Animation Event Methods - Call these from animations
    public void OnFootstep()
    {
        PlayRandomSound(footstepSounds);
    }

    public void OnAttackSound()
    {
        PlayRandomSound(attackSounds);
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime >= enemyData.attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            int attackChoice = Random.Range(0, 3);

            if (attackChoice == 0)
                animator.SetTrigger(attackHash);
            else if (attackChoice == 1 && hasSecondAttack)
                animator.SetTrigger(attack2Hash);
            else if (attackChoice == 2 && hasThirdAttack)
                animator.SetTrigger(attack3Hash);
            else
                animator.SetTrigger(attackHash);
        }

        PlayRandomSound(attackSounds);

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(enemyData.damage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (animator != null && hitHash != 0)
            animator.SetTrigger(hitHash);

        PlayRandomSound(hitSounds);

        if (isCrawling && enemyData.canStandUp && hasStandUp)
        {
            float healthPercent = currentHealth / enemyData.maxHealth;
            if (healthPercent <= enemyData.standUpHealthThreshold)
            {
                StandUp();
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void StandUp()
    {
        isCrawling = false;
        if (animator != null)
        {
            animator.SetBool(isCrawlingHash, false);
            animator.SetTrigger(standUpHash);
        }
        agent.speed = enemyData.moveSpeed * 1.5f;
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger(dieHash);

        PlayRandomSound(deathSounds);

        if (agent != null)
            agent.enabled = false;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(enemyData.scoreValue);

        if (PowerUpSpawner.Instance != null)
            PowerUpSpawner.Instance.TrySpawnPowerUp(transform.position, enemyData.powerUpDropChance);

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyDeath();

        Destroy(gameObject, 2f);
    }

    public int GetScoreValue()
    {
        return enemyData.scoreValue;
    }

    public float GetPowerUpDropChance()
    {
        return enemyData.powerUpDropChance;
    }

    public bool IsDead()
    {
        return isDead;
    }
}