using UnityEngine;
using System;
using System.Collections.Generic;

public class UpgradeSystem : MonoBehaviour
{
    public PlayerThrow playerThrow;
    public TrashBin trashBin;
    public TrashCollection trashCollection;
    public TrashSpawner trashSpawner;
    public EnemyAI enemyAI;

    public int maxUpgradeLevel = 5;

    public AudioClip upgradePurchaseClip;

    public Dictionary<UpgradeType, int> currentUpgradeLevels = new Dictionary<UpgradeType, int>();

    public int upgradeCurrency = 0;
    public float scoreToCurrencyRate = 0.1f; // Example: 10 score = 1 currency

    public enum UpgradeType
    {
        ThrowPower,
        TrashBinCapacity,
        TrashBinHealth,
        Damage,
        CompressionSpeed,
        CollectionRadius,
        PortalSpawnRate,
        PortalSpawnRadius,
        PortalSpawnCount
    }

    private Dictionary<UpgradeType, float[]> upgradeValues = new Dictionary<UpgradeType, float[]>();
    public Dictionary<UpgradeType, int[]> upgradeCosts = new Dictionary<UpgradeType, int[]>();

    void Start()
    {
        // Initialize current levels
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            currentUpgradeLevels[type] = 0;
        }

        // Initialize upgrade values per level (example values, can be adjusted)
        upgradeValues[UpgradeType.ThrowPower] = new float[] { 2f, 4f, 6f, 8f, 10f };
        upgradeValues[UpgradeType.TrashBinCapacity] = new float[] { 5f, 10f, 15f, 20f, 25f };
        upgradeValues[UpgradeType.TrashBinHealth] = new float[] { 20f, 40f, 60f, 80f, 100f };
        upgradeValues[UpgradeType.Damage] = new float[] { 5f, 10f, 15f, 20f, 25f };
        upgradeValues[UpgradeType.CompressionSpeed] = new float[] { 0.4f, 0.3f, 0.2f, 0.1f, 0.05f }; // lower is faster
        upgradeValues[UpgradeType.CollectionRadius] = new float[] { 4f, 5f, 6f, 7f, 8f };
        upgradeValues[UpgradeType.PortalSpawnRate] = new float[] { 4f, 3f, 2f, 1f, 0.5f }; // lower is faster spawn
        upgradeValues[UpgradeType.PortalSpawnRadius] = new float[] { 12f, 14f, 16f, 18f, 20f };
        upgradeValues[UpgradeType.PortalSpawnCount] = new float[] { 1f, 2f, 3f, 4f, 5f };

        // Initialize upgrade costs per level (example values)
        upgradeCosts[UpgradeType.ThrowPower] = new int[] { 10, 20, 30, 40, 50 };
        upgradeCosts[UpgradeType.TrashBinCapacity] = new int[] { 15, 30, 45, 60, 75 };
        upgradeCosts[UpgradeType.TrashBinHealth] = new int[] { 15, 30, 45, 60, 75 };
        upgradeCosts[UpgradeType.Damage] = new int[] { 20, 40, 60, 80, 100 };
        upgradeCosts[UpgradeType.CompressionSpeed] = new int[] { 10, 20, 30, 40, 50 };
        upgradeCosts[UpgradeType.CollectionRadius] = new int[] { 10, 20, 30, 40, 50 };
        upgradeCosts[UpgradeType.PortalSpawnRate] = new int[] { 25, 50, 75, 100, 125 };
        upgradeCosts[UpgradeType.PortalSpawnRadius] = new int[] { 20, 40, 60, 80, 100 };
        upgradeCosts[UpgradeType.PortalSpawnCount] = new int[] { 30, 60, 90, 120, 150 };

        // Apply initial upgrades (level 0 means base values)
        ApplyAllUpgrades();
    }

    public void ConvertScoreToCurrency()
    {
        if (ScoreManager.Instance != null)
        {
            int score = ScoreManager.Instance.GetScore();
            int currencyToAdd = Mathf.FloorToInt(score * scoreToCurrencyRate);
            if (currencyToAdd > 0)
            {
                upgradeCurrency += currencyToAdd;
                ScoreManager.Instance.AddScore(-score);
                Debug.Log($"Converted {score} score to {currencyToAdd} upgrade currency.");
            }
        }
    }

    public bool CanUpgrade(UpgradeType type)
    {
        int currentLevel = currentUpgradeLevels[type];
        if (currentLevel >= maxUpgradeLevel)
            return false;

        int cost = upgradeCosts[type][currentLevel];
        return upgradeCurrency >= cost;
    }

    public bool Upgrade(UpgradeType type)
    {
        if (!CanUpgrade(type))
        {
            Debug.Log("Cannot upgrade " + type + ": insufficient currency or max level reached.");
            return false;
        }

        int currentLevel = currentUpgradeLevels[type];
        int cost = upgradeCosts[type][currentLevel];

        // Deduct currency
        upgradeCurrency -= cost;

        // Increase level
        currentUpgradeLevels[type] = currentLevel + 1;

        // Apply upgrade
        ApplyUpgrade(type);

        // Play sound
        if (AudioManager.Instance != null && upgradePurchaseClip != null)
        {
            AudioManager.Instance.PlaySFX(upgradePurchaseClip);
        }

        Debug.Log(type + " upgraded to level " + currentUpgradeLevels[type]);
        return true;
    }

    private void ApplyUpgrade(UpgradeType type)
    {
        int level = currentUpgradeLevels[type];
        if (level == 0) return; // no upgrade applied

        float value = upgradeValues[type][level - 1];

        switch (type)
        {
            case UpgradeType.ThrowPower:
                if (playerThrow != null)
                {
                    playerThrow.throwForce = 50f + value; // base 50 + upgrade
                }
                break;

            case UpgradeType.TrashBinCapacity:
                if (trashBin != null)
                {
                    trashBin.maxTrashCapacity = 10 + (int)value; // base 10 + upgrade
                }
                break;

            case UpgradeType.TrashBinHealth:
                if (trashBin != null)
                {
                    trashBin.maxHealth = 100 + (int)value; // base 100 + upgrade
                    trashBin.TakeDamage(0); // update UI
                }
                break;

            case UpgradeType.Damage:
                if (enemyAI != null)
                {
                    enemyAI.attackDamage = 10 + (int)value; // base 10 + upgrade
                }
                break;

            case UpgradeType.CompressionSpeed:
                if (trashCollection != null)
                {
                    trashCollection.compressionDelay = value; // lower is faster
                }
                break;

            case UpgradeType.CollectionRadius:
                if (trashCollection != null)
                {
                    trashCollection.collectionRadius = value;
                }
                break;

            case UpgradeType.PortalSpawnRate:
                if (trashSpawner != null)
                {
                    trashSpawner.spawnInterval = value; // lower is faster spawn
                }
                break;

            case UpgradeType.PortalSpawnRadius:
                if (trashSpawner != null)
                {
                    trashSpawner.spawnRadius = value;
                }
                break;

            case UpgradeType.PortalSpawnCount:
                // For spawn count, we need to extend TrashSpawner to support multiple spawns per interval
                // This will be handled separately
                break;
        }
    }

    private void ApplyAllUpgrades()
    {
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            ApplyUpgrade(type);
        }
    }

    public int GetConvertedCurrency()
    {
        return upgradeCurrency;
    }
}
