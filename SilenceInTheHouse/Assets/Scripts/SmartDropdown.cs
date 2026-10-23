
using UnityEngine;
using UnityEngine.UI;

public class SmartDropdown : MonoBehaviour
{
    [Header("Элементы триггера")]
    public Button triggerButton;
    public Image triggerImage;
    public Image selectedTextImage;

    [Header("Спрайты триггера")]
    public Sprite defaultNormal;
    public Sprite defaultHover;

    [Header("Панель с вариантами")]
    public GameObject optionsPanel;

    [Header("Ссылка на SettingsMenu")]
    public SettingsMenu settingsMenu;

    [Header("Данные выбранного варианта")]
    public int resolutionWidth = 0;
    public int resolutionHeight = 0;

    public FullScreenMode screenMode = FullScreenMode.Windowed;

    private bool isOpen = false;

    private void Start()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (triggerImage != null && defaultNormal != null)
            triggerImage.sprite = defaultNormal;

        if (triggerButton != null)
        {
            triggerButton.onClick.RemoveListener(ToggleDropdown);
            triggerButton.onClick.AddListener(ToggleDropdown);
        }

        if (settingsMenu == null)
            settingsMenu = FindObjectOfType<SettingsMenu>();
    }

    private void OnDestroy()
    {
        if (triggerButton != null)
            triggerButton.onClick.RemoveListener(ToggleDropdown);
    }

    private void ToggleDropdown()
    {
        if (isOpen)
            CloseDropdown();
        else
            OpenDropdown();
    }

    private void OpenDropdown()
    {
        if (optionsPanel == null)
            return;

        optionsPanel.SetActive(true);
        isOpen = true;
    }

    public void CloseDropdown()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        isOpen = false;
    }

    // Вызывается при выборе варианта
    public void SelectOption(Sprite textSprite)
    {
        if (selectedTextImage != null && textSprite != null)
        {
            selectedTextImage.sprite = textSprite;
            selectedTextImage.gameObject.SetActive(true);
        }

        NotifySettingsMenu();
        CloseDropdown();
    }

    // Передача выбранного значения в SettingsMenu
    private void NotifySettingsMenu()
    {
        if (settingsMenu == null)
            settingsMenu = FindObjectOfType<SettingsMenu>();

        if (settingsMenu == null)
        {
            Debug.LogError(
                "SmartDropdown: не найден SettingsMenu!",
                this
            );
            return;
        }

        if (resolutionWidth > 0 && resolutionHeight > 0)
        {
            settingsMenu.OnResolutionSelected(
                resolutionWidth,
                resolutionHeight
            );
        }
        else
        {
            settingsMenu.OnScreenModeSelected(screenMode);
        }
    }

    // Отображение выбранного разрешения
    public void SetResolutionDisplay(int width, int height)
    {
        resolutionWidth = width;
        resolutionHeight = height;

        DropdownOption[] options = GetDropdownOptions();

        foreach (DropdownOption option in options)
        {
            if (option.resolutionWidth == width &&
                option.resolutionHeight == height)
            {
                DisplayOption(option);
                return;
            }
        }

        Debug.LogWarning(
            $"SmartDropdown: не найден вариант разрешения {width}x{height}.",
            this
        );
    }

    // Отображение выбранного режима экрана
    public void SetScreenModeDisplay(FullScreenMode mode)
    {
        screenMode = mode;

        DropdownOption[] options = GetDropdownOptions();

        foreach (DropdownOption option in options)
        {
            bool isScreenModeOption =
                option.resolutionWidth <= 0 &&
                option.resolutionHeight <= 0;

            if (isScreenModeOption && option.screenMode == mode)
            {
                DisplayOption(option);
                return;
            }
        }

        Debug.LogWarning(
            $"SmartDropdown: не найден вариант режима экрана {mode}.",
            this
        );
    }

    // Получение вариантов из панели списка
    private DropdownOption[] GetDropdownOptions()
    {
        if (optionsPanel == null)
            return GetComponentsInChildren<DropdownOption>(true);

        return optionsPanel.GetComponentsInChildren<DropdownOption>(true);
    }

    // Обновление картинки выбранного варианта
    private void DisplayOption(DropdownOption option)
    {
        if (option == null)
            return;

        if (selectedTextImage != null && option.optionNormal != null)
        {
            selectedTextImage.sprite = option.optionNormal;
            selectedTextImage.gameObject.SetActive(true);
        }

        if (option.resolutionWidth > 0 &&
            option.resolutionHeight > 0)
        {
            resolutionWidth = option.resolutionWidth;
            resolutionHeight = option.resolutionHeight;
        }
        else
        {
            screenMode = option.screenMode;
        }
    }
}
