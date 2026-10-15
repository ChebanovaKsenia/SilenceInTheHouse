using UnityEngine;
using UnityEngine.UI;

public class SmartDropdown : MonoBehaviour
{
    [Header("Элементы триггера")]
    public Button triggerButton;
    public Image triggerImage;          // Фон триггера (не меняется)
    public Image selectedTextImage;     // Image для отображения выбранного текста

    [Header("Спрайты триггера (по умолчанию)")]
    public Sprite defaultNormal;
    public Sprite defaultHover;

    [Header("Панель с вариантами")]
    public GameObject optionsPanel;

    [Header("Ссылка на SettingsMenu")]
    public SettingsMenu settingsMenu;

    [Header("Данные для SettingsMenu")]
    // Для выпадающего списка разрешения
    public int resolutionWidth = 0;
    public int resolutionHeight = 0;

    // Для выпадающего списка режима экрана
    public FullScreenMode screenMode = FullScreenMode.Windowed;

    private bool isOpen = false;

    void Start()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (triggerImage != null && defaultNormal != null)
        {
            triggerImage.sprite = defaultNormal;
        }

        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(ToggleDropdown);
        }
    }

    void ToggleDropdown()
    {
        if (isOpen)
            CloseDropdown();
        else
            OpenDropdown();
    }

    void OpenDropdown()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            isOpen = true;
        }
    }

    public void CloseDropdown()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
            isOpen = false;
        }
    }

    // Этот метод вызывается из DropdownOption при клике на вариант
    public void SelectOption(Sprite textSprite)
    {
        // Меняем только спрайт текста, а не фон триггера
        if (selectedTextImage != null && textSprite != null)
        {
            selectedTextImage.sprite = textSprite;
            selectedTextImage.gameObject.SetActive(true);
        }

        // Сообщаем SettingsMenu о выборе
        NotifySettingsMenu();

        CloseDropdown();
    }

    // Уведомляем SettingsMenu о выбранном варианте
    void NotifySettingsMenu()
    {
        if (settingsMenu == null) return;

        // Если это dropdown разрешения (есть ширина и высота)
        if (resolutionWidth > 0 && resolutionHeight > 0)
        {
            settingsMenu.OnResolutionSelected(resolutionWidth, resolutionHeight);
        }
        // Если это dropdown режима экрана
        else
        {
            settingsMenu.OnScreenModeSelected(screenMode);
        }
    }
}