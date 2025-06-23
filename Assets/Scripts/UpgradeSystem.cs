using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public PlayerThrow playerThrow;
    public float throwForceUpgradeAmount = 2f;
    public int maxUpgradeLevel = 5;

    private int currentUpgradeLevel = 0;

    public void UpgradeThrowPower()
    {
        if (currentUpgradeLevel < maxUpgradeLevel)
        {
            currentUpgradeLevel++;
            playerThrow.throwForce += throwForceUpgradeAmount;
            Debug.Log("Throw power upgraded to level " + currentUpgradeLevel);

            // Play upgrade purchase sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.upgradePurchaseClip);
            }
        }
        else
        {
            Debug.Log("Max upgrade level reached");
        }
    }
}
