
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

    [Header("Точки шкафа")]
    public Transform hidePoint;
    public Transform peekPoint;
    public Transform exitPoint;

    [Tooltip("Точка на NavMesh перед шкафом, куда идёт монстр. " +
             "Если не задана, используется корень шкафа.")]
    public Transform monsterCheckPoint;

    [Header("Взаимодействие")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionDistance = 2f;
    public TMP_Text interactionText;

    [Header("Перемещение")]
    [Min(0.05f)] public float enterDuration = 0.7f;
    [Min(0.05f)] public float exitDuration = 0.7f;

    [Header("Камера внутри шкафа")]
    public float hiddenMouseSensitivity = 2f;

    [Tooltip("Максимальный поворот влево и вправо относительно щели.")]
    [Range(5f, 80f)]
    public float maxPeekYaw = 35f;

    [Tooltip("Максимальный поворот вверх и вниз относительно щели.")]
    [Range(5f, 60f)]
    public float maxPeekPitch = 25f;

    [Header("Видимость игрока")]
    [Tooltip("Назначь только визуальную модель игрока, не корневой объект.")]
    public GameObject playerVisual;

    private bool isPlayerInside;
    private bool isTransitioning;

    private Coroutine transitionCoroutine;

    private Transform originalCameraParent;
    private Vector3 originalCameraLocalPosition;
    private Quaternion originalCameraLocalRotation;

    private bool originalColliderEnabled;
    private bool originalRigidbodyKinematic;
    private bool originalRigidbodyUseGravity;

    private bool originalCursorVisible;
    private CursorLockMode originalCursorLockMode;

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

        originalCursorVisible = Cursor.visible;
        originalCursorLockMode = Cursor.lockState;

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInside)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (player == null || isTransitioning)
            return;

        if (isPlayerInside)
        {
            SetPrompt("E — выйти");
            HandleHiddenCamera();

            if (Input.GetKeyDown(interactKey))
                BeginExit();

            return;
        }

        bool playerIsNear = IsPlayerNearWardrobe();

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(playerIsNear);

            if (playerIsNear)
                interactionText.text = "E — спрятаться";
        }

        if (playerIsNear && Input.GetKeyDown(interactKey))
            BeginEnter();

    }

    private bool ValidateReferences()
    {
        bool valid = true;

        if (player == null ||
            playerController == null ||
            firstPersonCamera == null ||
            playerRigidbody == null ||
            playerCollider == null ||
            playerCamera == null ||
            hidePoint == null ||
            peekPoint == null ||
            exitPoint == null ||
            monsterAI == null)
        {
            Debug.LogError(
                "[WardrobeHiding] Не назначены обязательные ссылки в Inspector!",
                this
            );

            valid = false;
        }

        return valid;
    }

    private bool IsPlayerNearWardrobe()
    {
        return Vector3.Distance(player.position, transform.position)
               <= interactionDistance;
    }

    private void BeginEnter()
    {
        if (isPlayerInside || isTransitioning)
            return;

        if (monsterAI != null && monsterAI.playerHasWon)
            return;

        // Сначала немедленно сообщаем монстру, что игрок начал прятаться.
        // Это происходит ДО перемещения и до отключения коллайдера.
        Transform checkPoint = monsterCheckPoint != null
            ? monsterCheckPoint
            : transform;

        monsterAI.OnPlayerHide(checkPoint);

        originalCursorVisible = Cursor.visible;
        originalCursorLockMode = Cursor.lockState;

        isTransitioning = true;
        transitionCoroutine = StartCoroutine(EnterWardrobeRoutine());
    }

    private IEnumerator EnterWardrobeRoutine()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

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
        Quaternion targetRotation =
            Quaternion.Euler(0f, hidePoint.eulerAngles.y, 0f);

        float elapsed = 0f;

        while (elapsed < enterDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / enterDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            player.position =
                Vector3.Lerp(startPosition, targetPosition, t);

            player.rotation =
                Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        player.position = targetPosition;
        player.rotation = targetRotation;

        // Перемещаем камеру к щели шкафа.
        playerCamera.transform.SetParent(peekPoint, false);
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;

        peekBaseLocalRotation = playerCamera.transform.localRotation;
        peekYaw = 0f;
        peekPitch = 0f;

        isPlayerInside = true;

        if (playerVisual != null)
            playerVisual.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isTransitioning = false;
        transitionCoroutine = null;

        SetPrompt("E — выйти");

        Debug.Log("[WardrobeHiding] Игрок спрятался в шкафу.");
    }

    private void HandleHiddenCamera()
    {
        if (playerCamera == null)
            return;

        float mouseX = Input.GetAxis("Mouse X") * hiddenMouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * hiddenMouseSensitivity;

        peekYaw += mouseX;
        peekPitch -= mouseY;

        peekYaw = Mathf.Clamp(peekYaw, -maxPeekYaw, maxPeekYaw);
        peekPitch = Mathf.Clamp(peekPitch, -maxPeekPitch, maxPeekPitch);

        playerCamera.transform.localRotation =
            peekBaseLocalRotation *
            Quaternion.Euler(peekPitch, peekYaw, 0f);
    }

    private void BeginExit()
    {
        if (!isPlayerInside || isTransitioning)
            return;

        isTransitioning = true;
        transitionCoroutine = StartCoroutine(ExitWardrobeRoutine());
    }

    private IEnumerator ExitWardrobeRoutine()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        // Возвращаем камеру к её исходному родителю и трансформу.
        playerCamera.transform.SetParent(originalCameraParent, false);
        playerCamera.transform.localPosition = originalCameraLocalPosition;
        playerCamera.transform.localRotation = originalCameraLocalRotation;

        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;

        Vector3 targetPosition = exitPoint.position;
        Quaternion targetRotation =
            Quaternion.Euler(0f, exitPoint.eulerAngles.y, 0f);

        float elapsed = 0f;

        while (elapsed < exitDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / exitDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            player.position =
                Vector3.Lerp(startPosition, targetPosition, t);

            player.rotation =
                Quaternion.Slerp(startRotation, targetRotation, t);

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

        // Проверяем монстра после перемещения игрока наружу.
        // Если он рядом, MonsterAI сразу запускает смерть.
        monsterAI.OnPlayerExitWardrobe();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("[WardrobeHiding] Игрок вышел из шкафа.");
    }

    private void SetPrompt(string message)
    {
        if (interactionText == null)
            return;

        interactionText.text = message;
        interactionText.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
    }
}
