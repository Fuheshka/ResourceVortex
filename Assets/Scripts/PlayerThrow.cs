using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    public Transform throwOrigin;
    public float throwForce = 10f;
    public GameObject trashPrefab;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ThrowTrash();
        }
    }

    void ThrowTrash()
    {
        if (trashPrefab == null) return;

        Debug.Log("ThrowOrigin position: " + throwOrigin.position + ", forward: " + throwOrigin.forward);
        Debug.Log("Player forward: " + transform.forward);

        GameObject thrownTrash = Instantiate(trashPrefab, throwOrigin.position, Quaternion.identity);
        Rigidbody rb = thrownTrash.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (rb.isKinematic)
            {
                rb.isKinematic = false;
            }
            // Use player's forward direction for throwing
            rb.linearVelocity = transform.forward * throwForce;
            Debug.Log("Throwing trash with player forward direction: " + transform.forward + " and force: " + throwForce);
        }
    }
}
