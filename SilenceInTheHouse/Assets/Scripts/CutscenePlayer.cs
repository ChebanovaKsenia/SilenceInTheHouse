using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

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

    private bool isPlaying = false;

    void Start()
    {
        if (cutsceneCanvas != null) cutsceneCanvas.SetActive(false);
        if (player != null) player.SetActive(false);

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
        if (isPlaying) return;
        isPlaying = true;

        Debug.Log("ЗАПУСК КАТ-СЦЕНЫ!");

        if (fadeOverlay != null)
        {
            StartCoroutine(FadeOutBeforeCutscene());
        }
        else
        {
            StartCutsceneImmediately();
        }
    }

    System.Collections.IEnumerator FadeOutBeforeCutscene()
    {
        Image fadeImage = fadeOverlay.GetComponent<Image>();
        if (fadeImage == null) yield break;

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
        // Останавливаем музыку меню
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMenuMusic();
        }

        if (cutsceneCanvas != null) cutsceneCanvas.SetActive(true);
        if (cutsceneScreen != null) cutsceneScreen.gameObject.SetActive(true);

        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }

        Invoke(nameof(EndCutscene), (float)videoPlayer.clip.length);
    }

    void EndCutscene()
    {
        Debug.Log("Кат-сцена окончена. Игрок просыпается!");

        if (cutsceneCanvas != null) cutsceneCanvas.SetActive(false);

        if (player != null)
        {
            player.SetActive(true);
            if (playerSpawnPoint != null)
            {
                player.transform.position = playerSpawnPoint.position;
                player.transform.rotation = playerSpawnPoint.rotation;
            }
        }

        PlayerControllerFPS playerController = player.GetComponent<PlayerControllerFPS>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        MonsterAI monster = FindObjectOfType<MonsterAI>();
        if (monster != null)
        {
            monster.gameObject.SetActive(true);
        }
    }
}