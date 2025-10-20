using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Name_It_Or_Run_It
{
    public class LetterWindow : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private LevelManager levelManager;
        private static bool isClickable = true;
        [SerializeField] private TMP_Text letter;
        [SerializeField] private float animTime;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isClickable)
            {
                isClickable = false;
                AudioManager.instance.PlayOneShot("Tap");
                Taptic.Light();
                TappedAnim().OnComplete(() => Reset());
                levelManager.CloseLetters(this);
                levelManager.CheckAnswer(letter.text);
            }

        }

        public void SetLetter(string letterText)
        {
            letter.text = letterText;
        }

        public void SetClickable(bool state)
        {
            isClickable = state;
        }

        private void Reset()
        {
            letter.DOFade(1f, 0.01f);
            letter.transform.DOScale(1f, 0.01f);
            this.SetLetter("");
        }

        private Sequence TappedAnim()
        {
            Sequence tapAnim = DOTween.Sequence();

            tapAnim.Append(letter.DOFade(0f, animTime));
            tapAnim.Join(letter.transform.DOScale(2f, animTime));

            return tapAnim;
        }
    }
}