using UnityEngine;
using UnityEngine.UI;

public class TrashBin : MonoBehaviour
{
    [Header("Scoring")]
    public int pointsPerTrash = 10; // Points awarded per compressed trash
    public string compressedTrashTag = "bullet"; // Tag for compressed trash projectiles

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Trash Bin Settings")]
    public int maxTrashCapacity = 10; // Maximum trash the bin can hold
    private int currentTrashAmount = 0;

    private Collider binCollider;

    public Slider healthBarSlider;
    public Slider trashFillSlider; // Slider to show trash fill level

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
        currentTrashAmount = 0;
        UpdateTrashFillUI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(compressedTrashTag))
        {
            if (currentTrashAmount < maxTrashCapacity)
            {
                currentTrashAmount++;
                Debug.Log("Trash deposited. Current trash amount: " + currentTrashAmount);
                UpdateTrashFillUI();

                // Award points
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(pointsPerTrash);
                }

                // Destroy the compressed trash projectile
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log("Trash bin is full. Cannot deposit more trash.");
            }
        }
    }

    // To detect compressed trash entering the bin, use OnCollisionEnter since collider is not a trigger
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(compressedTrashTag))
        {
            if (currentTrashAmount < maxTrashCapacity)
            {
                currentTrashAmount++;
                Debug.Log("Trash deposited via collision. Current trash amount: " + currentTrashAmount);
                UpdateTrashFillUI();

                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(pointsPerTrash);
                }
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.Log("Trash bin is full. Cannot deposit more trash.");
            }
        }
    }

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

public void ClearTrash()
{
    Debug.Log("ClearTrash() called on TrashBin.");
    Debug.Log("Trash bin cleared. Trash amount reset from " + currentTrashAmount + " to 0.");
    currentTrashAmount = 0;
    UpdateTrashFillUI();

    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.PlayTrashBinClearSFX();
    }
}

    void UpdateTrashFillUI()
    {
        if (trashFillSlider != null)
        {
            trashFillSlider.value = (float)currentTrashAmount / maxTrashCapacity;
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
