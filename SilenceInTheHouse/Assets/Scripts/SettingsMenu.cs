
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsMenu : MonoBehaviour
{
    [Header("Панель настроек")]
    public GameObject settingsPanel;

    [Header("Громкость")]
    public Slider volumeSlider;
    public TMP_Text volumePercentText;

    [Header("Выпадающие списки")]
    public SmartDropdown resolutionDropdown;
    public SmartDropdown screenModeDropdown;

    [Header("Настройки по умолчанию")]
    public int defaultResolutionWidth = 1920;
    public int defaultResolutionHeight = 1080;

    [Range(0f, 1f)]
    public float defaultVolume = 0.5f;

    public FullScreenMode defaultScreenMode =
        FullScreenMode.FullScreenWindow;

    private int resolutionWidth;
    private int resolutionHeight;
    private FullScreenMode screenMode;
    private float volume;

    private const string ResolutionWidthKey = "Settings_ResolutionWidth";
    private const string ResolutionHeightKey = "Settings_ResolutionHeight";
    private const string ScreenModeKey = "Settings_ScreenMode";
    private const string VolumeKey = "Settings_Volume";

    private void Awake()
    {
        if (settingsPanel == null)
            settingsPanel = gameObject;

        LoadSettings();

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;

            volumeSlider.SetValueWithoutNotify(volume);

            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        ApplyVolume();
        UpdateVolumeText();
        ApplyDisplaySettings();
        RefreshDropdownDisplays();

        Debug.Log("Настройки загружены.");
    }

    private void OnEnable()
    {
        RefreshDropdownDisplays();
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }

    private void LoadSettings()
    {
        resolutionWidth = PlayerPrefs.GetInt(
            ResolutionWidthKey,
            defaultResolutionWidth
        );

        resolutionHeight = PlayerPrefs.GetInt(
            ResolutionHeightKey,
            defaultResolutionHeight
        );

        int savedMode = PlayerPrefs.GetInt(
            ScreenModeKey,
            (int)defaultScreenMode
        );

        if (Enum.IsDefined(typeof(FullScreenMode), savedMode))
            screenMode = (FullScreenMode)savedMode;
        else
            screenMode = defaultScreenMode;

        volume = Mathf.Clamp01(
            PlayerPrefs.GetFloat(VolumeKey, defaultVolume)
        );
    }

    // Открытие настроек
    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshDropdownDisplays();

        Debug.Log("Открыты настройки.");
    }

    // Закрытие настроек
    public void CloseSettings()
    {
        SaveSettings();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Debug.Log("Настройки закрыты.");
    }

    // Выбор разрешения через SmartDropdown
    public void OnResolutionSelected(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("Некорректное разрешение.");
            return;
        }

        resolutionWidth = width;
        resolutionHeight = height;

        ApplyDisplaySettings();

        Debug.Log($"Выбрано разрешение: {width}x{height}");
    }

    // Выбор режима экрана через SmartDropdown
    public void OnScreenModeSelected(FullScreenMode mode)
    {
        screenMode = mode;

        ApplyDisplaySettings();

        Debug.Log($"Выбран режим экрана: {mode}");
    }

    // Применение разрешения и режима экрана
    private void ApplyDisplaySettings()
    {
        Screen.SetResolution(
            resolutionWidth,
            resolutionHeight,
            screenMode
        );
    }

    // Изменение громкости
    private void OnVolumeChanged(float value)
    {
        volume = Mathf.Clamp01(value);

        ApplyVolume();
        UpdateVolumeText();
    }

    private void ApplyVolume()
    {
        AudioListener.volume = volume;
    }

    // Обновление текста процентов в реальном времени
    private void UpdateVolumeText()
    {
        if (volumePercentText != null)
        {
            int percent = Mathf.RoundToInt(volume * 100f);
            volumePercentText.text = percent + "%";
        }
    }

    // Обновление отображения выбранных вариантов
    public void RefreshDropdownDisplays()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.SetResolutionDisplay(
                resolutionWidth,
                resolutionHeight
            );
        }

        if (screenModeDropdown != null)
        {
            screenModeDropdown.SetScreenModeDisplay(screenMode);
        }
    }

    // Сохранение настроек
    public void SaveSettings()
    {
        PlayerPrefs.SetInt(
            ResolutionWidthKey,
            resolutionWidth
        );

        PlayerPrefs.SetInt(
            ResolutionHeightKey,
            resolutionHeight
        );

        PlayerPrefs.SetInt(
            ScreenModeKey,
            (int)screenMode
        );

        PlayerPrefs.SetFloat(
            VolumeKey,
            volume
        );

        PlayerPrefs.Save();

        Debug.Log("Настройки сохранены.");
    }

    // Сброс настроек
    public void ResetSettings()
    {
        resolutionWidth = defaultResolutionWidth;
        resolutionHeight = defaultResolutionHeight;
        screenMode = defaultScreenMode;
        volume = defaultVolume;

        if (volumeSlider != null)
            volumeSlider.SetValueWithoutNotify(volume);

        ApplyVolume();
        UpdateVolumeText();
        ApplyDisplaySettings();
        RefreshDropdownDisplays();

        SaveSettings();

        Debug.Log("Настройки сброшены до значений по умолчанию.");
    }
}
