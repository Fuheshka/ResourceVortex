using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Цель (игрок)
    public string targetTag = "Player"; // Тег цели, если target не задан

    [Header("Movement Settings")]
    public float moveSpeed = 3f; // Скорость ходьбы
    public float stoppingDistance = 1f; // Дистанция остановки
    public float rotationSpeed = 5f; // Скорость поворота
    public float groundCheckDistance = 0.2f; // Проверка земли под ногами

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Чтобы враг не падал от столкновений

        // Автопоиск цели, если не задана
        if (target == null)
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
            if (targetObj != null) target = targetObj.transform;
        }
    }

    void Update()
    {
        // Проверяем, стоит ли враг на земле
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance
        );

        // Поворот к цели (только по оси Y, чтобы не наклонялся)
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0; // Игнорируем разницу по высоте

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    void FixedUpdate()
    {
        if (isGrounded && target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            // Движение к цели (только по XZ, без полётов)
            if (distance > stoppingDistance)
            {
                Vector3 moveDirection = (target.position - transform.position).normalized;
                moveDirection.y = 0; // Обнуляем вертикальное движение
                rb.linearVelocity = new Vector3(
                    moveDirection.x * moveSpeed,
                    rb.linearVelocity.y, // Сохраняем гравитацию
                    moveDirection.z * moveSpeed
                );
            }
            else
            {
                // Остановка
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
    }
}
