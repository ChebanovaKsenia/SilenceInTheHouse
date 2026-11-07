using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingAnimation : MonoBehaviour
{
    [Header("Вращающийся круг")]
    public Image loadingCircle;
    public float rotationSpeed = 180f; // градусов в секунду

    [Header("Точки")]
    public TextMeshProUGUI[] dots; // массив из 3 точек
    public float jumpHeight = 10f; // высота подпрыгивания
    public float waveSpeed = 3f; // скорость волны
    public float dotSpacing = 0.2f; // задержка между точками

    private float[] baseYPositions; // исходные Y позиции точек
    private Vector3[] basePositions;

    void Start()
    {
        // Сохраняем исходные позиции точек
        basePositions = new Vector3[dots.Length];
        for (int i = 0; i < dots.Length; i++)
        {
            basePositions[i] = dots[i].rectTransform.anchoredPosition;
        }
    }

    void Update()
    {
        // Вращение круга
        if (loadingCircle != null)
        {
            loadingCircle.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }

        // Анимация точек (волна)
        for (int i = 0; i < dots.Length; i++)
        {
            // Вычисляем смещение по синусоиде
            float timeOffset = Time.time * waveSpeed + (i * dotSpacing * Mathf.PI * 2);
            float yOffset = Mathf.Sin(timeOffset) * jumpHeight;

            // Применяем смещение
            Vector3 newPos = basePositions[i];
            newPos.y += yOffset;
            dots[i].rectTransform.anchoredPosition = newPos;
        }
    }
}