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

    public GameObject portalChildObject; // Reference to the portal child object (trash spawner portal)

    public AudioClip upgradePurchaseClip;

    public Dictionary<UpgradeType, int> currentUpgradeLevels = new Dictionary<UpgradeType, int>();

    public int upgradeCurrency = 0;
    public float scoreToCurrencyRate = 0.1f; // Example: 10 score = 1 currency

    public int maxUpgradeLevel = 5;

    // Base costs for health restore upgrades
    private int basePlayerHealthRestoreCost = 20;
    private int baseTrashBinHealthRestoreCost = 20;

    private float baseCollectionRadius; // Base collection radius to calculate decrease
    private Vector3 basePortalScale; // Base scale of the portal child object

    private float basePortalSpawnRadius; // Base spawn radius of trashSpawner

    public UpgradeUI upgradeUI; // Reference to UpgradeUI to refresh UI after initialization

    public enum UpgradeType
    {
        ThrowPower,
        TrashBinCapacity,
        TrashBinHealth,
        PlayerHealthRestore,  // New upgrade type for player health restoration
        TrashBinHealthRestore, // New upgrade type for trash bin health restoration
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
        // Remove upgrade values for health restore upgrades as they are fixed
        // upgradeValues[UpgradeType.PlayerHealthRestore] = new float[] { 10f, 20f, 30f, 40f, 50f };
        // upgradeValues[UpgradeType.TrashBinHealthRestore] = new float[] { 10f, 20f, 30f, 40f, 50f };
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
        // For health restore upgrades, start with base cost and increase with level (handled in CanUpgrade and Upgrade)
        upgradeCosts[UpgradeType.PlayerHealthRestore] = new int[] { 20 }; 
        upgradeCosts[UpgradeType.TrashBinHealthRestore] = new int[] { 20 };
        upgradeCosts[UpgradeType.Damage] = new int[] { 20, 40, 60, 80, 100 };
        upgradeCosts[UpgradeType.CompressionSpeed] = new int[] { 10, 20, 30, 40, 50 };
        upgradeCosts[UpgradeType.CollectionRadius] = new int[] { 10, 20, 30, 40, 50 };
        upgradeCosts[UpgradeType.PortalSpawnRate] = new int[] { 25, 50, 75, 100, 125 };
        upgradeCosts[UpgradeType.PortalSpawnRadius] = new int[] { 20, 40, 60, 80, 100 };
        upgradeCosts[UpgradeType.PortalSpawnCount] = new int[] { 30, 60, 90, 120, 150 };

        // Store base values for collection radius, portal scale, and portal spawn radius
        if (trashCollection != null)
        {
            baseCollectionRadius = trashCollection.collectionRadius;
        }
        else
        {
            baseCollectionRadius = 5f; // default fallback
        }

        if (portalChildObject != null)
        {
            basePortalScale = portalChildObject.transform.localScale;
        }
        else
        {
            basePortalScale = Vector3.one; // default fallback
        }

        if (trashSpawner != null)
        {
            basePortalSpawnRadius = trashSpawner.spawnRadius;
        }
        else
        {
            basePortalSpawnRadius = 10f; // default fallback
        }

        // Apply initial upgrades (level 0 means base values)
        ApplyAllUpgrades();

        // Call UpgradeUI to refresh UI after initialization
        if (upgradeUI != null)
        {
            upgradeUI.RefreshUI();
        }
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
                if (upgradeUI != null)
                {
                    upgradeUI.RefreshUI();
                }
            }
        }
    }

    public bool CanUpgrade(UpgradeType type)
    {
        int currentLevel = currentUpgradeLevels[type];
        // Remove max level limit for health restore upgrades
        if ((type != UpgradeType.PlayerHealthRestore && type != UpgradeType.TrashBinHealthRestore) && currentLevel >= maxUpgradeLevel)
            return false;

        int cost;
        if (type == UpgradeType.PlayerHealthRestore)
        {
            cost = basePlayerHealthRestoreCost * (currentLevel + 1);
        }
        else if (type == UpgradeType.TrashBinHealthRestore)
        {
            cost = baseTrashBinHealthRestoreCost * (currentLevel + 1);
        }
        else
        {
            cost = upgradeCosts[type][currentLevel];
        }
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
        int cost;
        if (type == UpgradeType.PlayerHealthRestore)
        {
            cost = basePlayerHealthRestoreCost * (currentLevel + 1);
        }
        else if (type == UpgradeType.TrashBinHealthRestore)
        {
            cost = baseTrashBinHealthRestoreCost * (currentLevel + 1);
        }
        else
        {
            cost = upgradeCosts[type][currentLevel];
        }

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

        float value = 0f;
        if (type != UpgradeType.PlayerHealthRestore && type != UpgradeType.TrashBinHealthRestore)
        {
            value = upgradeValues[type][level - 1];
        }

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

            case UpgradeType.PlayerHealthRestore:
                if (playerThrow != null && playerThrow.GetComponent<PlayerHealth>() != null)
                {
                    PlayerHealth playerHealth = playerThrow.GetComponent<PlayerHealth>();
                    int restoreAmount = (int)(playerHealth.maxHealth * 0.3f);
                    playerHealth.RestoreHealth(restoreAmount);
                }
                break;

            case UpgradeType.TrashBinHealthRestore:
                if (trashBin != null)
                {
                    int restoreAmount = (int)(trashBin.maxHealth * 0.3f);
                    trashBin.RestoreHealth(restoreAmount);
                }
                break;

case UpgradeType.Damage:
    if (playerThrow != null && playerThrow.trashPrefab != null)
    {
        TrashProjectile projectile = playerThrow.trashPrefab.GetComponent<TrashProjectile>();
        if (projectile != null)
        {
            projectile.damage = level;
            if (projectile.damage < 1)
            {
                projectile.damage = 1;
            }
        }
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
        // Increase collection radius by a fixed amount per level
        float increaseAmount = 0.5f * level; // example increase per level
        trashCollection.collectionRadius = baseCollectionRadius + increaseAmount;

        // Increase scale of portal child object proportionally
        if (portalChildObject != null)
        {
            float scaleIncreaseFactor = 0.1f * level; // example scale increase per level
            Vector3 newScale = basePortalScale * (1f + scaleIncreaseFactor);
            portalChildObject.transform.localScale = newScale;
        }
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
                    // Decrease spawn radius by 0.5 per upgrade level
                    float decreaseAmount = 0.5f * level;
                    trashSpawner.spawnRadius = Mathf.Max(0f, basePortalSpawnRadius - decreaseAmount);

                    // Decrease scale of portal child object by 0.5 per upgrade level on all axes
                    if (portalChildObject != null)
                    {
                        float scaleDecreaseAmount = 0.5f * level;
                        Vector3 newScale = basePortalScale - new Vector3(scaleDecreaseAmount, scaleDecreaseAmount, scaleDecreaseAmount);
                        newScale.x = Mathf.Max(0f, newScale.x);
                        newScale.y = Mathf.Max(0f, newScale.y);
                        newScale.z = Mathf.Max(0f, newScale.z);
                        portalChildObject.transform.localScale = newScale;
                    }
                }
                break;

            case UpgradeType.PortalSpawnCount:
            // For spawn count, we need to extend TrashSpawner to support multiple spawns per interval
            if (trashSpawner != null)
            {
                trashSpawner.spawnCount = (int)value;
            }
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
