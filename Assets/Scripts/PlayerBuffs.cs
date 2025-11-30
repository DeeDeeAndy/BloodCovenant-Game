using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBuffs : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private Dictionary<PowerUpType, Coroutine> activeBuffs = new Dictionary<PowerUpType, Coroutine>();

    private float baseWalkSpeed;
    private float baseRunSpeed;
    private float baseMeleeDamage;
    private float baseRangedDamage;

    public bool IsInvincible { get; private set; }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();

        baseWalkSpeed = playerController.walkSpeed;
        baseRunSpeed = playerController.runSpeed;
        baseMeleeDamage = playerCombat.meleeDamage;
        baseRangedDamage = playerCombat.rangedDamage;
    }

    public void ApplyPowerUp(PowerUpData powerUp)
    {
        if (powerUp.type == PowerUpType.LorePickup)
        {
            DisplayLore(powerUp);
            return;
        }

        if (activeBuffs.ContainsKey(powerUp.type))
        {
            StopCoroutine(activeBuffs[powerUp.type]);
            activeBuffs.Remove(powerUp.type);
        }

        Coroutine buffCoroutine = StartCoroutine(PowerUpRoutine(powerUp));
        activeBuffs[powerUp.type] = buffCoroutine;

        Debug.Log($"Power-up activated: {powerUp.powerUpName} for {powerUp.duration}s");
    }

    IEnumerator PowerUpRoutine(PowerUpData powerUp)
    {
        ApplyEffect(powerUp, true);
        yield return new WaitForSeconds(powerUp.duration);
        ApplyEffect(powerUp, false);
        activeBuffs.Remove(powerUp.type);

        Debug.Log($"Power-up expired: {powerUp.powerUpName}");
    }

    void ApplyEffect(PowerUpData powerUp, bool apply)
    {
        float multiplier = apply ? powerUp.effectValue : 1f;

        switch (powerUp.type)
        {
            case PowerUpType.HolyWater:
                IsInvincible = apply;
                break;

            case PowerUpType.BloodFrenzy:
                playerCombat.meleeDamage = baseMeleeDamage * multiplier;
                playerCombat.rangedDamage = baseRangedDamage * multiplier;
                if (apply) StartCoroutine(BloodFrenzyDrain(powerUp.duration));
                break;

            case PowerUpType.LunarEssence:
                playerController.walkSpeed = baseWalkSpeed * multiplier;
                playerController.runSpeed = baseRunSpeed * multiplier;
                break;

            case PowerUpType.HealthPack:
                if (apply) playerController.Heal(powerUp.effectValue);
                break;

            case PowerUpType.DamageBoost:
                playerCombat.meleeDamage = baseMeleeDamage * multiplier;
                playerCombat.rangedDamage = baseRangedDamage * multiplier;
                break;

            case PowerUpType.SpeedBoost:
                playerController.walkSpeed = baseWalkSpeed * multiplier;
                playerController.runSpeed = baseRunSpeed * multiplier;
                break;
        }

        if (!apply)
        {
            playerController.walkSpeed = baseWalkSpeed;
            playerController.runSpeed = baseRunSpeed;
            playerCombat.meleeDamage = baseMeleeDamage;
            playerCombat.rangedDamage = baseRangedDamage;
        }
    }

    void DisplayLore(PowerUpData powerUp)
    {
        if (LoreManager.Instance != null)
            LoreManager.Instance.ShowLore(powerUp.loreText, powerUp.loreAudioClip);
        else
            Debug.Log($"LORE: {powerUp.loreText}");
    }

    IEnumerator BloodFrenzyDrain(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            playerController.TakeDamage(1f);
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }
    }

    public bool HasBuff(PowerUpType type)
    {
        return activeBuffs.ContainsKey(type);
    }
}