
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class MonsterFootsteps : MonoBehaviour
{
    [Header("Звуки шагов")]
    public AudioClip step1;
    public AudioClip step2;

    [Header("Компоненты")]
    public NavMeshAgent agent;
    private AudioSource audioSource;

    [Header("Частота шагов")]
    public float walkStepInterval = 0.65f;
    public float runStepInterval = 0.4f;

    [Header("Определение бега")]
    public float runSpeedThreshold = 3f;

    [Header("Звук")]
    [Range(0f, 1f)]
    public float volume = 0.8f;

    [Range(0.8f, 1.2f)]
    public float walkPitch = 1f;

    [Range(0.8f, 1.2f)]
    public float runPitch = 1.05f;

    [Header("3D-звук")]
    public float minDistance = 1f;
    public float maxDistance = 18f;

    private float stepTimer;
    private bool useFirstStep = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.volume = volume;
        audioSource.pitch = 1f;
    }

    private void Update()
    {
        if (agent == null || !agent.isOnNavMesh ||
            agent.isStopped || agent.velocity.magnitude < 0.15f)
        {
            stepTimer = 0f;
            return;
        }

        bool isRunning = agent.speed >= runSpeedThreshold;

        float currentInterval = isRunning
            ? runStepInterval
            : walkStepInterval;

        audioSource.pitch = isRunning ? runPitch : walkPitch;

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            PlayFootstep();
            stepTimer = currentInterval;
        }
    }

    private void PlayFootstep()
    {
        AudioClip clip = useFirstStep ? step1 : step2;

        if (clip == null)
            return;

        audioSource.PlayOneShot(clip, volume);

        useFirstStep = !useFirstStep;
    }
}
