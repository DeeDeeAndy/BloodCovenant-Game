using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Blood Covenant/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [Header("Info")]
    public string upgradeName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
    public UpgradePath path;

    [Header("Cost")]
    public int cost = 100;

    [Header("Effects")]
    public UpgradeType upgradeType;
    public float effectValue = 1f;

    [Header("Requirements")]
    public int requiredLevel = 1;
    public UpgradeData prerequisite;
}

public enum UpgradePath
{
    Vampire,
    Wraith,
    Holy,
    Lycan,
    General
}

public enum UpgradeType
{
    MaxHealth,
    HealthRegen,
    MeleeDamage,
    RangedDamage,
    AttackSpeed,
    MoveSpeed,
    CritChance,
    LifeSteal,
    DamageReduction,
    XPBoost
}