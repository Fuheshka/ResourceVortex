using TMPro;
using UnityEngine;
using System.Collections;

public class TrashCollection : MonoBehaviour
{
    public LayerMask collectableLayer; // ���� ��� ���������� ���������
    public KeyCode collectKey = KeyCode.E; // ������� ��� �����
    public KeyCode compressKey = KeyCode.F; // ������� ��� ������
    public float collectionRadius = 3f; // ������ ������� �����
    public int bulletsPerCompression = 1; // ���������� �������� �� ������
    public int trashPerBullet = 5; // ������� ������ ����� ��� �������
    public TextMeshProUGUI counterText; // UI ����� ��� ��������
    public TextMeshProUGUI collectPromptText; // UI ����� ��� ���������
    public PlayerThrow playerThrow; // ������ �� ������ �������
    public float compressionDelay = 0.5f; // �������� ����� ��������

    private int trashCount = 0; // ������� ������
    public int bulletCount = 0; // ������� ��������
    private bool isCompressing = false; // ���� ��� �������������� ������������� ������

    void Start()
    {
        // ������������� ������ ��������
        if (counterText != null)
        {
            UpdateCounterText();
        }
        else
        {
            Debug.LogWarning("Counter Text is not assigned in TrashCollection script.");
        }

        // ������������� ������ ���������
        if (collectPromptText != null)
        {
            collectPromptText.gameObject.SetActive(false); // �������� ����������
        }
        else
        {
            Debug.LogWarning("Collect Prompt Text is not assigned in TrashCollection script.");
        }

        // �������� ������� ������� PlayerThrow
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
        // �������� ������� ���������� ��������
        UpdateCollectPrompt();

        // �������� ������� ������� ��� �����
        if (Input.GetKeyDown(collectKey))
        {
            CollectTrashInRadius();
        }

        // �������� ������� ������� ��� ������
        if (Input.GetKeyDown(compressKey) && !isCompressing)
        {
            StartCoroutine(CompressAllTrash());
        }
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
        Debug.Log($"Collected trash. Total: {trashCount}");
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
                Debug.Log($"Compressed {trashPerBullet} trash into {bulletsPerCompression} bullet(s). Total bullets: {bulletCount}");
            }
            else
            {
                Debug.LogWarning("Cannot compress: PlayerThrow or trashPrefab not assigned.");
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

        if (totalBulletsCreated > 0)
        {
            Debug.Log($"Compression complete: Created {totalBulletsCreated} bullet(s).");
        }
        else
        {
            Debug.Log($"Not enough trash to compress. Need {trashPerBullet}, have {trashCount}.");
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
    }
}