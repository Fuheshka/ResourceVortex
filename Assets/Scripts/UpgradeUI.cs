using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    [Header("Player Health Restore")]
    public TextMeshProUGUI playerHealthRestoreLevelText;
    public TextMeshProUGUI playerHealthRestoreCostText;
    public Button playerHealthRestoreUpgradeButton;

    [Header("Trash Bin Health Restore")]
    public TextMeshProUGUI trashBinHealthRestoreLevelText;
    public TextMeshProUGUI trashBinHealthRestoreCostText;
    public Button trashBinHealthRestoreUpgradeButton;

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

    // Colors for button states
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color pressedColor = Color.red;
    public Color selectedColor = Color.green;

    public Camera playerCamera;
    public float maxRayDistance = 10f;
    public LayerMask buttonLayerMask;

    private Dictionary<Button, UpgradeSystem.UpgradeType> buttonUpgradeTypeMap;
    private Button hoveredButton;
    private Button selectedButton;

    void Start()
    {
        buttonUpgradeTypeMap = new Dictionary<Button, UpgradeSystem.UpgradeType>()
        {
            { throwPowerUpgradeButton, UpgradeSystem.UpgradeType.ThrowPower },
            { trashBinCapacityUpgradeButton, UpgradeSystem.UpgradeType.TrashBinCapacity },
            { trashBinHealthUpgradeButton, UpgradeSystem.UpgradeType.TrashBinHealth },
            { playerHealthRestoreUpgradeButton, UpgradeSystem.UpgradeType.PlayerHealthRestore },
            { trashBinHealthRestoreUpgradeButton, UpgradeSystem.UpgradeType.TrashBinHealthRestore },
            { damageUpgradeButton, UpgradeSystem.UpgradeType.Damage },
            { compressionSpeedUpgradeButton, UpgradeSystem.UpgradeType.CompressionSpeed },
            { collectionRadiusUpgradeButton, UpgradeSystem.UpgradeType.CollectionRadius },
            { portalSpawnRateUpgradeButton, UpgradeSystem.UpgradeType.PortalSpawnRate },
            { portalSpawnRadiusUpgradeButton, UpgradeSystem.UpgradeType.PortalSpawnRadius },
            { portalSpawnCountUpgradeButton, UpgradeSystem.UpgradeType.PortalSpawnCount }
        };

        UpdateUI();
    }

    void Update()
    {
        UpdateHoveredButton();

        if (hoveredButton != null)
        {
            if (hoveredButton != selectedButton)
            {
                SetButtonColor(hoveredButton, hoverColor);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OnButtonClicked(hoveredButton, buttonUpgradeTypeMap[hoveredButton]);
            }
        }
        else
        {
            ResetHoveredButtonColor();
        }
    }

    void UpdateHoveredButton()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerCamera is not assigned in UpgradeUI.");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, buttonLayerMask))
        {
            Button hitButton = hit.collider.GetComponent<Button>();
            if (hitButton != null && buttonUpgradeTypeMap.ContainsKey(hitButton))
            {
                if (hoveredButton != hitButton)
                {
                    ResetHoveredButtonColor();
                    hoveredButton = hitButton;
                }
                return;
            }
        }

        ResetHoveredButtonColor();
        hoveredButton = null;
    }

    void ResetHoveredButtonColor()
    {
        if (hoveredButton != null && hoveredButton != selectedButton)
        {
            SetButtonColor(hoveredButton, normalColor);
        }
    }

    void OnButtonClicked(Button btn, UpgradeSystem.UpgradeType type)
    {
        if (upgradeSystem.Upgrade(type))
        {
            UpdateUI();
            SetSelectedButton(btn);
        }
    }

    void SetSelectedButton(Button btn)
    {
        if (selectedButton != null)
        {
            SetButtonColor(selectedButton, normalColor);
        }

        selectedButton = btn;

        if (selectedButton != null)
        {
            SetButtonColor(selectedButton, selectedColor);
        }
    }

    void SetButtonColor(Button btn, Color color)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        cb.pressedColor = color;
        cb.selectedColor = color;
        btn.colors = cb;
    }

    void UpdateUI()
    {
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.ThrowPower, throwPowerLevelText, throwPowerCostText, throwPowerUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.TrashBinCapacity, trashBinCapacityLevelText, trashBinCapacityCostText, trashBinCapacityUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.TrashBinHealth, trashBinHealthLevelText, trashBinHealthCostText, trashBinHealthUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.PlayerHealthRestore, playerHealthRestoreLevelText, playerHealthRestoreCostText, playerHealthRestoreUpgradeButton);
        UpdateUpgradeUI(UpgradeSystem.UpgradeType.TrashBinHealthRestore, trashBinHealthRestoreLevelText, trashBinHealthRestoreCostText, trashBinHealthRestoreUpgradeButton);
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

        // For health restore upgrades, do not disable button or show MAX
        if ((type == UpgradeSystem.UpgradeType.PlayerHealthRestore || type == UpgradeSystem.UpgradeType.TrashBinHealthRestore))
        {
            int cost = 0;
            if (type == UpgradeSystem.UpgradeType.PlayerHealthRestore)
            {
                cost = 20 * (level + 1);
            }
            else if (type == UpgradeSystem.UpgradeType.TrashBinHealthRestore)
            {
                cost = 20 * (level + 1);
            }
            costText.text = $"Cost: {cost}";
            upgradeButton.interactable = upgradeSystem.CanUpgrade(type);
            SetButtonColor(upgradeButton, normalColor);
        }
        else if (level >= upgradeSystem.maxUpgradeLevel)
        {
            costText.text = "MAX";
            upgradeButton.interactable = false;
            SetButtonColor(upgradeButton, normalColor);
        }
        else
        {
            int cost = upgradeSystem.upgradeCosts[type][level];
            costText.text = $"Cost: {cost}";
            upgradeButton.interactable = upgradeSystem.CanUpgrade(type);
            SetButtonColor(upgradeButton, normalColor);
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
