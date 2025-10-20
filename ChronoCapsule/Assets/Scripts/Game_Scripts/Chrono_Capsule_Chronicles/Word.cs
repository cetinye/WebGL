using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chrono_Capsule_Chronicles
{
    public class Word : MonoBehaviour, IPointerClickHandler
    {
        private static bool isClickable = true;
        [SerializeField] private RectTransform parentRect;
        [SerializeField] private RectTransform textRect;
        [SerializeField] private TMP_Text wordText;

        [SerializeField] private Image feedback;
        [SerializeField] private Sprite selectSprite;
        [SerializeField] private Sprite correctSprite;
        [SerializeField] private Sprite wrongSprite;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isClickable)
            {
                isClickable = false;
                feedback.sprite = selectSprite;
                feedback.enabled = true;
                AudioManager.instance.PlayOneShot(SoundType.Tap);
                LevelManager.instance.SetClickedWord(this);
                LevelManager.instance.GiveFeedback(wordText.text);
            }
        }

        public void SetWordText(string word)
        {
            wordText.text = word;

            ResizeText();
        }

        public void SetFeedback(bool isCorrect)
        {
            if (isCorrect)
                feedback.sprite = correctSprite;
            else
                feedback.sprite = wrongSprite;
        }

        public void SetClickable(bool state)
        {
            isClickable = state;
        }

        private void ResizeText()
        {
            textRect.anchoredPosition = parentRect.anchoredPosition;
            textRect.sizeDelta = parentRect.sizeDelta;
        }
    }
}