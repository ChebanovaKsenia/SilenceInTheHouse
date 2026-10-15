using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class SpriteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Спрайты")]
    public Sprite normalSprite;      // Обычный спрайт
    public Sprite hoverSprite;       // При наведении
    public Sprite pressedSprite;     // При нажатии (опционально)

    private Image buttonImage;
    private Button button;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();

        // Устанавливаем начальный спрайт
        if (normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // При наведении мыши
        if (hoverSprite != null && button.interactable)
        {
            buttonImage.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Когда мышь уходит
        if (normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // При нажатии кнопки мыши
        if (pressedSprite != null && button.interactable)
        {
            buttonImage.sprite = pressedSprite;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Когда отпускаем кнопку мыши
        if (hoverSprite != null && button.interactable)
        {
            buttonImage.sprite = hoverSprite;
        }
        else if (normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }
}