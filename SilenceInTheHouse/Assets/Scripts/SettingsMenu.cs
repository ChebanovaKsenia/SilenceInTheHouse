using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Панели")]
    public GameObject settingsPanel;
    public GameObject mainMenuButtons;

    [Header("Выпадающие списки")]
    public SmartDropdown resolutionDropdown;  // Ссылка на скрипт выпадающего списка разрешения
    public SmartDropdown screenModeDropdown;  // Ссылка на скрипт выпадающего списка режима экрана

    [Header("Громкость музыки")]
    public Slider musicSlider;

    [Header("Громкость звуков (SFX)")]
    public Slider sfxSlider;

    [Header("Кнопки управления")]
    public Button saveButton;
    public Button resetButton;
    public Button backButton;

    private int selectedWidth = 1920;
    private int selectedHeight = 1080;
    private FullScreenMode selectedMode = FullScreenMode.Windowed;

    void Start()
    {
        if (saveButton != null) saveButton.onClick.AddListener(SaveSettings);
        if (resetButton != null) resetButton.onClick.AddListener(ResetSettings);
        if (backButton != null) backButton.onClick.AddListener(GoBack);

        LoadSettings();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        if (mainMenuButtons != null) mainMenuButtons.SetActive(false);

        // Обновляем отображение текущих настроек
        UpdateDropdownsDisplay();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (mainMenuButtons != null) mainMenuButtons.SetActive(true);
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("ScreenWidth", selectedWidth);
        PlayerPrefs.SetInt("ScreenHeight", selectedHeight);
        PlayerPrefs.SetInt("ScreenMode", (int)selectedMode);

        if (musicSlider != null) PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        if (sfxSlider != null) PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);

        PlayerPrefs.Save();

        // Применяем настройки
        ApplySettings();

        Debug.Log("Настройки сохранены!");
    }

    void ApplySettings()
    {
        // Применяем разрешение и режим экрана
        Screen.SetResolution(selectedWidth, selectedHeight, selectedMode);

        // Применяем громкость
        if (AudioManager.Instance != null)
        {
            if (musicSlider != null) AudioManager.Instance.SetMusicVolume(musicSlider.value);
            if (sfxSlider != null) AudioManager.Instance.SetSFXVolume(sfxSlider.value);
        }
    }

    void ResetSettings()
    {
        if (musicSlider != null) musicSlider.value = 0.5f;
        if (sfxSlider != null) sfxSlider.value = 0.5f;

        selectedWidth = 1920;
        selectedHeight = 1080;
        selectedMode = FullScreenMode.Windowed;

        ApplySettings();
        UpdateDropdownsDisplay();

        Debug.Log("Настройки сброшены!");
    }

    void GoBack() => CloseSettings();

    void LoadSettings()
    {
        // Загружаем сохранённые настройки
        selectedWidth = PlayerPrefs.GetInt("ScreenWidth", Screen.currentResolution.width);
        selectedHeight = PlayerPrefs.GetInt("ScreenHeight", Screen.currentResolution.height);
        selectedMode = (FullScreenMode)PlayerPrefs.GetInt("ScreenMode", (int)Screen.fullScreenMode);

        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        if (musicSlider != null) musicSlider.value = savedMusicVolume;
        if (sfxSlider != null) sfxSlider.value = savedSFXVolume;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(savedMusicVolume);
            AudioManager.Instance.SetSFXVolume(savedSFXVolume);
        }

        // Обновляем отображение
        UpdateDropdownsDisplay();
    }

    void UpdateDropdownsDisplay()
    {
        // Обновляем текст на кнопках выпадающих списков
        if (resolutionDropdown != null)
        {
            string resolutionText = selectedWidth + " x " + selectedHeight;
            Debug.Log("Обновляем разрешение: " + resolutionText);
            // Здесь нужно вызвать метод SmartDropdown для обновления текста
            // Но так как у нас спрайты, нужно найти соответствующий спрайт
        }

        if (screenModeDropdown != null)
        {
            string modeText = GetScreenModeText(selectedMode);
            Debug.Log("Обновляем режим экрана: " + modeText);
        }
    }

    string GetScreenModeText(FullScreenMode mode)
    {
        switch (mode)
        {
            case FullScreenMode.FullScreenWindow:
                return "Полноэкранный";
            case FullScreenMode.MaximizedWindow:
                return "Окно без рамки";
            case FullScreenMode.Windowed:
            default:
                return "Оконный";
        }
    }

    // Методы для вызова из SmartDropdown при выборе варианта
    public void OnResolutionSelected(int width, int height)
    {
        selectedWidth = width;
        selectedHeight = height;
        Debug.Log("Выбрано разрешение: " + width + "x" + height);
    }

    public void OnScreenModeSelected(FullScreenMode mode)
    {
        selectedMode = mode;
        Debug.Log("Выбран режим экрана: " + mode);
    }
}