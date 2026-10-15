using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderController : MonoBehaviour
{
    public Slider slider;

    [Header("Спрайты для смены при наведении")]
    public Sprite bgInactive;
    public Sprite bgActive;
    public Sprite fillInactive;
    public Sprite fillActive;
    public Sprite handleNormal;
    public Sprite handleActive;

    private Image bgImage;
    private Image fillImage;
    private Image handleImage;

    void Start()
    {
        // Находим компоненты слайдера
        Transform bgTransform = transform.Find("Background");
        Transform fillTransform = transform.Find("Fill Area/Fill");
        Transform handleTransform = transform.Find("Handle Slide Area/Handle");

        if (bgTransform != null) bgImage = bgTransform.GetComponent<Image>();
        if (fillTransform != null) fillImage = fillTransform.GetComponent<Image>();
        if (handleTransform != null) handleImage = handleTransform.GetComponent<Image>();

        // Проверяем слайдер
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        if (slider != null)
        {
            // Удаляем старые слушатели, чтобы не дублировались
            slider.onValueChanged.RemoveAllListeners();

            // Подписываемся на изменение
            slider.onValueChanged.AddListener(OnVolumeChanged);

            // Сразу применяем текущее значение
            OnVolumeChanged(slider.value);
        }
        else
        {
            Debug.LogError("VolumeSliderController: Slider не найден!");
        }
    }

    void OnVolumeChanged(float value)
    {
        // Применяем громкость напрямую через AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
        else
        {
            // Запасной вариант, если AudioManager нет
            AudioListener.volume = value;
        }
    }

    public void OnHoverEnter()
    {
        if (bgImage && bgActive) bgImage.sprite = bgActive;
        if (fillImage && fillActive) fillImage.sprite = fillActive;
        if (handleImage && handleActive) handleImage.sprite = handleActive;
    }

    public void OnHoverExit()
    {
        if (bgImage && bgInactive) bgImage.sprite = bgInactive;
        if (fillImage && fillInactive) fillImage.sprite = fillInactive;
        if (handleImage && handleNormal) handleImage.sprite = handleNormal;
    }
}