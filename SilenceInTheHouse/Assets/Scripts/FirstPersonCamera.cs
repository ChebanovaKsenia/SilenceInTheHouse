using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Настройки мыши")]
    public float mouseSensitivity = 2.0f;

    [Header("Ссылки")]
    public Transform playerBody;

    [Header("Настройки камеры")]
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;

    private float xRotation = 0f;
    private float mouseX;
    private float mouseY;
    private float pendingMouseX;

    private Rigidbody playerRigidbody;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerBody == null)
        {
            Debug.LogError(
                "FirstPersonCamera: playerBody не назначен! " +
                "Перетащи объект Player в это поле."
            );
            enabled = false;
            return;
        }

        playerRigidbody = playerBody.GetComponent<Rigidbody>();

        if (playerRigidbody != null)
        {
            playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void Update()
    {
        // Считываем движение мыши один раз за кадр.
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // Накапливаем горизонтальный ввод для физического обновления.
        pendingMouseX += mouseX;

        // Вертикальный поворот камеры.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(
            xRotation,
            minVerticalAngle,
            maxVerticalAngle
        );
    }

    void FixedUpdate()
    {
        if (playerBody == null)
            return;

        float rotationAmount = pendingMouseX;
        pendingMouseX = 0f;

        if (Mathf.Abs(rotationAmount) < 0.0001f)
            return;

        // Если на игроке есть Rigidbody, вращаем через физику.
        if (playerRigidbody != null)
        {
            Quaternion targetRotation =
                playerRigidbody.rotation *
                Quaternion.Euler(0f, rotationAmount, 0f);

            playerRigidbody.MoveRotation(targetRotation);
        }
        else
        {
            playerBody.Rotate(
                0f,
                rotationAmount,
                0f,
                Space.World
            );
        }
    }

    void LateUpdate()
    {
        // Обновляем наклон камеры после движения игрока.
        transform.localRotation = Quaternion.Euler(
            xRotation,
            0f,
            0f
        );
    }

    void OnDisable()
    {
        pendingMouseX = 0f;
    }
}