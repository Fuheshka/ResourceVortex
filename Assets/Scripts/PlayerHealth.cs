using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthBarSlider; // ������ �� ������� HP
    public float invincibilityTime = 1f; // ����� ������������ ����� �����
    private bool isInvincible = false;

    [Header("Death Screen")]
    public DeathScreenManager deathScreenManager;

    void Start()
    {
        currentHealth = maxHealth;

        // ��������� Slider, ���� �� ��������
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
            TakeDamage(10); // ���� �� �����
            StartCoroutine(InvincibilityFrame()); // ���������� ������������
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
        Debug.Log("Player died!");
        if (deathScreenManager != null)
        {
            deathScreenManager.ShowDeathScreen();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
