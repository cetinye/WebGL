using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Chefs_Secret_Recipes
{
    public class Answer : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private TMP_Text answerText;
        [SerializeField] private Image answerImage;
        private bool isCorrect;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (GameStateManager.GetGameState() != GameState.Question) return;

            AudioManager.instance.PlayAt(SoundType.Select, 0.3f);
            SelectAnimation();
            isCorrect = LevelManager.instance.CheckAnswer(int.Parse(answerText.text));
            ColorBG();
        }

        public void SetText(int val)
        {
            answerText.text = val.ToString("F0");
        }

        public int GetTextVal()
        {
            return int.Parse(answerText.text);
        }

        public void SetImage(Sprite val)
        {
            answerImage.sprite = val;
            answerText.enabled = false;
            answerImage.enabled = true;
        }

        private void SelectAnimation()
        {
            Vector3 defScale = transform.localScale;
            transform.DOScale(transform.localScale * 1.2f, 0.25f);
        }

        public void ColorBG()
        {
            if (isCorrect)
                bgImage.DOColor(Color.green, 1f);
            else
                bgImage.DOColor(Color.red, 1f);
        }
    }
}