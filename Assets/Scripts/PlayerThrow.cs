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

        GameObject thrownTrash = Instantiate(trashPrefab, throwOrigin.position, Quaternion.identity);
        Rigidbody rb = thrownTrash.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        }
    }
}
