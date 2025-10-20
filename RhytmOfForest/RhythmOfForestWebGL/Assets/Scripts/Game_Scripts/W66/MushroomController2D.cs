using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_rotf
{
    public class MushroomController2D : MonoBehaviour
    {
        public event Action<int> MushroomPressed;

        [SerializeField] private List<Mushroom2D> _mushrooms;
        private List<SequenceElement> _sequence;

        private Camera _camera;
        private bool isPressable;

        private void Awake()
        {
            _camera = Camera.main;
            foreach (var m in _mushrooms)
            {
                m.Pressed += OnPressed;
            }
        }

        private void OnDestroy()
        {
            foreach (var m in _mushrooms)
            {
                m.Pressed -= OnPressed;
            }
        }

        private void OnPressed(Mushroom2D mushroom)
        {
            if (!isPressable) return;

            var index = _mushrooms.IndexOf(mushroom);
            if (index == -1)
                return;

            MushroomPressed?.Invoke(index);
        }

        public void SetPressable(bool state)
        {
            isPressable = state;
        }

        public void PlayMushroom(int index)
        {
            _mushrooms[index].LightUp();
            AudioController.Instance.PlayMushroomSound(index);
        }
    }
}

