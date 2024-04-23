using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite OnSprite;
    [SerializeField] private Sprite OffSprite;

    public void Switch(bool onFlag)
    {
        image.sprite = onFlag ? OnSprite : OffSprite;
    }
}
