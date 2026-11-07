using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Настройки видео")]
    public VideoPlayer videoPlayer;
    public RawImage cutsceneScreen;
    public GameObject cutsceneCanvas;

    [Header("Чёрный экран (Затемнение)")]
    public GameObject fadeOverlay;
    public float fadeOutDuration = 1.5f; // Длительность плавного появления игры после кат-сцены

    [Header("Что происходит после кат-сцены")]
    public Transform playerSpawnPoint;
    public GameObject player;

    private PlayerControllerFPS playerController;
    private MonsterAI monster;
    private bool isPlaying = false;
    private bool cutsceneEnded = false;

    void Start()
    {
        // 1. Блокируем игрока и монстра
        if (player != null)
        {
            playerController = player.GetComponent<PlayerControllerFPS>();
            if (playerController != null)
                playerController.enabled = false;
        }

        monster = FindObjectOfType<MonsterAI>();
        if (monster != null)
        {
            if (monster.Agent != null)
                monster.Agent.isStopped = true;
            monster.gameObject.SetActive(false);
        }

        // 2. Готовим UI: видео-холст пока скрыт
        if (cutsceneCanvas != null)
            cutsceneCanvas.SetActive(false);
        if (cutsceneScreen != null)
            cutsceneScreen.gameObject.SetActive(false);

        // 3. ВАЖНО: Чёрный экран ВКЛЮЧЕН и полностью непрозрачен с самого начала!
        // Это гарантирует, что игрок НЕ увидит 3D-уровень при загрузке.
        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(true);
            Image fadeImage = fadeOverlay.GetComponent<Image>();
            if (fadeImage != null)
            {
                Color color = fadeImage.color;
                color.a = 1f; // Полностью чёрный
                fadeImage.color = color;
            }
        }
    }

    public void PlayCutscene()
    {
        if (isPlaying) return;

        if (videoPlayer == null || videoPlayer.clip == null)
        {
            Debug.LogError("CutscenePlayer: невозможно запустить видео. Проверь VideoPlayer и Clip.");
            return;
        }

        isPlaying = true;
        cutsceneEnded = false;

        Debug.Log("Вступительная кат-сцена запускается!");

        // ИЗМЕНЕНО: Убрали FadeOutBeforeCutscene. Запускаем видео сразу, 
        // так как cutsceneCanvas перекроет чёрный экран (если у него выше Sort Order).
        StartCutsceneImmediately();
    }

    void StartCutsceneImmediately()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMenuMusic();

        if (cutsceneCanvas != null)
            cutsceneCanvas.SetActive(true);

        if (cutsceneScreen != null)
            cutsceneScreen.gameObject.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.isLooping = false;
        videoPlayer.Play();

        Debug.Log("Видеоплеер запущен.");

        StartCoroutine(WaitForVideoEnd());
    }

    IEnumerator WaitForVideoEnd()
    {
        double duration = videoPlayer.clip.length;

        if (duration <= 0)
        {
            Debug.LogError("CutscenePlayer: длительность видео равна нулю.");
            yield break;
        }

        yield return new WaitForSecondsRealtime((float)duration);

        EndCutscene();
    }

    void EndCutscene()
    {
        if (cutsceneEnded) return;
        cutsceneEnded = true;

        Debug.Log("Кат-сцена окончена. Игрок просыпается!");

        // 1. Останавливаем видео и скрываем его
        if (videoPlayer != null)
            videoPlayer.Stop();
        if (cutsceneCanvas != null)
            cutsceneCanvas.SetActive(false);
        if (cutsceneScreen != null)
            cutsceneScreen.gameObject.SetActive(false);

        // 2. ВКЛЮЧАЕМ чёрный экран обратно, чтобы скрыть момент телепортации игрока
        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(true);
            Image fadeImage = fadeOverlay.GetComponent<Image>();
            if (fadeImage != null)
            {
                Color color = fadeImage.color;
                color.a = 1f;
                fadeImage.color = color;
            }
        }

        // 3. Перемещаем игрока и включаем управление
        if (player != null)
        {
            player.transform.position = playerSpawnPoint != null ? playerSpawnPoint.position : player.transform.position;
            if (playerSpawnPoint != null)
                player.transform.rotation = playerSpawnPoint.rotation;
        }

        if (playerController != null)
            playerController.enabled = true;

        if (monster != null)
        {
            monster.gameObject.SetActive(true);
            if (monster.Agent != null)
                monster.Agent.isStopped = false;
        }

        // 4. Плавно убираем чёрный экран, открывая игру
        StartCoroutine(FadeOutToGameplay());
    }

    IEnumerator FadeOutToGameplay()
    {
        Image fadeImage = fadeOverlay.GetComponent<Image>();
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        fadeOverlay.SetActive(false); // Полностью отключаем, чтобы не мешал

        Debug.Log("Вступление завершено. Управление игроком включено, мир открыт.");
    }
}