
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

    [Header("Затемнение при загрузке")]
    public GameObject fadeOverlay;
    public float fadeOutDuration = 1f;

    [Header("Что происходит после кат-сцены")]
    public Transform playerSpawnPoint;
    public GameObject player;

    private PlayerControllerFPS playerController;
    private MonsterAI monster;
    private bool isPlaying = false;
    private bool cutsceneEnded = false;

    void Start()
    {
        if (player != null)
        {
            // Не отключаем игрока целиком:
            // иначе отключится и его камера.
            playerController = player.GetComponent<PlayerControllerFPS>();

            if (playerController != null)
                playerController.enabled = false;
        }
        else
        {
            Debug.LogError("CutscenePlayer: не назначен Player!");
        }

        monster = FindObjectOfType<MonsterAI>();

        // На время вступления останавливаем монстра.
        if (monster != null)
        {
            if (monster.Agent != null)
                monster.Agent.isStopped = true;

            monster.gameObject.SetActive(false);
        }

        if (cutsceneCanvas != null)
            cutsceneCanvas.SetActive(false);

        if (cutsceneScreen != null)
            cutsceneScreen.gameObject.SetActive(false);

        if (videoPlayer == null)
            Debug.LogError("CutscenePlayer: не назначен VideoPlayer!");
        else if (videoPlayer.clip == null)
            Debug.LogError("CutscenePlayer: в VideoPlayer не назначен видеоклип!");

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
    }

    public void PlayCutscene()
    {
        if (isPlaying)
            return;

        if (videoPlayer == null || videoPlayer.clip == null)
        {
            Debug.LogError("CutscenePlayer: невозможно запустить видео. Проверь VideoPlayer и Clip.");
            return;
        }

        isPlaying = true;
        cutsceneEnded = false;

        Debug.Log("Вступительная кат-сцена запускается!");

        if (fadeOverlay != null)
            StartCoroutine(FadeOutBeforeCutscene());
        else
            StartCutsceneImmediately();
    }

    IEnumerator FadeOutBeforeCutscene()
    {
        Image fadeImage = fadeOverlay.GetComponent<Image>();

        if (fadeImage == null)
        {
            Debug.LogWarning("CutscenePlayer: на FadeOverlay нет компонента Image.");
            StartCutsceneImmediately();
            yield break;
        }

        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        fadeOverlay.SetActive(false);

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
        // Ждём завершения видеоклипа.
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
        if (cutsceneEnded)
            return;

        cutsceneEnded = true;

        Debug.Log("Кат-сцена окончена. Игрок просыпается!");

        if (videoPlayer != null)
            videoPlayer.Stop();

        if (cutsceneCanvas != null)
            cutsceneCanvas.SetActive(false);

        if (cutsceneScreen != null)
            cutsceneScreen.gameObject.SetActive(false);

        if (player != null)
        {
            player.transform.position = playerSpawnPoint != null
                ? playerSpawnPoint.position
                : player.transform.position;

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

        Debug.Log("Вступление завершено. Управление игроком включено.");
    }
}
