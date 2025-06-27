using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public DeathScreenManager deathScreenManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Optionally, call a player death method if exists
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Die();
            }

            if (deathScreenManager != null)
            {
                deathScreenManager.ShowDeathScreen();
            }
            else
            {
                Debug.LogWarning("DeathScreenManager reference is not set in DeathZone.");
            }
        }
    }
}
