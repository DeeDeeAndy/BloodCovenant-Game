using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Melee Settings")]
    public float meleeRange = 2f;
    public float meleeDamage = 25f;
    public float meleeRadius = 1f;
    public Transform meleePoint;
    public LayerMask enemyLayer;

    [Header("Ranged Settings")]
    public float rangedDamage = 20f;
    public float rangedRange = 50f;
    public Transform firePoint;
    public LayerMask shootableLayers;
    public GameObject muzzleFlashPrefab;
    public GameObject hitEffectPrefab;

    [Header("Audio")]
    public AudioClip meleeHitSound;
    public AudioClip meleeMissSound;
    public AudioClip shootSound;
    private AudioSource audioSource;

    [Header("Combo System")]
    public float comboWindow = 1f;
    private int comboCount = 0;
    private Coroutine comboResetCoroutine;

    private Animator animator;
    private bool canAttack = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (meleePoint == null)
            meleePoint = transform;

        if (firePoint == null)
            firePoint = transform;
    }

    public void OnMeleeHit()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(meleePoint.position, meleeRadius, enemyLayer);

        bool hitSomething = false;

        foreach (Collider enemy in hitEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null && !enemyScript.IsDead())
            {
                float damage = meleeDamage * (1f + (comboCount * 0.1f));
                enemyScript.TakeDamage(damage);
                hitSomething = true;
            }
        }

        if (hitSomething)
        {
            if (audioSource != null && meleeHitSound != null)
                audioSource.PlayOneShot(meleeHitSound);

            comboCount++;
            if (comboResetCoroutine != null)
                StopCoroutine(comboResetCoroutine);
            comboResetCoroutine = StartCoroutine(ResetCombo());
        }
        else
        {
            if (audioSource != null && meleeMissSound != null)
                audioSource.PlayOneShot(meleeMissSound);
        }
    }

    IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(comboWindow);
        comboCount = 0;
    }

    public void OnShootProjectile()
    {
        if (firePoint == null)
            firePoint = transform;

        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(flash, 0.5f);
        }

        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);

        RaycastHit hit;
        Vector3 shootDirection = firePoint.forward;

        if (Physics.Raycast(firePoint.position, shootDirection, out hit, rangedRange, shootableLayers))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead())
            {
                enemy.TakeDamage(rangedDamage);
            }

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (meleePoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleePoint.position, meleeRadius);
    }
}