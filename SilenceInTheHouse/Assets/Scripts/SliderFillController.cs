using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SliderFillController : MonoBehaviour
{
    private RectTransform rectTransform;
    private float fullWidth;
    private Slider parentSlider;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentSlider = transform.parent.parent.GetComponent<Slider>();

        if (parentSlider == null)
        {
            Debug.LogError("SliderFillController: Не найден родительский Slider!");
            return;
        }

        // Запоминаем полную ширину
        fullWidth = rectTransform.sizeDelta.x;

        // Подписываемся на изменение слайдера
        parentSlider.onValueChanged.AddListener(UpdateFillWidth);

        // Обновляем начальное значение
        UpdateFillWidth(parentSlider.value);
    }

    void UpdateFillWidth(float value)
    {
        // value от 0 до 1
        float newWidth = fullWidth * value;
        Vector2 newSize = rectTransform.sizeDelta;
        newSize.x = newWidth;
        rectTransform.sizeDelta = newSize;
    }
}