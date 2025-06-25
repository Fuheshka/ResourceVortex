using UnityEngine;

public class HealthBarLookAtCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Main camera not found. Health bar will not rotate.");
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // Rotate to face the camera
            transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
        }
    }
}
