using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject shieldObject;
    public float shieldDuration = 5f;
    public float shieldCooldown = 10f;

    private bool isShieldActive = false;
    private float shieldTimer = 0f;
    private float cooldownTimer = 0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire2") && cooldownTimer <= 0f && !isShieldActive)
        {
            ActivateShield();
        }

        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                DeactivateShield();
                cooldownTimer = shieldCooldown;
            }
        }
        else
        {
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }
        }
    }

    void ActivateShield()
    {
        isShieldActive = true;
        shieldTimer = shieldDuration;
        if (shieldObject != null)
        {
            shieldObject.SetActive(true);
        }
    }

    void DeactivateShield()
    {
        isShieldActive = false;
        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
        }
    }

    public bool IsShieldActive()
    {
        return isShieldActive;
    }
}
