using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Blood Covenant/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;
    public EnemyType enemyType;

    [Header("Stats")]
    public float maxHealth = 50f;
    public float damage = 10f;
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Behavior")]
    public bool startsCrawling = false;
    public bool canStandUp = false;
    public float standUpHealthThreshold = 0.5f;

    [Header("Rewards")]
    public int scoreValue = 100;
    public float powerUpDropChance = 0.1f;

    [Header("Visuals")]
    public Color glowColor = Color.red;
}

public enum EnemyType
{
    Thrall,
    ShadowSpawn,
    FeralLycan,
    CrimsonKnight,
    LichWarden,
    Hellhound,
    Crawler,
    Boss
}