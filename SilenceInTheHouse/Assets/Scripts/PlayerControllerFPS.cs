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
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (playerCamera == null)
        {
            Debug.LogError("PlayerControllerFPS: playerCamera не назначена! Перетащи Main Camera в это поле.");
        }
    }

    void Update()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerControllerFPS: playerCamera = null, движение отключено!");
            return;
        }

        // Определяем скорость (Shift = бег)
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = sprintSpeed;
        else
            currentSpeed = moveSpeed;

        // Получаем ввод с клавиатуры
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Движение относительно камеры
        Vector3 moveDirection = (playerCamera.transform.forward * z) + (playerCamera.transform.right * x);
        moveDirection.y = 0;
        moveDirection.Normalize();

        // Применяем скорость (velocity, а не linearVelocity!)
        rb.velocity = new Vector3(
            moveDirection.x * currentSpeed,
            rb.velocity.y,  // сохраняем вертикальную скорость (гравитацию)
            moveDirection.z * currentSpeed
        );
    }
}