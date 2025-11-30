using UnityEngine;

[CreateAssetMenu(fileName = "NewPowerUp", menuName = "Blood Covenant/Power Up Data")]
public class PowerUpData : ScriptableObject
{
    public string powerUpName;
    public PowerUpType type;
    public float duration = 10f;
    public float effectValue = 2f;
    public Color glowColor = Color.yellow;
    public GameObject visualEffectPrefab;
    public AudioClip pickupSound;

    [Header("Lore Collectible (Only for LorePickup type)")]
    [TextArea(3, 10)]
    public string loreText;
    public AudioClip loreAudioClip;
}

public enum PowerUpType
{
    HolyWater,
    BloodFrenzy,
    LorePickup,
    LunarEssence,
    HealthPack,
    DamageBoost,
    SpeedBoost
}