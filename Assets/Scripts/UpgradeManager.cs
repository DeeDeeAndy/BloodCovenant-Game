using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Available Upgrades")]
    public List<UpgradeData> allUpgrades;

    private List<UpgradeData> purchasedUpgrades = new List<UpgradeData>();
    private int currency = 0;

    private PlayerController playerController;
    private PlayerCombat playerCombat;

    public event System.Action<int> OnCurrencyChanged;
    public event System.Action<UpgradeData> OnUpgradePurchased;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerCombat = player.GetComponent<PlayerCombat>();
        }
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        OnCurrencyChanged?.Invoke(currency);
        Debug.Log($"Currency: {currency}");
    }

    public bool CanAfford(UpgradeData upgrade)
    {
        return currency >= upgrade.cost;
    }

    public bool HasPrerequisite(UpgradeData upgrade)
    {
        if (upgrade.prerequisite == null)
            return true;

        return purchasedUpgrades.Contains(upgrade.prerequisite);
    }

    public bool AlreadyOwned(UpgradeData upgrade)
    {
        return purchasedUpgrades.Contains(upgrade);
    }

    public bool TryPurchaseUpgrade(UpgradeData upgrade)
    {
        if (AlreadyOwned(upgrade))
        {
            Debug.Log("Already owned!");
            return false;
        }

        if (!HasPrerequisite(upgrade))
        {
            Debug.Log("Missing prerequisite!");
            return false;
        }

        if (!CanAfford(upgrade))
        {
            Debug.Log("Not enough currency!");
            return false;
        }

        currency -= upgrade.cost;
        purchasedUpgrades.Add(upgrade);
        ApplyUpgrade(upgrade);

        OnCurrencyChanged?.Invoke(currency);
        OnUpgradePurchased?.Invoke(upgrade);

        Debug.Log($"Purchased: {upgrade.upgradeName}");
        return true;
    }

    void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.MaxHealth:
                playerController.maxHealth += upgrade.effectValue;
                playerController.Heal(upgrade.effectValue);
                break;

            case UpgradeType.MeleeDamage:
                playerCombat.meleeDamage += upgrade.effectValue;
                break;

            case UpgradeType.RangedDamage:
                playerCombat.rangedDamage += upgrade.effectValue;
                break;

            case UpgradeType.MoveSpeed:
                playerController.walkSpeed += upgrade.effectValue;
                playerController.runSpeed += upgrade.effectValue;
                break;
        }
    }

    public int GetCurrency() => currency;
    public List<UpgradeData> GetPurchasedUpgrades() => purchasedUpgrades;
    public List<UpgradeData> GetAvailableUpgrades() => allUpgrades;
}