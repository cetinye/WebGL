using System;
using DG.Tweening;
using UnityEngine;

namespace Witmina_MarineManagement
{
    public class UIServiceElement : MonoBehaviour
    {
        public static event Action<RequestType> ServiceSelected;

        [SerializeField] private RequestType _type;
        public RequestType RequestType => _type;

        private bool _selected;

        private Tween _punchTween;

        private Vector3 initialScale;

        private void Start()
        {
            initialScale = transform.localScale;
        }

        private void OnDestroy()
        {
            _punchTween.Kill();
        }

        public void OnClick()
        {
            ServiceSelected?.Invoke(_type);
            _punchTween.Kill();
            transform.localScale = initialScale;
            _punchTween = transform.DOPunchScale(-0.15f * Vector3.one, 0.4f, 2, 0.5f);

            Taptic.Success();
        }
    }
}

