using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using W56;

public class W56_Bin : MonoBehaviour
{
    public MATERIAL_TYPE type;
    public Image image;
    public TMP_Text tm_Text;
    public RectTransform rectTransform;
    public int posIndex;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetSprite(Sprite sprite)
    {
        image.DOFade(0f, 0f);
        image.sprite = sprite;
        tm_Text.text = LeanLocalization.GetTranslationText(type.ToString());
        image.SetNativeSize();
        image.DOFade(1f, 0.25f);
    }

    public void SetRecttransform(RectTransform target)
    {
        rectTransform.anchoredPosition3D = target.anchoredPosition3D;
    }

    public int GetPosIndex()
    {
        return posIndex;
    }

    public void SetPosIndex(int index)
    {
        posIndex = index;
    }

    public Tween MoveTo(RectTransform target, float tweenTime)
    {
        return rectTransform.DOAnchorPos3D(target.anchoredPosition, tweenTime);
    }
}