using UnityEngine;
using TMPro; // Для работы с TextMeshPro

public class DoorInteraction : MonoBehaviour
{
    [Header("Ссылки")]
    public Door door;             // Ссылка на скрипт двери
    public GameObject uiPrompt;   // Ссылка на UI объект с текстом
    public TextMeshProUGUI promptText; // Ссылка на сам компонент текста

    private bool playerInRange = false;

    void Update()
    {
        // Если игрок в зоне и нажал R
        if (playerInRange && Input.GetKeyDown(KeyCode.R))
        {
            door.ToggleDoor(); // Переключаем дверь
            UpdateUIText();    // Обновляем надпись
        }
    }

    // Срабатывает, когда игрок заходит в зону (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            uiPrompt.SetActive(true); // Показываем надпись
            UpdateUIText();
        }
    }

    // Срабатывает, когда игрок выходит из зоны
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            uiPrompt.SetActive(false); // Скрываем надпись
        }
    }

    // Меняем текст в зависимости от состояния двери
    private void UpdateUIText()
    {
        if (door.isOpen)
        {
            promptText.text = "R - close";
        }
        else
        {
            promptText.text = "R - open";
        }
    }
}