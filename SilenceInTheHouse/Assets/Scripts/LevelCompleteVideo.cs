using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class LevelCompleteVideo : MonoBehaviour
{
    [Header("Видео")]
    public VideoPlayer videoPlayer;
    public RawImage videoScreen;

    [Header("Настройки")]
    public float videoDuration = 7.019f;
    public string mainMenuSceneName = "MainMenuScene";

    [Header("Затемнение")]
    public GameObject fadeOverlay;

    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        if (fadeOverlay != null) fadeOverlay.SetActive(false);

        // Выключаем видео при старте
        gameObject.SetActive(false);
    }

    public void PlayVideo()
    {
        Debug.Log("LevelCompleteVideo.PlayVideo() вызван!");
        StartCoroutine(VideoSequence());
    }

    IEnumerator VideoSequence()
    {
        Debug.Log("Начинаем последовательность видео...");

        // Включаем видео
        if (videoScreen != null)
        {
            videoScreen.gameObject.SetActive(true);
            Debug.Log("VideoScreen включён");
        }

        // Запускаем музыку с плавным появлением
        if (audioManager != null)
        {
            Debug.Log("Запускаем музыку...");
            audioManager.PlayLevelCompleteMusic();
        }

        // Запускаем видео
        if (videoPlayer != null)
        {
            Debug.Log("Запускаем Video Player...");
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("VideoPlayer не назначен!");
        }

        // Ждём окончания видео
        Debug.Log("Ждём " + videoDuration + " секунд...");
        yield return new WaitForSeconds(videoDuration);

        // Возвращаемся в главное меню
        Debug.Log("Возврат в главное меню...");
        ReturnToMainMenu();
    }

    void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }
}