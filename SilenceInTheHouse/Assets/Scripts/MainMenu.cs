using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Панели")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanelObject;
    private SettingsMenu settingsMenu;

    [Header("Кнопки главного меню")]
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Кнопки окна настроек")]
    public Button settingsBackButton;
    public Button settingsResetButton;
    public Button settingsSaveButton;

    [Header("Панель подтверждения выхода")]
    public GameObject exitConfirmationPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Затемнение")]
    public GameObject fadeOverlay;
    public float fadeDuration = 1.5f;

    [Header("Сцена игры")]
    public string gameSceneName = "LoadingScene"; // ИЗМЕНЕНО: было "MainLevel", теперь загружаем сцену загрузки

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Получаем SettingsMenu с объекта SettingsMenuManager.
        if (settingsPanelObject != null)
        {
            settingsMenu =
                settingsPanelObject.GetComponent<SettingsMenu>();

            if (settingsMenu == null)
            {
                Debug.LogError(
                    "На Settings Panel Object отсутствует SettingsMenu!",
                    settingsPanelObject
                );
            }
        }
        else
        {
            Debug.LogError(
                "В MainMenu не назначен Settings Panel Object!"
            );
        }

        // При запуске показываем главное меню.
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        // Панель настроек берётся из самого SettingsMenu.
        if (settingsMenu != null)
        {
            settingsMenu.CloseSettings();
        }

        // Подключаем кнопки главного меню.
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        // Подключаем кнопки настроек.
        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(ReturnToMainMenu);

        if (settingsResetButton != null)
            settingsResetButton.onClick.AddListener(ResetSettings);

        if (settingsSaveButton != null)
            settingsSaveButton.onClick.AddListener(SaveSettings);

        // Подключаем кнопки подтверждения выхода.
        if (yesButton != null)
            yesButton.onClick.AddListener(OnYesClicked);

        if (noButton != null)
            noButton.onClick.AddListener(OnNoClicked);

        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);

        if (fadeOverlay != null)
            fadeOverlay.SetActive(false);
    }

    private void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayClicked);

        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsClicked);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked);

        if (settingsBackButton != null)
            settingsBackButton.onClick.RemoveListener(ReturnToMainMenu);

        if (settingsResetButton != null)
            settingsResetButton.onClick.RemoveListener(ResetSettings);

        if (settingsSaveButton != null)
            settingsSaveButton.onClick.RemoveListener(SaveSettings);

        if (yesButton != null)
            yesButton.onClick.RemoveListener(OnYesClicked);

        if (noButton != null)
            noButton.onClick.RemoveListener(OnNoClicked);
    }

    // Открытие настроек.
    public void OnSettingsClicked()
    {
        Debug.Log("Нажата кнопка НАСТРОЙКИ!");

        if (settingsMenu == null && settingsPanelObject != null)
            settingsMenu =
                settingsPanelObject.GetComponent<SettingsMenu>();

        if (settingsMenu == null)
        {
            Debug.LogError(
                "Не найден SettingsMenu на SettingsMenuManager!",
                settingsPanelObject
            );
            return;
        }

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        settingsMenu.OpenSettings();
    }

    // Возврат из настроек в главное меню.
    public void ReturnToMainMenu()
    {
        Debug.Log("Возврат в главное меню.");

        if (settingsMenu != null)
            settingsMenu.CloseSettings();

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Сброс настроек.
    public void ResetSettings()
    {
        Debug.Log("Нажата кнопка СБРОСИТЬ.");

        if (settingsMenu == null && settingsPanelObject != null)
            settingsMenu =
                settingsPanelObject.GetComponent<SettingsMenu>();

        if (settingsMenu != null)
            settingsMenu.ResetSettings();
        else
            Debug.LogError("Не удалось сбросить настройки: SettingsMenu не найден.");
    }

    // Сохранение настроек.
    public void SaveSettings()
    {
        Debug.Log("Нажата кнопка СОХРАНИТЬ.");

        if (settingsMenu == null && settingsPanelObject != null)
            settingsMenu =
                settingsPanelObject.GetComponent<SettingsMenu>();

        if (settingsMenu != null)
            settingsMenu.SaveSettings();
        else
            Debug.LogError("Не удалось сохранить настройки: SettingsMenu не найден.");
    }

    // Нажатие кнопки ИГРАТЬ.
    private void OnPlayClicked()
    {
        Debug.Log("Нажата кнопка ИГРАТЬ!");

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMenuMusic();

        if (fadeOverlay != null)
        {
            fadeOverlay.SetActive(true);
            StartCoroutine(FadeToBlack());
        }
        else
        {
            LoadGameScene();
        }
    }

    // Нажатие кнопки ВЫХОД.
    private void OnExitClicked()
    {
        Debug.Log("Нажата кнопка ВЫХОД!");

        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(true);
    }

    // Подтверждение выхода.
    private void OnYesClicked()
    {
        Debug.Log("Выход из игры...");

        SaveSettings();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Отмена выхода.
    private void OnNoClicked()
    {
        Debug.Log("Возврат в меню подтверждения выхода.");

        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);
    }

    // Затемнение экрана перед запуском игры.
    private IEnumerator FadeToBlack()
    {
        Image fadeImage = fadeOverlay.GetComponent<Image>();

        if (fadeImage == null)
        {
            LoadGameScene();
            yield break;
        }

        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            color.a = Mathf.Lerp(
                0f,
                1f,
                elapsed / fadeDuration
            );

            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        yield return new WaitForSeconds(0.5f);

        LoadGameScene();
    }

    // Загрузка игровой сцены.
    private void LoadGameScene()
    {
        Debug.Log("Загрузка сцены: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }
}