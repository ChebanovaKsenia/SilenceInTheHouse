using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Панели")]
    public GameObject settingsPanelObject;
    private SettingsMenu settingsMenu;

    [Header("Кнопки главного меню")]
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Панель подтверждения выхода")]
    public GameObject exitConfirmationPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Затемнение")]
    public GameObject fadeOverlay;
    public float fadeDuration = 1.5f;

    [Header("Сцена игры")]
    public string gameSceneName = "MainLevel";

    void Start()
    {
        // РАЗБЛОКИРУЕМ КУРСОР ПРИ ЗАГРУЗКЕ МЕНЮ
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Инициализация меню настроек
        if (settingsPanelObject != null)
        {
            settingsMenu = settingsPanelObject.GetComponent<SettingsMenu>();
            settingsPanelObject.SetActive(false);
        }
        // Инициализация меню настроек
        if (settingsPanelObject != null)
        {
            settingsMenu = settingsPanelObject.GetComponent<SettingsMenu>();
            settingsPanelObject.SetActive(false);
        }

        // Назначаем слушатели нажатий
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);

        if (exitConfirmationPanel != null) exitConfirmationPanel.SetActive(false);
        if (fadeOverlay != null) fadeOverlay.SetActive(false);
    }

    void OnPlayClicked()
    {
        Debug.Log("Нажата кнопка ИГРАТЬ!");

        // Останавливаем музыку меню
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMenuMusic();
        }

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

    void OnSettingsClicked()
    {
        Debug.Log("Нажата кнопка НАСТРОЙКИ!");

        if (settingsMenu == null)
        {
            settingsMenu = FindObjectOfType<SettingsMenu>();
        }

        if (settingsMenu != null)
        {
            settingsMenu.OpenSettings();
        }
    }

    void OnExitClicked()
    {
        Debug.Log("Нажата кнопка ВЫХОД!");
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(true);
    }

    void OnYesClicked()
    {
        Debug.Log("Выход из игры...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnNoClicked()
    {
        Debug.Log("Возврат в меню...");
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);
    }

    System.Collections.IEnumerator FadeToBlack()
    {
        Image fadeImage = fadeOverlay.GetComponent<Image>();
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        yield return new WaitForSeconds(0.5f);
        LoadGameScene();
    }

    void LoadGameScene()
    {
        Debug.Log("Загрузка сцены: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }
}