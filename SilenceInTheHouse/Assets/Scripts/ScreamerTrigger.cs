using UnityEngine;

public class ScreamerTrigger : MonoBehaviour
{
    [Header("Настройки скримера")]
    public GameObject screamerScene; // Весь объект Screamer_Scene
    public AudioClip screamerSound;
    public float screamerDuration = 4f;

    private Camera mainCamera;
    private Camera screamerCamera;
    private AudioSource audioSource;
    private bool isScreaming = false;

    void Start()
    {
        mainCamera = Camera.main; // Находим основную камеру
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Находим камеру внутри объекта скримера
        if (screamerScene != null)
        {
            screamerCamera = screamerScene.GetComponentInChildren<Camera>();
            screamerScene.SetActive(false); // Выключаем до момента скримера

            if (screamerCamera != null)
            {
                screamerCamera.enabled = false; // Выключаем камеру скримера
            }
        }
    }

    public void TriggerScreamer()
    {
        if (isScreaming) return;
        isScreaming = true;

        Debug.Log("СКРИМЕР ЗАПУЩЕН!");

        // Отключаем основную камеру
        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }

        // Включаем сцену скримера и его камеру
        if (screamerScene != null)
        {
            screamerScene.SetActive(true);
        }

        if (screamerCamera != null)
        {
            screamerCamera.enabled = true;
        }

        // Воспроизводим звук
        if (screamerSound != null)
        {
            audioSource.PlayOneShot(screamerSound);
        }

        // Возврат через N секунд
        Invoke(nameof(EndScreamer), screamerDuration);
    }

    void EndScreamer()
    {
        Debug.Log("Скример окончен!");

        // Выключаем камеру скримера
        if (screamerCamera != null)
        {
            screamerCamera.enabled = false;
        }

        // Выключаем сцену скримера
        if (screamerScene != null)
        {
            screamerScene.SetActive(false);
        }

        // Включаем основную камеру
        if (mainCamera != null)
        {
            mainCamera.enabled = true;
        }

        isScreaming = false;

        // Здесь можно добавить Game Over или перезагрузку уровня
        Debug.Log("Возврат к игре...");
    }
}