using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("Настройки")]
    public string nextSceneName = "GameScene"; // имя следующей сцены
    public float minimumLoadTime = 2f; // минимальное время показа загрузки (чтобы игрок успел увидеть анимацию)

    private AsyncOperation asyncLoad;
    private float loadStartTime;

    void Start()
    {
        loadStartTime = Time.time;
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // Начинаем асинхронную загрузку
        asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false; // не активируем сцену сразу

        Debug.Log("Начинаем загрузку сцены: " + nextSceneName);

        // Ждём, пока сцена загрузится ИЛИ пройдёт минимальное время
        while (!asyncLoad.isDone)
        {
            // Прогресс загрузки (0 to 0.9, потом становится 1 когда готова)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Прогресс загрузки: " + (progress * 100) + "%");

            // Проверяем: сцена загружена И прошло минимальное время?
            if (asyncLoad.progress >= 0.9f && (Time.time - loadStartTime) >= minimumLoadTime)
            {
                Debug.Log("Сцена загружена, активируем...");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Сцена активирована!");
    }
}