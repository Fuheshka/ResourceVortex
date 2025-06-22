using UnityEngine;

public class TrashProjectile : MonoBehaviour
{
    public int damage = 1;
    public int scorePerKill = 10;

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
