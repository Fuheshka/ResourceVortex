using UnityEngine;

public class TrashBin : MonoBehaviour
{
    [Header("Scoring")]
    public int pointsPerTrash = 10; // Points awarded per compressed trash
    public string compressedTrashTag = "bullet"; // Tag for compressed trash projectiles

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    private Collider binCollider;

    void Awake()
    {
        binCollider = GetComponent<Collider>();
        if (binCollider == null)
        {
            Debug.LogError("TrashBin requires a Collider component.");
        }
        else
        {
            // Ensure collider is not a trigger to block players and enemies
            binCollider.isTrigger = false;
        }

        currentHealth = maxHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(compressedTrashTag))
        {
            // Award points
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(pointsPerTrash);
            }

            // Destroy the compressed trash projectile
            Destroy(other.gameObject);

            Debug.Log("Compressed trash deposited. Points awarded: " + pointsPerTrash);
        }
    }

    // To detect compressed trash entering the bin, use OnCollisionEnter since collider is not a trigger
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(compressedTrashTag))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(pointsPerTrash);
            }
            Destroy(collision.gameObject);
            Debug.Log("Compressed trash deposited via collision. Points awarded: " + pointsPerTrash);
        }
    }

    public UnityEngine.UI.Slider healthBarSlider;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("TrashBin takes damage: " + damage + ", current health: " + currentHealth);

        if (healthBarSlider != null)
        {
            healthBarSlider.value = (float)currentHealth / maxHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("TrashBin destroyed");

        // Show death screen
        DeathScreenManager deathScreenManager = FindObjectOfType<DeathScreenManager>();
        if (deathScreenManager != null)
        {
            deathScreenManager.ShowDeathScreen();
        }
        else
        {
            Debug.LogWarning("DeathScreenManager not found in scene.");
        }

        Destroy(gameObject);
    }
}
