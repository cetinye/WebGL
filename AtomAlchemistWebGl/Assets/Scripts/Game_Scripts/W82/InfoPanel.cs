using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Witmina_AtomAlchemist;
using Lean.Localization;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private LevelBehaviour levelBehaviour;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text title;
    [SerializeField] private GameObject blockPanel;
    [SerializeField] private float showTime;
    [SerializeField] private TMP_Text targetElementText;
    [SerializeField] private TMP_Text elementText;
    [SerializeField] private TMP_Text explanationText;
    [SerializeField] private Button okButton;
    [SerializeField] private Image arrow;
    [SerializeField] private Sprite upArrow;
    [SerializeField] private Sprite downArrow;

    public void SetTargetElement(ElementName elementName, Element element, bool isArrowUp)
    {
        levelBehaviour.SetTimer(false);
        okButton.interactable = true;
        blockPanel.SetActive(true);
        targetElementText.text = LeanLocalization.GetTranslationText("TargetElement") + ": " + elementName.ToString() + ".";
        elementText.text = element.ToString();

        if (isArrowUp)
        {
            arrow.sprite = upArrow;
            explanationText.text = LeanLocalization.GetTranslationText("AimAtNucleus");
        }
        else
        {
            arrow.sprite = downArrow;
            explanationText.text = LeanLocalization.GetTranslationText("AimAtElectrons");
        }

        FadePanel(1, showTime);
    }

    private void FadePanel(int alpha, float time)
    {
        Sequence showSeq = DOTween.Sequence();

        showSeq.Append(background.DOFade(alpha, time));
        showSeq.Join(title.DOFade(alpha, time));
        showSeq.Join(targetElementText.DOFade(alpha, time));
        showSeq.Join(elementText.DOFade(alpha, time));
        showSeq.Join(explanationText.DOFade(alpha, time));
        showSeq.Join(arrow.DOFade(alpha, time));

        if (time > 0.2f)
            showSeq.Play().OnComplete(() => okButton.gameObject.SetActive(true));
        else
            showSeq.Play();

        AudioController.instance.PlayOneShot("InfoPanel");
    }

    public void CloseInfoPanel()
    {
        AudioController.instance.PlayOneShot("OKButton");
        okButton.interactable = false;
        FadePanel(0, 0f);
        blockPanel.SetActive(false);
        levelBehaviour.SetTimer(true);
    }
}
