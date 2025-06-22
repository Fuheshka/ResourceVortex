using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3; // Enemy dies after 3 hits
    private int currentHealth;
    public Slider healthBarSlider;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    [Header("Score Settings")]
    public int scoreValue = 10;

    private Transform target;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private float lastAttackTime;
    private bool isKnockedback = false;
    private float knockbackEndTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        // Freeze rotation on X and Z axes only, allow Y axis rotation for turning
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // Add drag to reduce sliding after knockback
        rb.linearDamping = 3f;
        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }

        if (healthBarSlider == null)
        {
            healthBarSlider = GetComponentInChildren<Slider>();
        }
        UpdateHealthBar();
    }

    void Update()
    {
        if (isKnockedback)
        {
            // Zero angular velocity continuously during knockback to prevent spinning
            rb.angularVelocity = Vector3.zero;

            if (Time.time >= knockbackEndTime)
            {
                isKnockedback = false;
                rb.isKinematic = false;
                // Restore Rigidbody constraints to freeze rotation on X and Z axes only
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
            // Do not skip movement while knocked back
        }

        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        // Implement attack logic here, e.g., reduce player health
        Debug.Log("Enemy attacks player for " + attackDamage + " damage.");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            TrashProjectile projectile = collision.gameObject.GetComponent<TrashProjectile>();
            if (projectile != null)
            {
                Debug.Log("Enemy hit by bullet with damage: " + projectile.damage);
                TakeDamage(projectile.damage);
                Vector3 knockbackDir = (transform.position - collision.transform.position).normalized;
                ApplyKnockback(knockbackDir, 2f, 0.5f);
                Destroy(collision.gameObject);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Enemy takes damage: " + damage + ", current health before: " + currentHealth);
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Debug.Log("Enemy died");
            Die();
        }
    }

    void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        if (isKnockedback) return;

        isKnockedback = true;
        knockbackEndTime = Time.time + duration;

        // Do not disable NavMeshAgent during knockback
        rb.isKinematic = false;
        // Freeze rotation on X and Z axes only, allow Y axis rotation for turning
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // Reduce knockback force to lessen sliding
        rb.AddForce(direction * force, ForceMode.Impulse);
        // Zero angular velocity to prevent spinning
        rb.angularVelocity = Vector3.zero;
    }

    void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        ScoreManager.Instance.AddScore(scoreValue);
        Destroy(gameObject);
    }
}
