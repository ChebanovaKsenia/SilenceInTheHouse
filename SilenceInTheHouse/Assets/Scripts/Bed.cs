
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bed : MonoBehaviour
{
    [Header("Настройки")]
    public float interactionDistance = 1.8f;

    [Header("Точка взаимодействия")]
    public Transform interactionPoint;

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
            playerController = player.GetComponent<PlayerControllerFPS>();
        else
            Debug.LogError("Bed: игрок с тегом Player не найден!");

        if (interactionPoint == null)
            Debug.LogError("Bed: НЕ назначен BedInteractionPoint!");

        if (teddyBear == null)
            Debug.LogError("Bed: не назначен TeddyBear!");

        if (levelCompleteVideo == null)
            Debug.LogError("Bed: не назначен LevelCompleteVideo!");

        if (fadeOverlay != null)
            fadeOverlay.SetActive(false);
    }

    void Update()
    {
        if (isSleeping)
            return;

        // Русская А на английской раскладке — физическая клавиша F.
        if (!Input.GetKeyDown(KeyCode.F))
            return;

        Debug.Log("Bed: нажата клавиша F.");

        // При любой ошибке ссылок взаимодействие запрещено.
        if (player == null || interactionPoint == null || teddyBear == null)
        {
            Debug.LogError(
                "Bed: взаимодействие невозможно. Проверь ссылки в Inspector."
            );
            return;
        }

        // Проверяем полное расстояние: X, Y и Z.
        float distance = Vector3.Distance(
            player.transform.position,
            interactionPoint.position
        );

        Debug.Log(
            "Bed: расстояние до точки кровати = " +
            distance.ToString("F2") + " м."
        );

        // Нельзя активировать кровать с другого этажа.
        if (distance > interactionDistance)
        {
            Debug.Log("Bed: слишком далеко от кровати!");
            return;
        }

        // Мишка должна быть подобрана.
        if (!teddyBear.isPickedUp)
        {
            Debug.Log("Bed: сначала подберите мишку!");
            return;
        }

        // Не разрешаем мгновенно использовать кровать после подбора.
        if (Time.time - teddyBear.lastPickupTime < 1f)
        {
            Debug.Log("Bed: подождите и подойдите к кровати.");
            return;
        }

        SleepWithTeddy();
    }

    void SleepWithTeddy()
    {
        if (isSleeping)
            return;

        isSleeping = true;

        Debug.Log("Bed: игрок рядом с кроватью и имеет мишку.");

        if (monster != null)
        {
            monster.playerHasWon = true;

            if (monster.Agent != null)
                monster.Agent.isStopped = true;
        }

        if (playerController != null)
            playerController.enabled = false;

        teddyBear.Drop();
        teddyBear.transform.SetParent(transform);
        teddyBear.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        teddyBear.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(true);
            StartCoroutine(FadeToBlack());
        }

        Invoke(nameof(ShowLevelCompleteVideo), 1.5f);
    }

    IEnumerator FadeToBlack()
    {
        float duration = 1.5f;
        float elapsed = 0f;

        Image fadeImage = fadeOverlay.GetComponent<Image>();

        if (fadeImage == null)
        {
            Debug.LogWarning("Bed: на FadeOverlay отсутствует компонент Image.");
            yield break;
        }

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

        color.a = 1f;
        fadeImage.color = color;
    }

    void ShowLevelCompleteVideo()
    {
        if (levelCompleteVideo == null)
        {
            Debug.LogError("Bed: не назначен LevelCompleteVideo!");
            return;
        }

        Debug.Log("Bed: запускаем видео концовки.");

        levelCompleteVideo.gameObject.SetActive(true);
        levelCompleteVideo.PlayVideo();
    }
}
