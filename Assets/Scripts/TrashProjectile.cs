using UnityEngine;

public class TrashProjectile : MonoBehaviour
{
    public int damage = 1;
    public int scorePerKill = 10;
    public float destroyHeight = -10f; // Height below which trash is destroyed

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    void Update()
    {
        if (transform.position.y < destroyHeight)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Optionally, apply damage to enemy here if enemy has health script
            // EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            // if (enemyHealth != null)
            // {
            //     enemyHealth.TakeDamage(damage);
            // }

            //ScoreManager.Instance.AddScore(scorePerKill);

            Destroy(gameObject);
        }
        else
        {
            // Destroy projectile immediately on any collision to prevent bouncing
            Destroy(gameObject);
        }
    }
}
