using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    public int comboCount = 0;
    public float comboResetTime = 3f;
    public float scoreMultiplier = 1f;
    public float multiplierIncrement = 0.5f;

    private float comboTimer = 0f;
    private bool isTakingDamage = false;

    void Update()
    {
        if (comboCount > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    public void EnemyKilled()
    {
        if (!isTakingDamage)
        {
            comboCount++;
            comboTimer = comboResetTime;
            scoreMultiplier = 1f + comboCount * multiplierIncrement;
            Debug.Log("Combo: " + comboCount + " Multiplier: " + scoreMultiplier);
        }
        else
        {
            ResetCombo();
        }
    }

    public void PlayerTookDamage()
    {
        isTakingDamage = true;
        ResetCombo();
        isTakingDamage = false;
    }

    void ResetCombo()
    {
        comboCount = 0;
        scoreMultiplier = 1f;
        comboTimer = 0f;
        Debug.Log("Combo reset");
    }
}
