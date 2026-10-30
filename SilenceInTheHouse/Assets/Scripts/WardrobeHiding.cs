
using System.Collections;
using UnityEngine;
using TMPro;

public class WardrobeHiding : MonoBehaviour
{
    [Header("Игрок")]
    public Transform player;
    public PlayerControllerFPS playerController;
    public FirstPersonCamera firstPersonCamera;
    public Rigidbody playerRigidbody;
    public CapsuleCollider playerCollider;
    public Camera playerCamera;

    [Header("Монстр")]
    public MonsterAI monsterAI;

    [Header("Точки укрытия")]
    public Transform hidePoint;
    public Transform peekPoint;
    public Transform exitPoint;

    [Tooltip("Точка на NavMesh перед укрытием, куда идёт монстр.")]
    public Transform monsterCheckPoint;

    [Header("Взаимодействие")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionDistance = 2f;

    [Tooltip("Необязательная точка для проверки расстояния до укрытия. Если пусто, используется позиция объекта со скриптом.")]
    public Transform interactionPoint;

    [Tooltip("Общий UI TextMeshPro. Можно назначить один и тот же текст всем укрытиям.")]
    public TMP_Text interactionText;

    [Header("Перемещение")]
    [Min(0.05f)] public float enterDuration = 0.7f;
    [Min(0.05f)] public float exitDuration = 0.7f;

    [Header("Камера внутри укрытия")]
    public float hiddenMouseSensitivity = 2f;

    [Range(5f, 80f)]
    public float maxPeekYaw = 35f;

    [Range(5f, 60f)]
    public float maxPeekPitch = 25f;

    [Header("Видимость игрока")]
    [Tooltip("Назначь только визуальную модель игрока, не корневой объект.")]
    public GameObject playerVisual;

    // Единственное укрытие, которым игрок сейчас занят.
    private static WardrobeHiding activeHiding;

    // Единственная подсказка для всех укрытий.
    private static TMP_Text sharedText;

    private bool isPlayerInside;
    private bool isTransitioning;
    private Coroutine transitionCoroutine;

    private Transform originalCameraParent;
    private Vector3 originalCameraLocalPosition;
    private Quaternion originalCameraLocalRotation;

    private bool originalColliderEnabled;
    private bool originalRigidbodyKinematic;
    private bool originalRigidbodyUseGravity;

    private Quaternion peekBaseLocalRotation;
    private float peekYaw;
    private float peekPitch;

    private void Start()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        originalCameraParent = playerCamera.transform.parent;
        originalCameraLocalPosition = playerCamera.transform.localPosition;
        originalCameraLocalRotation = playerCamera.transform.localRotation;

        originalColliderEnabled = playerCollider.enabled;
        originalRigidbodyKinematic = playerRigidbody.isKinematic;
        originalRigidbodyUseGravity = playerRigidbody.useGravity;

        // Не выключаем общий текст здесь:
        // другие экземпляры этого скрипта используют его тоже.
        if (interactionText != null)
        {
            sharedText = interactionText;
        }
    }

    private void Update()
    {
        // Пока игрок занят конкретным укрытием,
        // остальные укрытия не меняют текст и управление.
        if (activeHiding != null)
        {
            if (activeHiding != this)
                return;

            if (isTransitioning)
            {
                HidePrompt();
                return;
            }

            if (isPlayerInside)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SetPrompt("E — выйти");
                HandleHiddenCamera();

                if (Input.GetKeyDown(interactKey))
                    BeginExit();

                return;
            }
        }

        // Если игрок ни в одном укрытии не находится,
        // подсказкой управляет ближайшее доступное укрытие.
        WardrobeHiding nearest = FindNearestHidingPlace();

        if (nearest == this)
        {
            SetPrompt("E — спрятаться");

            if (Input.GetKeyDown(interactKey))
                BeginEnter();
        }
        else if (nearest == null)
        {
            HidePrompt();
        }
    }

    private bool ValidateReferences()
    {
        if (player == null ||
            playerController == null ||
            firstPersonCamera == null ||
            playerRigidbody == null ||
            playerCollider == null ||
            playerCamera == null ||
            hidePoint == null ||
            peekPoint == null ||
            exitPoint == null ||
            monsterAI == null ||
            interactionText == null)
        {
            Debug.LogError(
                "[WardrobeHiding] Не назначены обязательные ссылки в Inspector!",
                this
            );

            return false;
        }

        return true;
    }

    private Vector3 GetInteractionPosition()
    {
        return interactionPoint != null
            ? interactionPoint.position
            : transform.position;
    }

    private bool IsPlayerNear()
    {
        if (player == null)
            return false;

        return Vector3.Distance(
            player.position,
            GetInteractionPosition()
        ) <= interactionDistance;
    }

    private WardrobeHiding FindNearestHidingPlace()
    {
        WardrobeHiding[] hidingPlaces =
            FindObjectsOfType<WardrobeHiding>();

        WardrobeHiding nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (WardrobeHiding place in hidingPlaces)
        {
            if (place == null ||
                !place.isActiveAndEnabled ||
                place.isTransitioning ||
                place.isPlayerInside ||
                place.player == null ||
                place.interactionText == null)
            {
                continue;
            }

            if (place.player != player)
                continue;

            if (place.monsterAI != null &&
                place.monsterAI.playerHasWon)
            {
                continue;
            }

            if (!place.IsPlayerNear())
                continue;

            float distance = Vector3.Distance(
                player.position,
                place.GetInteractionPosition()
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = place;
            }
        }

        return nearest;
    }

    private void BeginEnter()
    {
        if (isPlayerInside || isTransitioning)
            return;

        if (activeHiding != null)
            return;

        if (monsterAI != null && monsterAI.playerHasWon)
            return;

        activeHiding = this;

        Transform checkPoint = monsterCheckPoint != null
            ? monsterCheckPoint
            : transform;

        monsterAI.OnPlayerHide(checkPoint);

        isTransitioning = true;
        transitionCoroutine = StartCoroutine(EnterRoutine());
    }

    private IEnumerator EnterRoutine()
    {
        HidePrompt();

        playerController.enabled = false;
        firstPersonCamera.enabled = false;

        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        playerRigidbody.isKinematic = true;
        playerRigidbody.useGravity = false;
        playerCollider.enabled = false;

        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;

        Vector3 targetPosition = hidePoint.position;
        Quaternion targetRotation = Quaternion.Euler(
            0f,
            hidePoint.eulerAngles.y,
            0f
        );

        float elapsed = 0f;

        while (elapsed < enterDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / enterDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            player.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            player.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        player.position = targetPosition;
        player.rotation = targetRotation;

        playerCamera.transform.SetParent(peekPoint, false);
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;

        peekBaseLocalRotation = playerCamera.transform.localRotation;
        peekYaw = 0f;
        peekPitch = 0f;

        if (playerVisual != null)
            playerVisual.SetActive(false);

        isPlayerInside = true;
        isTransitioning = false;
        transitionCoroutine = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetPrompt("E — выйти");

        Debug.Log(
            "[WardrobeHiding] Игрок спрятался: " + gameObject.name,
            this
        );
    }

    private void HandleHiddenCamera()
    {
        if (playerCamera == null)
            return;

        float mouseX =
            Input.GetAxis("Mouse X") * hiddenMouseSensitivity;

        float mouseY =
            Input.GetAxis("Mouse Y") * hiddenMouseSensitivity;

        peekYaw += mouseX;
        peekPitch -= mouseY;

        peekYaw = Mathf.Clamp(
            peekYaw,
            -maxPeekYaw,
            maxPeekYaw
        );

        peekPitch = Mathf.Clamp(
            peekPitch,
            -maxPeekPitch,
            maxPeekPitch
        );

        playerCamera.transform.localRotation =
            peekBaseLocalRotation *
            Quaternion.Euler(peekPitch, peekYaw, 0f);
    }

    private void BeginExit()
    {
        if (!isPlayerInside || isTransitioning)
            return;

        isTransitioning = true;
        transitionCoroutine = StartCoroutine(ExitRoutine());
    }

    private IEnumerator ExitRoutine()
    {
        HidePrompt();

        playerCamera.transform.SetParent(
            originalCameraParent,
            false
        );

        playerCamera.transform.localPosition =
            originalCameraLocalPosition;

        playerCamera.transform.localRotation =
            originalCameraLocalRotation;

        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;

        Vector3 targetPosition = exitPoint.position;
        Quaternion targetRotation = Quaternion.Euler(
            0f,
            exitPoint.eulerAngles.y,
            0f
        );

        float elapsed = 0f;

        while (elapsed < exitDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / exitDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            player.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            player.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        player.position = targetPosition;
        player.rotation = targetRotation;

        if (playerVisual != null)
            playerVisual.SetActive(true);

        playerCollider.enabled = originalColliderEnabled;
        playerRigidbody.isKinematic = originalRigidbodyKinematic;
        playerRigidbody.useGravity = originalRigidbodyUseGravity;

        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;

        firstPersonCamera.enabled = true;
        playerController.enabled = true;

        isPlayerInside = false;
        isTransitioning = false;
        transitionCoroutine = null;

        monsterAI.OnPlayerExitWardrobe();

        // Освобождаем общее управление.
        activeHiding = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HidePrompt();

        Debug.Log(
            "[WardrobeHiding] Игрок вышел из укрытия: " + gameObject.name,
            this
        );
    }

    private void SetPrompt(string message)
    {
        TMP_Text text = GetSharedText();

        if (text == null)
            return;

        text.text = message;

        if (!text.gameObject.activeSelf)
            text.gameObject.SetActive(true);
    }

    private void HidePrompt()
    {
        TMP_Text text = GetSharedText();

        if (text != null && text.gameObject.activeSelf)
            text.gameObject.SetActive(false);
    }

    private TMP_Text GetSharedText()
    {
        // Во время пряток приоритет у активного укрытия.
        if (activeHiding != null &&
            activeHiding.interactionText != null)
        {
            return activeHiding.interactionText;
        }

        if (interactionText != null)
            sharedText = interactionText;

        return sharedText;
    }

    private void OnDisable()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        if (activeHiding == this)
        {
            activeHiding = null;
            HidePrompt();
        }
    }
}
