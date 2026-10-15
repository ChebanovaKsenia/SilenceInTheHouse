using UnityEngine;
using UnityEngine.UI;

public class Bed : MonoBehaviour
{
    [Header("Настройки")]
    public float interactionDistance = 3f;

    [Header("Ссылки")]
    public TeddyBear teddyBear;
    public MonsterAI monster;
    public LevelCompleteVideo levelCompleteVideo;
    public GameObject fadeOverlay;

    private GameObject player;
    private PlayerControllerFPS playerController;
    private bool isSleeping = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerControllerFPS>();
        }

        if (teddyBear == null) Debug.LogError("ОШИБКА: Не назначен TeddyBear в кровати!");
        if (levelCompleteVideo == null) Debug.LogError("ОШИБКА: Не назначен LevelCompleteVideo в кровати!");

        if (fadeOverlay != null) fadeOverlay.SetActive(false);
    }

    void Update()
    {
        // Если уже спим или нет мишки/игрока, выходим
        if (isSleeping || player == null || teddyBear == null) return;

        // Проверяем нажатие E
        if (Input.GetKeyDown(KeyCode.E))
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < interactionDistance)
            {
                if (teddyBear.isPickedUp)
                {
                    Debug.Log(">>> ИГРОК НАЖАЛ E У КРОВАТИ С МИШКОЙ. ЗАПУСК ПОБЕДЫ!");
                    SleepWithTeddy();
                }
                else
                {
                    Debug.Log(">>> ИГРОК НАЖАЛ E У КРОВАТИ, НО МИШКА НЕ ПОДОБРАНА!");
                }
            }
        }
    }

    void SleepWithTeddy()
    {
        isSleeping = true;
        Debug.Log(">>> НАЧАЛО ПРОЦЕДУРЫ ПОБЕДЫ (SleepWithTeddy)");

        // 1. Останавливаем монстра навсегда
        if (monster != null)
        {
            monster.playerHasWon = true;
            if (monster.Agent != null) monster.Agent.isStopped = true;
            Debug.Log(">>> МОНСТР ОСТАНОВЛЕН.");
        }

        // 2. Блокируем управление игроком
        if (playerController != null)
        {
            playerController.enabled = false;
            Debug.Log(">>> УПРАВЛЕНИЕ ИГРОКОМ ОТКЛЮЧЕНО.");
        }

        // 3. Кладем мишку на кровать
        teddyBear.Drop();
        teddyBear.transform.SetParent(transform);
        teddyBear.transform.localPosition = new Vector3(0, 0.5f, 0);
        teddyBear.transform.localRotation = Quaternion.Euler(0, 0, 90);

        // 4. Затемнение экрана
        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(true);
            StartCoroutine(FadeToBlack());
        }

        // 5. Запуск видео победы через 1.5 секунды (время затемнения)
        Invoke(nameof(ShowLevelCompleteVideo), 1.5f);
    }

    System.Collections.IEnumerator FadeToBlack()
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Image fadeImage = fadeOverlay.GetComponent<Image>();

        if (fadeImage == null) yield break;

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadeImage.color = color;
            yield return null;
        }
    }

    void ShowLevelCompleteVideo()
    {
        Debug.Log(">>> ВЫЗОВ LevelCompleteVideo.PlayVideo()");
        if (levelCompleteVideo != null)
        {
            levelCompleteVideo.gameObject.SetActive(true);
            levelCompleteVideo.PlayVideo();
        }
    }
}