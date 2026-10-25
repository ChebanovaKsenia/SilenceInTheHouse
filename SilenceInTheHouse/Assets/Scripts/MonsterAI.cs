
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform player;
    public Transform[] patrolPoints;
    public DeathVideoPlayer deathVideoPlayer;

    [Header("Настройки")]
    public float detectionRadius = 2f;
    public float attackRadius = 1f;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float waitTimeAtPoint = 10f;

    [Header("Проверка шкафа")]
    public float wardrobeWaitTime = 5f;
    public float wardrobeArrivalDistance = 1.2f;

    [Header("Ярость")]
    public float enragedSpeedMultiplier = 2f;
    public float enragedDetectionRadius = 15f;

    [Header("Состояние игры")]
    public bool playerHasWon = false;

    [Header("Компоненты")]
    public NavMeshAgent agent;

    private Animator animator;

    private int currentPatrolIndex;
    private float waitTimer;
    private float wardrobeTimer;

    private bool isWaiting;
    private bool isDead;
    private bool isEnraged;
    private bool isPlayerHidden;
    private bool chaseWasActiveWhenHidden;
    private bool patrolPointSelected;

    private Transform wardrobeCheckPoint;

    private enum State
    {
        Patrol,
        Chase,
        Attack,
        CheckWardrobe
    }

    private State currentState = State.Patrol;

    public NavMeshAgent Agent => agent;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();
        if (agent == null)
        {
            Debug.LogError("[MonsterAI] На объекте монстра нет NavMeshAgent!", this);
            enabled = false;
            return;
        }

        agent.speed = walkSpeed;
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.autoBraking = true;

        // ВАЖНО: больше не поворачиваем корневой объект
        // принудительно в LateUpdate.

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            currentPatrolIndex = Random.Range(0, patrolPoints.Length);
            SetPatrolDestination();
        }
        else
        {
            Debug.LogWarning("[MonsterAI] Не назначены Patrol Points!", this);
            SetAnimationSpeed(0f);
        }
    }

    private void Update()
    {
        if (player == null || isDead || playerHasWon)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();

                if (!isPlayerHidden && distanceToPlayer <= detectionRadius)
                {
                    StartChase();
                    Debug.Log("[MonsterAI] Монстр заметил игрока.");
                }
                break;

            case State.Chase:
                if (isPlayerHidden)
                {
                    ReturnToPatrol();
                    break;
                }

                Chase();

                if (distanceToPlayer <= attackRadius)
                {
                    currentState = State.Attack;
                    TriggerDeath();
                }
                else if (distanceToPlayer > detectionRadius * 2f)
                {
                    ReturnToPatrol();
                    Debug.Log("[MonsterAI] Монстр потерял игрока.");
                }
                break;

            case State.Attack:
                break;

            case State.CheckWardrobe:
                CheckWardrobe();
                break;
        }
    }

    // ---------------- ПАТРУЛЬ ----------------

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            SetAnimationSpeed(0f);
            return;
        }

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTimeAtPoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                agent.isStopped = false;
                ChooseRandomNextPoint();
            }

            return;
        }

        if (agent.pathPending)
        {
            SetAnimationSpeed(1f);
            return;
        }

        if (!agent.hasPath)
        {
            if (!patrolPointSelected)
                ChooseRandomNextPoint();

            SetAnimationSpeed(0f);
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 0.15f)
        {
            isWaiting = true;
            waitTimer = 0f;
            agent.isStopped = true;
            SetAnimationSpeed(0f);
        }
        else
        {
            SetAnimationSpeed(1f);
        }
    }

    private void ChooseRandomNextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        int validCount = 0;

        foreach (Transform point in patrolPoints)
        {
            if (point != null)
                validCount++;
        }

        if (validCount == 0)
        {
            patrolPointSelected = false;
            SetAnimationSpeed(0f);
            return;
        }

        int nextIndex = currentPatrolIndex;

        if (validCount > 1)
        {
            int attempts = 0;

            do
            {
                nextIndex = Random.Range(0, patrolPoints.Length);
                attempts++;
            }
            while (
                (patrolPoints[nextIndex] == null ||
                 nextIndex == currentPatrolIndex) &&
                attempts < 100
            );
        }
        else
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    nextIndex = i;
                    break;
                }
            }
        }

        if (patrolPoints[nextIndex] == null)
        {
            patrolPointSelected = false;
            return;
        }

        currentPatrolIndex = nextIndex;
        SetPatrolDestination();
    }

    private void SetPatrolDestination()
    {
        if (agent == null ||
            patrolPoints == null ||
            patrolPoints.Length == 0 ||
            patrolPoints[currentPatrolIndex] == null)
        {
            patrolPointSelected = false;
            return;
        }

        agent.isStopped = false;
        agent.speed = walkSpeed;

        bool pathSet = agent.SetDestination(
            patrolPoints[currentPatrolIndex].position
        );

        patrolPointSelected = pathSet;
        isWaiting = false;
        waitTimer = 0f;

        SetAnimationSpeed(pathSet ? 1f : 0f);

        if (!pathSet)
            Debug.LogWarning("[MonsterAI] Не удалось построить путь к точке патруля.");
    }

    // ---------------- ПОГОНЯ ----------------

    private void Chase()
    {
        if (isPlayerHidden || player == null)
            return;

        agent.isStopped = false;
        agent.speed = runSpeed;

        // Обновляем цель во время погони.
        agent.SetDestination(player.position);

        SetAnimationSpeed(1f);
    }

    private void StartChase()
    {
        if (isPlayerHidden || isDead)
            return;

        currentState = State.Chase;
        isWaiting = false;
        patrolPointSelected = false;

        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);

        SetAnimationSpeed(1f);
    }

    private void ReturnToPatrol()
    {
        currentState = State.Patrol;

        isWaiting = false;
        waitTimer = 0f;
        patrolPointSelected = false;

        agent.isStopped = false;
        agent.speed = walkSpeed;

        ChooseRandomNextPoint();
    }

    // ---------------- ШКАФ ----------------

    public void OnPlayerHide(Transform checkPoint)
    {
        if (isDead || playerHasWon || isPlayerHidden)
            return;

        // Сохраняем, преследовал ли монстр игрока до пряток.
        chaseWasActiveWhenHidden =
            currentState == State.Chase || isEnraged;

        isPlayerHidden = true;
        wardrobeCheckPoint = checkPoint;

        if (chaseWasActiveWhenHidden && wardrobeCheckPoint != null)
        {
            currentState = State.CheckWardrobe;
            wardrobeTimer = 0f;
            isWaiting = false;
            patrolPointSelected = false;

            agent.isStopped = false;
            agent.speed = runSpeed;
            agent.SetDestination(wardrobeCheckPoint.position);

            SetAnimationSpeed(1f);
        }
        else
        {
            // Если монстр не преследовал игрока, продолжаем его патруль.
            if (currentState != State.Patrol)
                ReturnToPatrol();
        }
    }

    private void CheckWardrobe()
    {
        if (wardrobeCheckPoint == null)
        {
            ReturnToPatrol();
            return;
        }

        if (agent.pathPending)
        {
            SetAnimationSpeed(1f);
            return;
        }

        if (!agent.hasPath ||
            agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            FinishWardrobeCheck();
            return;
        }

        if (agent.remainingDistance > wardrobeArrivalDistance)
        {
            SetAnimationSpeed(1f);
            return;
        }

        agent.isStopped = true;
        SetAnimationSpeed(0f);

        wardrobeTimer += Time.deltaTime;

        if (wardrobeTimer >= wardrobeWaitTime)
            FinishWardrobeCheck();
    }

    private void FinishWardrobeCheck()
    {
        wardrobeCheckPoint = null;
        wardrobeTimer = 0f;

        // Игрок остаётся скрытым. Монстр возвращается к патрулю.
        ReturnToPatrol();
    }

    public void OnPlayerExitWardrobe()
    {
        if (isDead || playerHasWon)
            return;

        isPlayerHidden = false;

        if (player == null)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius)
        {
            currentState = State.Attack;
            TriggerDeath();
            return;
        }

        wardrobeCheckPoint = null;

        if (chaseWasActiveWhenHidden || isEnraged)
        {
            chaseWasActiveWhenHidden = false;
            StartChase();
        }
        else
        {
            chaseWasActiveWhenHidden = false;
            ReturnToPatrol();
        }
    }

    // ---------------- ЯРОСТЬ ----------------

    public void Enrage()
    {
        if (isEnraged || isDead)
            return;

        isEnraged = true;

        runSpeed *= enragedSpeedMultiplier;
        detectionRadius = enragedDetectionRadius;

        if (!isPlayerHidden)
            StartChase();

        Debug.Log("[MonsterAI] Монстр впал в ярость!");
    }

    // ---------------- СМЕРТЬ ----------------

    private void TriggerDeath()
    {
        if (isDead)
            return;

        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.updateRotation = false;
        }

        SetAnimationSpeed(0f);

        if (deathVideoPlayer != null)
        {
            deathVideoPlayer.PlayDeathVideo();
        }
        else
        {
            Debug.LogWarning("[MonsterAI] DeathVideoPlayer не назначен.");
        }
    }

    private void SetAnimationSpeed(float speed)
    {
        if (animator != null)
            animator.SetFloat("Speed", speed);
    }
    private void LateUpdate()
    {
        if (currentState != State.Attack &&
            agent != null &&
            agent.velocity.magnitude > 0.1f)
        {
            Vector3 lookDirection = agent.velocity.normalized;

            Quaternion targetRotation =
                Quaternion.LookRotation(lookDirection) *
                Quaternion.Euler(0f, 90f, 0f);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }
    }
}
