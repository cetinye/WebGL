using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Witmina_rotf
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Animator _transition;
        [SerializeField] private TextMeshProUGUI trialCountText;
        
        private static readonly int Transition1 = Animator.StringToHash("Transition");

        public void Reload()
        {
            GameManager.Instance.Load();
        }

        public void ShowEndGamePanel(bool success)
        {
            _transition.Play("End");
        }

        public void Transition()
        {
            _transition.SetTrigger(Transition1);
        }

        public void PlayStartAnimation()
        {
            _transition.Play("Start");
        }

        public void UpdateTrialCount()
        {
            trialCountText.text = GameManager.Instance.TrialCount.ToString() + "/" + GameManager.Instance.MaxTries;
        }
    }
}

