using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 1f;
    public float rotationSpeed = 5f;
    public float groundCheckDistance = 0.2f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    private Transform target;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentHealth = maxHealth;

        // Поиск цели (игрока)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;
    }

    void Update()
    {
        // Проверка земли
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // Поворот к цели
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        // Движение к цели
        if (isGrounded && target != null && Vector3.Distance(transform.position, target.position) > stoppingDistance)
        {
            Vector3 moveDirection = (target.position - transform.position).normalized;
            moveDirection.y = 0;
            rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    // Обработка столкновений с пулями
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            TrashProjectile projectile = collision.gameObject.GetComponent<TrashProjectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.damage);
                Destroy(collision.gameObject); // Уничтожаем пулю
            }
        }
    }

    // Получение урона
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " получил " + damage + " урона. Осталось HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Смерть врага
    void Die()
    {
        Debug.Log(gameObject.name + " умер!");
        Destroy(gameObject); // Или анимация смерти + отключение коллайдера
    }
}
