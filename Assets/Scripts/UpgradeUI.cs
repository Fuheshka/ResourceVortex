using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    public UpgradeSystem upgradeSystem;

    [Header("Converted Currency")]
    public TMPro.TextMeshProUGUI convertedCurrencyText;

    [Header("Throw Power")]
    public TextMeshProUGUI throwPowerLevelText;
    public TextMeshProUGUI throwPowerCostText;
    public Button throwPowerUpgradeButton;

    [Header("Trash Bin Capacity")]
    public TextMeshProUGUI trashBinCapacityLevelText;
    public TextMeshProUGUI trashBinCapacityCostText;
    public Button trashBinCapacityUpgradeButton;

    [Header("Trash Bin Health")]
    public TextMeshProUGUI trashBinHealthLevelText;
    public TextMeshProUGUI trashBinHealthCostText;
    public Button trashBinHealthUpgradeButton;

    [Header("Damage")]
    public TextMeshProUGUI damageLevelText;
    public TextMeshProUGUI damageCostText;
    public Button damageUpgradeButton;

    [Header("Compression Speed")]
    public TextMeshProUGUI compressionSpeedLevelText;
    public TextMeshProUGUI compressionSpeedCostText;
    public Button compressionSpeedUpgradeButton;

    [Header("Collection Radius")]
    public TextMeshProUGUI collectionRadiusLevelText;
    public TextMeshProUGUI collectionRadiusCostText;
    public Button collectionRadiusUpgradeButton;

    [Header("Portal Spawn Rate")]
    public TextMeshProUGUI portalSpawnRateLevelText;
    public TextMeshProUGUI portalSpawnRateCostText;
    public Button portalSpawnRateUpgradeButton;

    [Header("Portal Spawn Radius")]
    public TextMeshProUGUI portalSpawnRadiusLevelText;
    public TextMeshProUGUI portalSpawnRadiusCostText;
    public Button portalSpawnRadiusUpgradeButton;

    [Header("Portal Spawn Count")]
    public TextMeshProUGUI portalSpawnCountLevelText;
    public TextMeshProUGUI portalSpawnCountCostText;
    public Button portalSpawnCountUpgradeButton;

    void Start()
    {
        UpdateUI();

        throwPowerUpgradeButton.onClick.AddListener(() => Upgrade(UpgradeSystem.UpgradeType.ThrowPower));
        trashBinCapacityUpgradeButton.onClick.AddListener(() => Upgrade(UpgradeSystem.UpgradeType.TrashBinCapacity));
        trashBinHealthUpgradeButton.onClick.AddListener(() => Upgrade(UpgradeSystem.UpgradeType.TrashBinHealth));
        damageUpgradeButton.onClick.AddListener(() => Upgrade(UpgradeSystem.UpgradeType.Damage));
        compressionSpeedUpgradeButton.onClick.AddListener(() => Upgrade(UpgradeSystem.UpgradeType.CompressionSpeed));
        collectionRadiusUpgradeButton.onClick.AddListener(() => Upgrade(UpgradeSystem.UpgradeType.CollectionRadius));
        portalSpawnRateUpgradeButton.onClick.AddListener(() => Upgrade(UpgradeSystem.UpgradeType.PortalSpawnRate));
        portalSpawnRadiusUpgradeButton.onClick.AddListener(() => Upgrade(UpgradeSystem.UpgradeType.PortalSpawnRadius));
        portalSpawnCountUpgradeButton.onClick.AddListener(() => Upgrade(UpgradeSystem.UpgradeType.PortalSpawnCount));
    }

    void Upgrade(UpgradeSystem.UpgradeType type)
    {
        if (upgradeSystem.Upgrade(type))
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.ThrowPower, throwPowerLevelText, throwPowerCostText, throwPowerUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.TrashBinCapacity, trashBinCapacityLevelText, trashBinCapacityCostText, trashBinCapacityUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.TrashBinHealth, trashBinHealthLevelText, trashBinHealthCostText, trashBinHealthUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.Damage, damageLevelText, damageCostText, damageUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.CompressionSpeed, compressionSpeedLevelText, compressionSpeedCostText, compressionSpeedUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.CollectionRadius, collectionRadiusLevelText, collectionRadiusCostText, collectionRadiusUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.PortalSpawnRate, portalSpawnRateLevelText, portalSpawnRateCostText, portalSpawnRateUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.PortalSpawnRadius, portalSpawnRadiusLevelText, portalSpawnRadiusCostText, portalSpawnRadiusUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.PortalSpawnCount, portalSpawnCountLevelText, portalSpawnCountCostText, portalSpawnCountUpgradeButton);

        UpdateConvertedCurrencyText();
    }

    void UpdateUpgradeUI(UpgradeSystem.UpgradeType type, TextMeshProUGUI levelText, TextMeshProUGUI costText, Button upgradeButton)
    {
        int level = upgradeSystem.currentUpgradeLevels[type];
        levelText.text = $"Level: {level}";

        if (level >= upgradeSystem.maxUpgradeLevel)
        {
            costText.text = "MAX";
            upgradeButton.interactable = false;
        }
        else
        {
            int cost = upgradeSystem.upgradeCosts[type][level];
            costText.text = $"Cost: {cost}";
            upgradeButton.interactable = upgradeSystem.CanUpgrade(type);
        }
    }

    public void UpdateConvertedCurrencyText()
    {
        if (convertedCurrencyText != null && upgradeSystem != null)
        {
            int currency = upgradeSystem.GetConvertedCurrency();
            Debug.Log($"UpdateConvertedCurrencyText called. Currency: {currency}");
            convertedCurrencyText.text = $"Currency: {currency}";
        }
        else
        {
            Debug.LogWarning("ConvertedCurrencyText or UpgradeSystem is null in UpdateConvertedCurrencyText.");
        }
    }
}
