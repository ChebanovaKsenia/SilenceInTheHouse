
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderController : MonoBehaviour
{
    public Sprite bgInactive;
    public Sprite bgActive;
    public Sprite fillInactive;
    public Sprite fillActive;
    public Sprite handleNormal;
    public Sprite handleActive;

    private Image bgImage;
    private Image fillImage;
    private Image handleImage;

    private void Awake()
    {
        Transform bgTransform = transform.Find("Background");
        Transform fillTransform = transform.Find("Fill Area/Fill");
        Transform handleTransform = transform.Find("Handle Slide Area/Handle");

        if (bgTransform != null)
            bgImage = bgTransform.GetComponent<Image>();

        if (fillTransform != null)
            fillImage = fillTransform.GetComponent<Image>();

        if (handleTransform != null)
            handleImage = handleTransform.GetComponent<Image>();
    }

    public void OnHoverEnter()
    {
        if (bgImage != null && bgActive != null)
            bgImage.sprite = bgActive;

        if (fillImage != null && fillActive != null)
            fillImage.sprite = fillActive;

        if (handleImage != null && handleActive != null)
            handleImage.sprite = handleActive;
    }

    public void OnHoverExit()
    {
        if (bgImage != null && bgInactive != null)
            bgImage.sprite = bgInactive;

        if (fillImage != null && fillInactive != null)
            fillImage.sprite = fillInactive;

        if (handleImage != null && handleNormal != null)
            handleImage.sprite = handleNormal;
    }
}
