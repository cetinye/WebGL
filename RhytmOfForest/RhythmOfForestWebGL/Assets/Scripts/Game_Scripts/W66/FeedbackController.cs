using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_rotf
{
    public class FeedbackController : MonoBehaviour
    {
        [SerializeField] private LevelBehaviour _levelBehaviour;
        [SerializeField] private TMP_Text _goText;
        [SerializeField] private TMP_Text _yourTurn;

        private Sequence _showSequence;
        private Sequence _yourTurnSequence;

        public void Initialize()
        {
            _goText.gameObject.SetActive(false);
            _yourTurn.gameObject.SetActive(false);
        }

        public void ShowFeedback(string text, float duration)
        {
            _showSequence.Kill();

            _goText.text = text;
            _goText.gameObject.SetActive(true);
            _goText.color = new Color(1f, 1f, 1f, 0.1f);
            _showSequence = DOTween.Sequence().SetEase(Ease.Linear)
                .OnComplete(() => _goText.gameObject.SetActive(false));

            _showSequence.Append(_goText.DOFade(1f, 0.4f * duration)
                .SetEase(Ease.OutQuad));
            _showSequence.Insert(0.6f * duration, _goText.DOFade(0.1f, duration))
                .SetEase(Ease.InQuad);
            _showSequence.Play();
        }

        public void ShowYourTurn(float duration)
        {
            _yourTurnSequence.Kill();

            _yourTurn.gameObject.SetActive(true);
            _yourTurn.transform.localScale = Vector3.one;
            _yourTurn.color = new Color(255f, 255f, 255f, 0f);

            _yourTurnSequence = DOTween.Sequence().SetEase(Ease.Linear);

            _yourTurnSequence.Append(_yourTurn.DOFade(1f, 0.4f * duration))
                .SetEase(Ease.OutQuad);
            _yourTurnSequence.Insert(0.6f * duration, _yourTurn.DOFade(0.2f, 0.4f * duration))
                .SetEase(Ease.InQuad);
            _yourTurnSequence.OnComplete(() =>
            {
                _yourTurn.gameObject.SetActive(false);
                _levelBehaviour.ResetCountdown();
                _levelBehaviour.isCountdownOn = true;
                _levelBehaviour.SetPressable(true);
            });
            _yourTurnSequence.Play();

        }
    }

}
