using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_PaperCycle
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject _introPanel;
        [SerializeField] private GameObject _endGamePanel;
        [SerializeField] private GameObject _skipButton;
        
        public void Initialize()
        {
            _endGamePanel.gameObject.SetActive(false);
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
    }

}
