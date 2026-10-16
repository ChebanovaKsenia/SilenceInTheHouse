
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerControllerFPS : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;

    [Header("Камера")]
    public Camera playerCamera;

    private Rigidbody rb;
    private Vector2 moveInput;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Персонаж не должен заваливаться набок.
        rb.freezeRotation = true;

        // Делаем движение визуально плавнее.
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Коллизии проверяются надёжнее при быстром движении.
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError(
                "PlayerControllerFPS: не назначена камера игрока!"
            );
        }
    }

    void Update()
    {
        // Считываем нажатия в Update.
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : moveSpeed;
    }

    void FixedUpdate()
    {
        if (playerCamera == null)
            return;

        // Берём только горизонтальное направление взгляда.
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = transform.forward;

        forward.Normalize();

        // Горизонтальное направление вправо.
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        // Рассчитываем движение относительно камеры.
        Vector3 moveDirection =
            forward * moveInput.y +
            right * moveInput.x;

        // Не позволяем диагональному движению быть быстрее.
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        // Меняем только горизонтальную скорость.
        // Вертикальная скорость сохраняется для гравитации.
        Vector3 velocity = rb.velocity;

        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;

        rb.velocity = velocity;
    }
}
