using System.Collections;
using System.Collections.Generic;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Witmina_GarbageHunt
{
    public class FeedbackPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _comboText;
        [SerializeField] private RectTransform _fishPanel;
        [SerializeField] private float _duration = 1.5f;

        public float Duration => _duration;

        public void DisplayCombo(int combo)
        {
            _fishPanel.gameObject.SetActive(false);
            _comboText.gameObject.SetActive(true);
            _comboText.text = $"{LeanLocalization.GetTranslationText("Combo")}\n{combo}X";
        }

        public void DisplayFishPanel()
        {
            _comboText.gameObject.SetActive(false);
            _fishPanel.gameObject.SetActive(true);
        }

        public void Disable()
        {
            _comboText.gameObject.SetActive(false);
            _fishPanel.gameObject.SetActive(false);
        }
    }
}

