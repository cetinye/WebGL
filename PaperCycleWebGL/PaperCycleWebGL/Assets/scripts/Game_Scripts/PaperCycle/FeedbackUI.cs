using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_PaperCycle
{
    public class FeedbackUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _feedbackParent;
        [SerializeField] private Image _feedbackImage;
        [SerializeField] private Animator _feedbackAnimator;
        [SerializeField] private Sprite _successSprite;
        [SerializeField] private Sprite _failSprite;

        private RectTransform _rectTransform;
        private Coroutine _feedbackRoutine;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void GiveFeedback(HouseAlignment alignment, bool success, float delay = 0f)
        {
            if (_feedbackRoutine != null)
                StopCoroutine(_feedbackRoutine);

            _feedbackRoutine = StartCoroutine(FeedbackRoutine(alignment, success, delay));
        }

        private void ActivateFeedback(HouseAlignment alignment, bool success)
        {
            var sidePos = _rectTransform.rect.width / 2;
            
            switch (alignment)
            {
                case HouseAlignment.Left:
                    _feedbackParent.anchoredPosition = sidePos * Vector2.left;
                    break;
                case HouseAlignment.Right:
                    _feedbackParent.anchoredPosition = sidePos * Vector2.right;
                    break;
                case HouseAlignment.Neither:
                    _feedbackParent.anchoredPosition = Vector2.zero;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
            }

            _feedbackImage.sprite = success ? _successSprite : _failSprite;
            
            _feedbackAnimator.Play("Feedback");
        }

        private IEnumerator FeedbackRoutine(HouseAlignment alignment, bool success, float delay)
        {
            yield return new WaitForSeconds(delay);
            ActivateFeedback(alignment, success);
        }
    }
}