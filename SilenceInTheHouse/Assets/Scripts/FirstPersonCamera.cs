using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Настройки мыши")]
    public float mouseSensitivity = 2.0f;

    [Header("Ссылки")]
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerBody == null)
        {
            Debug.LogError("FirstPersonCamera: playerBody не назначен! Перетащи объект Player в это поле.");
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Вращение камеры вверх-вниз
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Вращение тела игрока влево-вправо
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            Debug.LogWarning("FirstPersonCamera: playerBody = null!");
        }
    }
}