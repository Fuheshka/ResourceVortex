using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthBarSlider; // —сылка на полоску HP
    public float invincibilityTime = 1f; // ¬рем€ неу€звимости после удара
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;

        // јвтопоиск Slider, если не назначен
        if (healthBarSlider == null)
        {
            healthBarSlider = GameObject.Find("PlayerHealthBar").GetComponent<Slider>();
        }

        UpdateHealthBar();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage(10); // ”рон от врага
            StartCoroutine(InvincibilityFrame()); // јктивируем неу€звимость
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = (float)currentHealth / maxHealth;
        }
    }

    System.Collections.IEnumerator InvincibilityFrame()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("»грок умер!");
        // ƒополнительные действи€: перезагрузка сцены, анимаци€ смерти и т.д.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}