using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Witmina_AtomAlchemist
{
    public class Orbital : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _haloSprite;
        [SerializeField] private float _radius = 1f;
        [SerializeField] private int _capacity = 2;
        public float rotateSpeed = 1f;

        private List<Electron> _electrons = new();

        public bool Full => _electrons.Count >= _capacity;
        public bool Occupied => _electrons.Count > 0;

        public bool AddElectron(Electron electron, bool disableAnimation = false)
        {
            if (Full)
                return false;

            electron.Stop();
            electron.transform.parent = transform;
            _electrons.Add(electron);

            _haloSprite.gameObject.SetActive(true);

            var count = _electrons.Count;
            for (int i = 0; i < count; i++)
            {
                var angle = i * 360f / count;
                var basePosition = _radius * Vector3.up;
                _electrons[i].transform.localPosition
                    = Quaternion.AngleAxis(angle, Vector3.forward) * basePosition;
                _electrons[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            }

            if (disableAnimation)
                return true;

            electron.transform.localScale = 0.2f * Vector3.one;
            electron.transform.DOScale(Vector3.one, 0.2f);

            return true;
        }

        public bool RemoveElectron(Electron electron)
        {
            if (!_electrons.Remove(electron))
                return false;

            if (_electrons.Count < 1)
                _haloSprite.gameObject.SetActive(false);

            electron.Pop();
            return true;
        }

        public void SetColor(Color color)
        {
            _haloSprite.color = color;
        }

        private void FixedUpdate()
        {
            transform.Rotate(Vector3.forward, rotateSpeed * 90f * Time.deltaTime);
        }

        public void Clear()
        {
            _haloSprite.gameObject.SetActive(false);
            for (int i = 0; i < _electrons.Count; i++)
            {
                Destroy(_electrons[i].gameObject);
            }

            _electrons.Clear();
        }
    }
}