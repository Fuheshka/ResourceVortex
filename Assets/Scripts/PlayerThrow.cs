using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    public Transform throwOrigin;
    public float throwForce = 50f; // Increased force for bullet-like speed
    public GameObject trashPrefab;
    public Camera playerCamera;
    public float maxThrowDistance = 100f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ThrowTrash();
        }
    }

    void ThrowTrash()
    {
        if (trashPrefab == null || throwOrigin == null || playerCamera == null)
        {
            Debug.LogWarning("TrashPrefab, ThrowOrigin, or PlayerCamera is not assigned.");
            return;
        }

        Vector3 throwDirection = playerCamera.transform.forward;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxThrowDistance))
        {
            throwDirection = (hit.point - throwOrigin.position).normalized;
        }

        GameObject thrownTrash = Instantiate(trashPrefab, throwOrigin.position, Quaternion.LookRotation(throwDirection));
        Rigidbody rb = thrownTrash.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("Thrown trash prefab does not have a Rigidbody component.");
            return;
        }

        rb.isKinematic = false;
        rb.linearVelocity = throwDirection * throwForce;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.useGravity = true;

        Debug.Log($"Thrown trash with velocity {rb.linearVelocity}");
    }
}
