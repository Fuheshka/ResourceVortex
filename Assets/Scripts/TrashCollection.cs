using TMPro;
using UnityEngine;
using System.Collections;

public class TrashCollection : MonoBehaviour
{
    public LayerMask collectableLayer; // Слой для собираемых предметов
    public KeyCode collectKey = KeyCode.E; // Клавиша для сбора
    public KeyCode compressKey = KeyCode.F; // Клавиша для сжатия
    public float collectionRadius = 3f; // Радиус области сбора
    public int bulletsPerCompression = 1; // Количество патронов за сжатие
    public int trashPerBullet = 5; // Сколько мусора нужно для патрона
    public TextMeshProUGUI counterText; // UI текст для счетчика
    public TextMeshProUGUI collectPromptText; // UI текст для подсказки
    public PlayerThrow playerThrow; // Ссылка на скрипт метания
    public float compressionDelay = 0.5f; // Задержка между сжатиями

    private int trashCount = 0; // Счетчик мусора
    public int bulletCount = 0; // Счетчик патронов
    private bool isCompressing = false; // Флаг для предотвращения множественных сжатий

    void Start()
    {
        // Инициализация текста счетчика
        if (counterText != null)
        {
            UpdateCounterText();
        }
        else
        {
            Debug.LogWarning("Counter Text is not assigned in TrashCollection script.");
        }

        // Инициализация текста подсказки
        if (collectPromptText != null)
        {
            collectPromptText.gameObject.SetActive(false); // Скрываем изначально
        }
        else
        {
            Debug.LogWarning("Collect Prompt Text is not assigned in TrashCollection script.");
        }

        // Проверка наличия скрипта PlayerThrow
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
        // Проверка наличия собираемых объектов
        UpdateCollectPrompt();

        // Проверка нажатия клавиши для сбора
        if (Input.GetKeyDown(collectKey))
        {
            CollectTrashInRadius();
        }

        // Проверка нажатия клавиши для сжатия
        if (Input.GetKeyDown(compressKey) && !isCompressing)
        {
            StartCoroutine(CompressAllTrash());
        }
    }

    void UpdateCollectPrompt()
    {
        // Проверяем, есть ли собираемые объекты в радиусе
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

        // Показываем или скрываем подсказку
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
        // Находим все коллайдеры в радиусе
        Collider[] colliders = Physics.OverlapSphere(transform.position, collectionRadius, collectableLayer);

        foreach (Collider col in colliders)
        {
            // Проверяем слой
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
        Destroy(trash); // Удаляем собранный объект
        Debug.Log($"Collected trash. Total: {trashCount}");
    }

    IEnumerator CompressAllTrash()
    {
        isCompressing = true; // Блокируем повторные сжатия
        int totalBulletsCreated = 0;

        // Пока хватает мусора для сжатия
        while (trashCount >= trashPerBullet)
        {
            trashCount -= trashPerBullet;
            bulletCount += bulletsPerCompression;
            totalBulletsCreated += bulletsPerCompression;

            // Обновляем префаб в PlayerThrow
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
            yield return new WaitForSeconds(compressionDelay); // Задержка между сжатиями
        }

        if (totalBulletsCreated > 0)
        {
            Debug.Log($"Compression complete: Created {totalBulletsCreated} bullet(s).");
        }
        else
        {
            Debug.Log($"Not enough trash to compress. Need {trashPerBullet}, have {trashCount}.");
        }

        isCompressing = false; // Разрешаем новые сжатия
    }

    public void UpdateCounterText()
    {
        if (counterText != null)
        {
            counterText.text = $"Trash: {trashCount} | Bullets: {bulletCount}";
        }
    }

    // Визуализация радиуса в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRadius);
    }
}