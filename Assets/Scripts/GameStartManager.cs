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
        "You need to collect trash from the portals above you (press E), compress it (press F), and throw it into the trash bin (press LMB).",
        "Sometimes the bin gets clogged and needs cleaning.",
        "Beware of the Krogs that inhabit this area; they don't like their trash disturbed.",
        "We don't know why, but we can't pollute the forest, right?",
        "Fight them off with compressed trash (press LMB), but don't forget to throw it away.",
        "Good luck, and stay safe!"
    };

    private int currentTextIndex = 0;

    public TrashSpawner trashSpawner;
    public WaveManager waveManager;

    // New fields for end game panel
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameText;
    [System.NonSerialized]
    private string[] endGameTexts = new string[]
    {
        "Well done, you have successfully defended the forest from trash and the invading Krogs.",
        "Your efforts have kept the environment clean and safe for all its inhabitants.",
        "The trash portals have been sealed, and peace has returned to the land. For today...",
        "Thank you for your dedication and bravery.",
        "Press E to restart your journey."
    };
    private int endGameTextIndex = 0;
    private bool isEndGameActive = false;

    // New fields for Easter panels
    public GameObject easterPanel1;
    public TextMeshProUGUI easterPanel1Text;
    public GameObject easterPanel2;
    public TextMeshProUGUI easterPanel2Text;

    private string[] easterPanel1Texts = new string[]
    {
        "Congratulations, you found the easter egg. Reach the top and get the ability."
    };

    private string[] easterPanel2Texts = new string[]
    {
        "Well done, you made it. Press E and get the reward."
    };

    private bool isEasterRewardGiven = false;

    public Camera playerCamera; // Reference to the player's camera
    public float maxLookDistance = 3f; // Max distance to detect looking at the sign
    public LayerMask signLayerMask; // Layer mask for the sign UI collider

    public FirstPersonController firstPersonController; // Reference to player controller

    void Start()
    {
        // Ensure end game panel is disabled at start
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        // Easter panels are not disabled at start to keep them always available
        // if (easterPanel1 != null)
        // {
        //     easterPanel1.SetActive(false);
        // }
        // if (easterPanel2 != null)
        // {
        //     easterPanel2.SetActive(false);
        // }

        // Set Easter panel texts immediately to ensure they display
        if (easterPanel1 != null && easterPanel1Text != null)
        {
            easterPanel1Text.text = easterPanel1Texts[0];
        }
        if (easterPanel2 != null && easterPanel2Text != null)
        {
            easterPanel2Text.text = easterPanel2Texts[0];
        }

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
            waveManager.AllWavesCompleted += OnAllWavesCompleted;
        }
    }

    void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.AllWavesCompleted -= OnAllWavesCompleted;
        }
    }

    void Update()
    {
        if (isEndGameActive)
        {
            // Allow advancing end game text by pressing E
            if (Input.GetKeyDown(KeyCode.E))
            {
                AdvanceEndGameText();
            }
            return;
        }

        // Remove check to allow reward always available
        // if (isEasterRewardGiven)
        // {
        //     return; // No further input after reward given
        // }

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

        // Check if player is looking at EasterPanel2 and presses E to get reward
        bool isLookingAtEasterPanel2 = false;
        if (playerCamera != null && easterPanel2 != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxLookDistance, signLayerMask))
            {
                if (hit.collider.gameObject == easterPanel2 || hit.collider.transform.IsChildOf(easterPanel2.transform))
                {
                    isLookingAtEasterPanel2 = true;
                }
            }
        }

        if (isLookingAtEasterPanel2 && Input.GetKeyDown(KeyCode.E))
        {
            GiveEasterReward();
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

            // Show EasterPanel1 with message
            if (easterPanel1 != null && easterPanel1Text != null)
            {
                easterPanel1.SetActive(true);
                easterPanel1Text.text = easterPanel1Texts[0];
            }
        }
    }

    void OnAllWavesCompleted()
    {
        // Disable trash spawning
        if (trashSpawner != null)
        {
            trashSpawner.enabled = false;
        }

        // Destroy or disable all portals with tags "enemyspawnportal" and "trashspawnportal"
        GameObject[] enemyPortals = GameObject.FindGameObjectsWithTag("EnemySpawnPortal");
        foreach (GameObject portal in enemyPortals)
        {
            portal.SetActive(false);
        }

        GameObject[] trashPortals = GameObject.FindGameObjectsWithTag("TrashSpawnPortal");
        foreach (GameObject portal in trashPortals)
        {
            portal.SetActive(false);
        }

        // Easter panels remain active to keep them always available
        // if (easterPanel1 != null)
        // {
        //     easterPanel1.SetActive(false);
        // }
        if (easterPanel2 != null && easterPanel2Text != null)
        {
            easterPanel2.SetActive(true);
            easterPanel2Text.text = easterPanel2Texts[0];
        }

        // Show end game panel and start end game text sequence
        if (endGamePanel != null && endGameText != null)
        {
            endGamePanel.SetActive(true);
            endGameTextIndex = 0;
            endGameText.text = endGameTexts[endGameTextIndex];
            isEndGameActive = true;
        }
    }

    void AdvanceEndGameText()
    {
        endGameTextIndex++;
        if (endGameTextIndex < endGameTexts.Length)
        {
            endGameText.text = endGameTexts[endGameTextIndex];
        }
        else
        {
            // End game text finished, hide panel and restart level
            if (endGamePanel != null)
            {
                endGamePanel.SetActive(false);
            }
            isEndGameActive = false;

            // Restart the current scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    void GiveEasterReward()
    {
        if (isEasterRewardGiven) return;

        if (firstPersonController != null)
        {
            firstPersonController.jumpPower *= 3f;
            firstPersonController.walkSpeed *= 3f;
            firstPersonController.sprintSpeed *= 3f;
        }

        if (easterPanel2Text != null)
        {
            easterPanel2Text.text = "Speed x3, Jump x3";
        }

        isEasterRewardGiven = true;

        // Do not disable EasterPanel2 to keep it always available
        // if (easterPanel2 != null)
        // {
        //     easterPanel2.SetActive(false);
        // }
    }
}
