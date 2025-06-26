using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float maxInteractionDistance = 3f;
    public LayerMask interactableLayerMask;

    private Button currentLookedAtButton;

    void Update()
    {
        currentLookedAtButton = null;

        if (playerCamera == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxInteractionDistance, interactableLayerMask))
        {
            Button button = hit.collider.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                currentLookedAtButton = button;

                // Optionally, highlight the button or show UI feedback here
            }
        }

        if (currentLookedAtButton != null && Input.GetKeyDown(KeyCode.E))
        {
            currentLookedAtButton.onClick.Invoke();
        }
    }
}
