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

    [Header("Ярость")]
    public float enragedSpeedMultiplier = 2f;
    public float enragedDetectionRadius = 15f;
    private bool isEnraged = false;

    [Header("Состояние игры")]
    public bool playerHasWon = false; // Флаг: игрок выиграл

    public NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool isDead = false;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    // Публичный доступ к агенту для кровати
    public NavMeshAgent Agent => agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.speed = walkSpeed;
        agent.updateRotation = true;

        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = Random.Range(0, patrolPoints.Length);
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        // ЕСЛИ ИГРОК ВЫИГРАЛ — МОНСТР ЗАМИРАЕТ
        if (player == null || isDead || playerHasWon) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (distToPlayer < detectionRadius)
                {
                    currentState = State.Chase;
                    agent.speed = runSpeed;
                    Debug.Log("МОНСТР УВИДЕЛ ИГРОКА!");
                }
                break;

            case State.Chase:
                Chase();
                if (distToPlayer < attackRadius)
                {
                    currentState = State.Attack;
                    TriggerDeath();
                }
                else if (distToPlayer > detectionRadius * 2f)
                {
                    currentState = State.Patrol;
                    agent.speed = walkSpeed;
                    Debug.Log("Игрок убежал.");
                }
                break;

            case State.Attack:
                break;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            animator.SetFloat("Speed", 0f);
            if (waitTimer >= waitTimeAtPoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                ChooseRandomNextPoint();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            isWaiting = true;
            waitTimer = 0f;
        }
        else
        {
            animator.SetFloat("Speed", 0.3f);
        }
    }

    void ChooseRandomNextPoint()
    {
        if (patrolPoints.Length <= 1) return;
        int newIndex;
        do { newIndex = Random.Range(0, patrolPoints.Length); } while (newIndex == currentPatrolIndex);
        currentPatrolIndex = newIndex;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Chase()
    {
        agent.SetDestination(player.position);
        animator.SetFloat("Speed", 1f);
    }

    public void Enrage()
    {
        if (isEnraged) return;
        isEnraged = true;

        Debug.Log("МОНСТР ВПАДАЕТ В ЯРОСТЬ!");

        runSpeed *= enragedSpeedMultiplier;
        agent.speed = runSpeed;
        detectionRadius = enragedDetectionRadius;

        currentState = State.Chase;
    }

    void TriggerDeath()
    {
        isDead = true;
        agent.isStopped = true;
        agent.updateRotation = false;
        animator.SetFloat("Speed", 0f);

        Debug.Log("МОНСТР УБИЛ ИГРОКА!");

        if (deathVideoPlayer != null)
        {
            deathVideoPlayer.PlayDeathVideo();
        }
    }

    void LateUpdate()
    {
        if (currentState != State.Attack && agent.velocity.magnitude > 0.1f)
        {
            Vector3 lookDirection = agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}