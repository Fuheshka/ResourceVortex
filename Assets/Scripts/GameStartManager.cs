using System.Collections;
using UnityEngine;
using TMPro;

public class GameStartManager : MonoBehaviour
{
    public TextMeshProUGUI signText; // Reference to the TextMeshProUGUI component on the sign UI
    public GameObject signUI; // The sign UI GameObject to enable/disable
    public Transform trashBinTransform; // Position near the trash bin to place the sign
    public float signYOffset = 2f; // Vertical offset for the sign position

    // The introductory text split into parts for switching
    private string[] introTexts = new string[]
    {
        "Hello, we are glad you joined us at work. To advance the text, press E while looking at this panel.",
        "Yes, we understand the path is not easy, but the salary is worth it.",
        "You need to collect trash from these portals (E), compress it (F), and throw it into the trash bin (LMB).",
        "Sometimes it gets clogged and needs to be cleaned.",
        "Also, Krogs live in these parts, they don't like it when trash is touched.",
        "We don't know why, but we can't pollute the forest, right?",
        "Fight them off with compressed trash (LMB), but don't forget to throw it away.",
        "See you soon!"
    };

    private int currentTextIndex = 0;

    public TrashSpawner trashSpawner;
    public WaveManager waveManager;

    void Start()
    {
        // Position the sign near the trash bin
        if (signUI != null && trashBinTransform != null)
        {
            signUI.transform.position = trashBinTransform.position + Vector3.up * signYOffset;
            signUI.SetActive(true);
        }

        // Show the first intro text
        if (signText != null)
        {
            signText.text = introTexts[0];
        }

        // Disable trash spawning and waves at start
        if (trashSpawner != null)
        {
            trashSpawner.enabled = false;
        }
        if (waveManager != null)
        {
            waveManager.enabled = false;
        }
    }

    public Camera playerCamera; // Reference to the player's camera
    public float maxLookDistance = 3f; // Max distance to detect looking at the sign
    public LayerMask signLayerMask; // Layer mask for the sign UI collider

    void Update()
    {
        // Check if player is looking at the sign panel
        bool isLookingAtSign = false;
        if (playerCamera != null && signUI != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxLookDistance, signLayerMask))
            {
                if (hit.collider.gameObject == signUI || hit.collider.transform.IsChildOf(signUI.transform))
                {
                    isLookingAtSign = true;
                }
            }
        }

        // Allow advancing text only if player is looking at the sign and presses E
        if (isLookingAtSign && Input.GetKeyDown(KeyCode.E))
        {
            AdvanceIntroText();
        }
    }

    void AdvanceIntroText()
    {
        currentTextIndex++;
        if (currentTextIndex < introTexts.Length)
        {
            if (signText != null)
            {
                signText.text = introTexts[currentTextIndex];
            }
        }
        else
        {
            // Intro finished, hide sign and enable trash spawning and waves
            if (signUI != null)
            {
                signUI.SetActive(false);
            }
            if (trashSpawner != null)
            {
                trashSpawner.enabled = true;
            }
            if (waveManager != null)
            {
                waveManager.enabled = true;
                waveManager.StartWaveSequence();
            }
        }
    }
}
