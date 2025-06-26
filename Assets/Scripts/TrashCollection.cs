using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrashCollection : MonoBehaviour
{
    public LayerMask collectableLayer; // Layer for collectable trash
    public KeyCode collectKey = KeyCode.E; // Key for collecting trash
    public KeyCode compressKey = KeyCode.F; // Key for compressing trash
    public float collectionRadius = 3f; // Radius for collecting trash
    public int bulletsPerCompression = 1; // Bullets created per compression
    public int trashPerBullet = 5; // Trash needed per bullet
    public TextMeshProUGUI counterText; // UI text for trash count
    public TextMeshProUGUI collectPromptText; // UI text for collect prompt
    public PlayerThrow playerThrow; // Reference to PlayerThrow script
    public float compressionDelay = 0.5f; // Delay between compressions

    // New variables for trash bin clearing
    public float clearTrashRadius = 3f; // Radius to detect trash bin for clearing
    public float holdClearDuration = 2f; // Time to hold key to clear trash
    public TextMeshProUGUI clearPromptText; // UI text for clear trash prompt
    public Image clearProgressImage; // UI Image for circular progress indicator
    public Transform playerCamera; // Reference to player's camera transform

    private int trashCount = 0; // Current trash count
    public int bulletCount = 0; // Current bullet count
    private bool isCompressing = false; // Is compressing trash
    private float clearKeyHoldTime = 0f; // Time the clear key has been held

    void Start()
    {
        if (counterText != null)
        {
            UpdateCounterText();
        }

        if (collectPromptText != null)
        {
            collectPromptText.gameObject.SetActive(false);
        }

        if (clearPromptText != null)
        {
            clearPromptText.gameObject.SetActive(false);
        }

        if (clearProgressImage != null)
        {
            clearProgressImage.fillAmount = 0f;
            clearProgressImage.gameObject.SetActive(false);
        }

        if (playerThrow == null)
        {
            playerThrow = GetComponent<PlayerThrow>();
            if (playerThrow == null)
            {
                Debug.LogWarning("PlayerThrow script not found on this GameObject.");
            }
        }
    }

    void Update()
    {
        // Update collect prompt UI
        UpdateCollectPrompt();

        // Update clear trash bin prompt UI
        UpdateClearPrompt();

        // Collect trash on key press
        if (Input.GetKeyDown(collectKey))
        {
            CollectTrashInRadius();
        }

        // Compress trash on key press
        if (Input.GetKeyDown(compressKey) && !isCompressing)
        {
            StartCoroutine(CompressAllTrash());
        }

        // Handle holding key to clear trash bin
        HandleClearTrashInput();

        // Debug: Draw a debug sphere at player position for clearTrashRadius
        Debug.DrawRay(transform.position, Vector3.up * 0.1f, Color.red);
        UnityEngine.Debug.DrawLine(transform.position + Vector3.forward * clearTrashRadius, transform.position - Vector3.forward * clearTrashRadius, Color.blue);
        UnityEngine.Debug.DrawLine(transform.position + Vector3.right * clearTrashRadius, transform.position - Vector3.right * clearTrashRadius, Color.blue);
        UnityEngine.Debug.DrawLine(transform.position + Vector3.up * clearTrashRadius, transform.position - Vector3.up * clearTrashRadius, Color.blue);
    }

    void UpdateCollectPrompt()
    {
        // ���������, ���� �� ���������� ������� � �������
        Collider[] colliders = Physics.OverlapSphere(transform.position, collectionRadius, collectableLayer);
        bool canCollect = false;

        foreach (Collider col in colliders)
        {
            if (collectableLayer.value == (collectableLayer.value | (1 << col.gameObject.layer)))
            {
                canCollect = true;
                break;
            }
        }

        // ���������� ��� �������� ���������
        if (collectPromptText != null)
        {
            collectPromptText.gameObject.SetActive(canCollect);
            if (canCollect)
            {
                collectPromptText.text = $"Press {collectKey} to collect";
            }
        }
    }

    void CollectTrashInRadius()
    {
        // ������� ��� ���������� � �������
        Collider[] colliders = Physics.OverlapSphere(transform.position, collectionRadius, collectableLayer);

        foreach (Collider col in colliders)
        {
            // ��������� ����
            if (collectableLayer.value == (collectableLayer.value | (1 << col.gameObject.layer)))
            {
                CollectTrash(col.gameObject);
            }
        }
    }

    void CollectTrash(GameObject trash)
    {
        trashCount++;
        UpdateCounterText();
        Destroy(trash); // ������� ��������� ������
    }

    IEnumerator CompressAllTrash()
    {
        isCompressing = true; // ��������� ��������� ������
        int totalBulletsCreated = 0;

        // ���� ������� ������ ��� ������
        while (trashCount >= trashPerBullet)
        {
            trashCount -= trashPerBullet;
            bulletCount += bulletsPerCompression;
            totalBulletsCreated += bulletsPerCompression;

            // ��������� ������ � PlayerThrow
            if (playerThrow != null && playerThrow.trashPrefab != null)
            {
                playerThrow.trashPrefab.tag = "bullet";
                playerThrow.trashPrefab.name = "Bullet";
            }

            UpdateCounterText();

            // Play trash recycle sound
            if (AudioManager.Instance != null && AudioManager.Instance.trashRecycleClips.Length > 0)
            {
                int index = Random.Range(0, AudioManager.Instance.trashRecycleClips.Length);
                AudioManager.Instance.PlayRecycleSFX(AudioManager.Instance.trashRecycleClips[index]);
            }

            yield return new WaitForSeconds(compressionDelay); // �������� ����� ��������
        }
        isCompressing = false; // ��������� ����� ������
    }

    public void UpdateCounterText()
    {
        if (counterText != null)
        {
            counterText.text = $"Trash: {trashCount} | Bullets: {bulletCount}";
        }
    }

    // ������������ ������� � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, clearTrashRadius);
    }

    void UpdateClearPrompt()
    {
        TrashBin nearbyBin = GetNearbyTrashBin();
        bool canClear = nearbyBin != null;

        if (clearPromptText != null)
        {
            clearPromptText.gameObject.SetActive(canClear);
            if (canClear)
            {
                clearPromptText.text = $"Hold {collectKey} to clear trash bin";
            }
        }
    }

    void HandleClearTrashInput()
    {
        TrashBin nearbyBin = GetNearbyTrashBin();
        if (nearbyBin == null)
        {
            clearKeyHoldTime = 0f;
            if (clearProgressImage != null)
            {
                clearProgressImage.fillAmount = 0f;
                clearProgressImage.gameObject.SetActive(false);
            }
            return;
        }

        // New check: only allow clearing if player is looking at the trash bin
        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, clearTrashRadius))
            {
                if (hit.collider != nearbyBin.GetComponent<Collider>())
                {
                    clearKeyHoldTime = 0f;
                    if (clearProgressImage != null)
                    {
                        clearProgressImage.fillAmount = 0f;
                        clearProgressImage.gameObject.SetActive(false);
                    }
                    return;
                }
            }
            else
            {
                clearKeyHoldTime = 0f;
                if (clearProgressImage != null)
                {
                    clearProgressImage.fillAmount = 0f;
                    clearProgressImage.gameObject.SetActive(false);
                }
                return;
            }
        }

        if (Input.GetKey(collectKey))
        {
            if (clearProgressImage != null && !clearProgressImage.gameObject.activeSelf)
            {
                clearProgressImage.gameObject.SetActive(true);
            }

            clearKeyHoldTime += Time.deltaTime;

            if (clearProgressImage != null)
            {
                clearProgressImage.fillAmount = Mathf.Clamp01(clearKeyHoldTime / holdClearDuration);
            }

            if (clearKeyHoldTime >= holdClearDuration)
            {
                nearbyBin.ClearTrash();
                clearKeyHoldTime = 0f;
                if (clearProgressImage != null)
                {
                    clearProgressImage.fillAmount = 0f;
                    clearProgressImage.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            clearKeyHoldTime = 0f;
            if (clearProgressImage != null)
            {
                clearProgressImage.fillAmount = 0f;
                clearProgressImage.gameObject.SetActive(false);
            }
        }
    }

    public LayerMask trashBinLayerMask; // Layer mask for trash bin detection

TrashBin GetNearbyTrashBin()
{
    Collider[] colliders = Physics.OverlapSphere(transform.position, clearTrashRadius, trashBinLayerMask);
    foreach (Collider col in colliders)
    {
        if (col.gameObject == this.gameObject) // Ignore player's own collider
            continue;

        TrashBin bin = col.GetComponent<TrashBin>();
        if (bin == null && col.transform.parent != null)
        {
            bin = col.transform.parent.GetComponent<TrashBin>();
        }
        if (bin == null)
        {
            bin = col.GetComponentInChildren<TrashBin>();
        }
        if (bin != null)
        {
            return bin;
        }
    }
    return null;
}
}
