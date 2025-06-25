using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public int pointsPerTrash = 10; // Points awarded per compressed trash
    public string compressedTrashTag = "bullet"; // Tag for compressed trash projectiles

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
}
