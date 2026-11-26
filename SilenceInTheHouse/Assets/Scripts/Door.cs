using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Настройки двери")]
    public float openAngle = 90f;   // Угол открытия
    public float closeAngle = 0f;   // Угол закрытия
    public float smoothSpeed = 5f;  // Скорость плавного открытия

    private float targetAngle;
    public bool isOpen = false;

    void Start()
    {
        targetAngle = closeAngle;
    }

    void Update()
    {
        // Плавное вращение
        float currentAngle = transform.localEulerAngles.y;
        float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * smoothSpeed);
        transform.localEulerAngles = new Vector3(0, newAngle, 0);
    }

    // Метод для переключения состояния
    public void ToggleDoor()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void Open()
    {
        isOpen = true;
        targetAngle = openAngle;
    }

    private void Close()
    {
        isOpen = false;
        targetAngle = closeAngle;
    }
}