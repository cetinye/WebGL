using System.Collections;
using System.Collections.Generic;
using Lean.Localization;
using TMPro;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject _introPanel;
        [SerializeField] private GameObject _endGamePanel;
        [SerializeField] private GameObject _skipButton;
        [SerializeField] private Animator _transitionAnimator;
        [SerializeField] private TMP_Text timer;
        [SerializeField] private TMP_Text wrong;
        [SerializeField] private TMP_Text correct;
        [SerializeField] private TMP_Text level;

        public void Initialize()
        {
            _endGamePanel.SetActive(false);
        }

        public void UpdateTime(float time)
        {
            timer.text = time.ToString();
        }

        public void UpdateWrongCount(int count)
        {
            wrong.text = count.ToString();
        }

        public void UpdateCorrectCount(int count)
        {
            correct.text = count.ToString();
        }

        public void UpdateLevel(int level)
        {
            this.level.text = $"{LeanLocalization.GetTranslationText("Level")} " + level.ToString();
        }

        public void ActivateEndGamePanel()
        {
            _endGamePanel.SetActive(true);
        }

        public void ToggleIntroPanel(bool active)
        {
            _introPanel.SetActive(active);
        }

        public void ToggleSkipButton(bool active)
        {
            _skipButton.SetActive(active);
        }

        public void PlayScreenTransitionIntro()
        {
            _transitionAnimator.gameObject.SetActive(true);
            _transitionAnimator.Play("Intro");
        }
        public void PlayScreenTransitionOutro()
        {
            _transitionAnimator.gameObject.SetActive(true);
            _transitionAnimator.Play("Outro");
        }
    }
}