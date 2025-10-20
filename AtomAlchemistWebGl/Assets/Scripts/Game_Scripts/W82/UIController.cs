using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_AtomAlchemist
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private LevelBehaviour levelBehaviour;
        [SerializeField] private InfoPanel infoPanel;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private List<TMP_Text> elementTexts;
        [SerializeField] private List<RectTransform> elementRecttransforms;
        [SerializeField] private TMP_Text targetElementText;
        [SerializeField] private RectTransform _endGamePanel;
        [SerializeField] private Image arrowImage;
        [SerializeField] private Sprite arrowUp;
        [SerializeField] private Sprite arrowDown;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private float scrollMoveTime;

        private int fontSize = 50;
        private Color fontColor = new Color(0.2235294f, 0.8352941f, 0.3137255f);

        private Tween _elementTween;

        public void Initialize()
        {
            _endGamePanel.gameObject.SetActive(false);
        }

        public void SetTimerText(float timer)
        {
            _timerText.text = timer.ToString("F0");
        }

        public TMP_Text GetTimerText()
        {
            return _timerText;
        }

        public void SetCurrentElement(Element current, bool punch = false)
        {
            ResetTextSizeFonts(elementTexts[(int)current - 1]);
            ResetTextSizeFonts(elementTexts[(int)current + 1]);

            ScrollTo(elementRecttransforms[(int)current], elementTexts[(int)current]);

            if (!punch)
                return;

            _elementTween.Kill();
            _elementTween = elementTexts[2].transform.DOPunchScale(.18f * Vector3.one, 0.2f);
        }

        public void SetTargetElement(string elementText)
        {
            targetElementText.text = elementText;

            infoPanel.SetTargetElement((ElementName)levelBehaviour.GetTargetElementIndex(), (Element)levelBehaviour.GetTargetElementIndex(), levelBehaviour.isArrowUp);
        }

        public void ActivateEndgamePanel()
        {
            _endGamePanel.gameObject.SetActive(true);
        }

        public void RotateArrow(bool isUp)
        {
            if (isUp)
                arrowImage.sprite = arrowUp;
            else
                arrowImage.sprite = arrowDown;
        }

        public void ScrollTo(RectTransform target, TMP_Text targetText)
        {
            Canvas.ForceUpdateCanvases();
            Vector2 viewPortLocalPosition = scrollRect.viewport.localPosition;
            Vector2 targetLocalPosition = target.localPosition;

            Vector2 newTargetLocalPositon = new Vector2(
                0 - (viewPortLocalPosition.x + targetLocalPosition.x),
                0 - (viewPortLocalPosition.y + targetLocalPosition.y)
            );

            contentPanel.DOLocalMove(newTargetLocalPositon, scrollMoveTime);
            targetText.DOColor(fontColor, scrollMoveTime);
            targetText.DOFontSize(fontSize, scrollMoveTime);
        }

        private void ResetTextSizeFonts(TMP_Text tmpText)
        {
            tmpText.DOFontSize(30, scrollMoveTime);
            tmpText.DOColor(Color.white, scrollMoveTime);
        }
    }
}

