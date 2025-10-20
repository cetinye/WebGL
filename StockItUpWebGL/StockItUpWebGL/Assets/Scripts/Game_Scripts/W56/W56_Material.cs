using UnityEngine;
using UnityEngine.UI;
using W56;

public class W56_Material : MonoBehaviour
{
    public MATERIAL_TYPE type;
    public Image image;

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
        image.SetNativeSize();
    }
}