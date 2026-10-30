
using System.Collections;
using UnityEngine;

public class ToiletLightFlicker : MonoBehaviour
{
    [Header("Источник света")]
    public Light targetLight;

    [Header("Яркость")]
    [Min(0f)] public float normalIntensity = 1.2f;
    [Min(0f)] public float flickerMinIntensity = 0.1f;
    [Min(0f)] public float flickerMaxIntensity = 1.5f;

    [Header("Частота мерцания")]
    [Min(0.1f)] public float minPause = 2f;
    [Min(0.1f)] public float maxPause = 6f;

    [Header("Длительность мерцания")]
    [Min(0.01f)] public float minFlickerDuration = 0.08f;
    [Min(0.01f)] public float maxFlickerDuration = 0.25f;

    [Range(0f, 1f)]
    [Tooltip("Вероятность полностью погаснуть во время одного всплеска мерцания.")]
    public float blackoutChance = 0.25f;

    private Coroutine flickerCoroutine;

    private void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        if (targetLight == null)
        {
            Debug.LogError(
                "[ToiletLightFlicker] Не найден Light!",
                this
            );

            enabled = false;
            return;
        }

        targetLight.enabled = true;
        targetLight.intensity = normalIntensity;

        flickerCoroutine = StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float pause = Random.Range(
                minPause,
                Mathf.Max(minPause, maxPause)
            );

            yield return new WaitForSeconds(pause);

            // Один короткий всплеск: несколько быстрых скачков яркости.
            int flickerCount = Random.Range(2, 6);

            for (int i = 0; i < flickerCount; i++)
            {
                if (Random.value < blackoutChance)
                {
                    targetLight.intensity = 0f;
                }
                else
                {
                    targetLight.intensity = Random.Range(
                        flickerMinIntensity,
                        flickerMaxIntensity
                    );
                }

                float duration = Random.Range(
                    minFlickerDuration,
                    Mathf.Max(
                        minFlickerDuration,
                        maxFlickerDuration
                    )
                );

                yield return new WaitForSeconds(duration);
            }

            // Возвращаем обычную яркость.
            targetLight.intensity = normalIntensity;
        }
    }

    private void OnDisable()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        if (targetLight != null)
            targetLight.intensity = normalIntensity;
    }
}
