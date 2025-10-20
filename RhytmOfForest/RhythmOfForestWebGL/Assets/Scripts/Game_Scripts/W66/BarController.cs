using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_rotf
{
    public class BarController : MonoBehaviour
    {
        [SerializeField] private RectTransform _barPanel;
        [SerializeField] private RectTransform _mushroomsParent;
        [SerializeField] private RectTransform _linesParent;
        [SerializeField] private RectTransform _timerLine;
        [SerializeField] private Image _fill;
        [SerializeField] private RectTransform _linePrefab;
        [SerializeField] private RectTransform _greyMushroomPrefab;
        [SerializeField] private List<RectTransform> _coloredMushroomPrefabs;

        private List<RectTransform> _lines = new();
        private List<RectTransform> _greyMushrooms = new();
        private List<RectTransform> _coloredMushrooms = new();

        private float _totalTime = 2f;

        private bool _active;

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                _barPanel.gameObject.SetActive(_active);
            }
        }

        public void Clear()
        {
            _timerLine.anchoredPosition = Vector2.zero;
            _timerLine.gameObject.SetActive(false);
            
            for (int i = 0; i < _lines.Count; i++)
            {
                Destroy(_lines[i].gameObject);
            }
            _lines.Clear();
            for (int i = 0; i < _greyMushrooms.Count; i++)
            {
                Destroy(_greyMushrooms[i].gameObject);
            }
            _greyMushrooms.Clear();
            for (int i = 0; i < _coloredMushrooms.Count; i++)
            {
                Destroy(_coloredMushrooms[i].gameObject);
            }
            _coloredMushrooms.Clear();
        }

        public void SetBar(List<SequenceElement> sequence)
        {
            _totalTime = sequence.Last().Time;

            foreach (var element in sequence)
            {
                var perc = element.Time / _totalTime;
                var pos = _greyMushroomPrefab.anchoredPosition;
                pos.x = perc * _mushroomsParent.rect.width;
                var g = Instantiate(_greyMushroomPrefab, _mushroomsParent);
                g.anchoredPosition = pos;
                _greyMushrooms.Add(g);
                
                var m = Instantiate(_coloredMushroomPrefabs[element.Index], _mushroomsParent);
                m.anchoredPosition = pos;
                _coloredMushrooms.Add(m);
                m.gameObject.SetActive(false);
            }
        }

        public void ActivateMushroom(int id, bool punchScale = true)
        {
            var mushroom = _coloredMushrooms[id];
            mushroom.gameObject.SetActive(true);

            if (!punchScale)
                return;
            
            mushroom.DOKill();
            mushroom.DOPunchScale(0.1f*Vector3.one, 0.2f);
        }

        public void DisableMushrooms()
        {
            foreach (var m in _coloredMushrooms)
            {
                m.DOKill();
                m.gameObject.SetActive(false);
            }
        }

        public void AddLine(float time)
        {
            var perc = Mathf.Clamp(time / _totalTime, 0f, 1.05f);
            var pos = perc * _mushroomsParent.rect.width * Vector2.right;
            var line = Instantiate(_linePrefab, _linesParent);
            line.anchoredPosition = pos;
            _lines.Add(line);
        }

        public void SetTimerLine(float time)
        {
            var perc = Mathf.Clamp(time / _totalTime, 0f, 1.05f);
            var pos = perc * _mushroomsParent.rect.width * Vector2.right;

            _timerLine.anchoredPosition = pos;
        }

        public void ToggleTimerLine(bool active)
        {
            _timerLine.gameObject.SetActive(active);
        }
    }
}

