using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class DropdownOption : MonoBehaviour
{
    [Header("Спрайты этого варианта")]
    public Sprite optionNormal;   // Обычный спрайт
    public Sprite optionHover;    // Спрайт при наведении

    [Header("Данные для SettingsMenu")]
    public int resolutionWidth = 0;
    public int resolutionHeight = 0;
    public FullScreenMode screenMode = FullScreenMode.Windowed;

    [Header("Ссылка на родительский триггер")]
    public SmartDropdown parentDropdown;

    private Image optionImage;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        optionImage = GetComponent<Image>();

        if (optionImage != null && optionNormal != null)
        {
            optionImage.sprite = optionNormal;
        }

        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        Debug.Log("Клик на вариант: " + gameObject.name);

        if (parentDropdown != null && optionNormal != null)
        {
            // Передаём данные о разрешении/режиме в родительский dropdown
            if (resolutionWidth > 0 && resolutionHeight > 0)
            {
                parentDropdown.resolutionWidth = resolutionWidth;
                parentDropdown.resolutionHeight = resolutionHeight;
            }
            else
            {
                parentDropdown.screenMode = screenMode;
            }

            parentDropdown.SelectOption(optionNormal);
        }
        else
        {
            if (parentDropdown == null)
                Debug.LogError("parentDropdown не назначен в " + gameObject.name);
            if (optionNormal == null)
                Debug.LogError("optionNormal не назначен в " + gameObject.name);
        }
    }
}