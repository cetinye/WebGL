using DG.Tweening;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    [RequireComponent(typeof(Renderer))]
    public class BlurController : MonoBehaviour
    {
        [SerializeField] private Color _endColor;
        [SerializeField] private float _duration = 0.1f;

        private Renderer _renderer;
        private Color _startColor;

        private Tweener _blurTween;

        private float _h1, _s1, _v1;
        private float _h2, _s2, _v2;
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _startColor = _renderer.material.color;
            
            Color.RGBToHSV(_startColor, out _h1, out _s1, out _v1);
            Color.RGBToHSV(_endColor, out _h2, out _s2, out _v2);
        }

        private void OnDestroy()
        {
            _blurTween.Kill();
        }

        public void ResetAmount()
        {
            _blurTween.Kill();
            _renderer.material.SetColor("_Color", _startColor);
        }

        public void SetAmount(float percentage)
        {
            percentage = Mathf.Clamp(percentage, 0f, 1f);
            var h3 = Mathf.Lerp(_h1, _h2, percentage);
            var s3 = Mathf.Lerp(_s1, _s2, percentage);
            var v3 = Mathf.Lerp(_v1, _v2, percentage);
            var alpha = Mathf.Lerp(_startColor.a, _endColor.a, percentage);
            var newColor = Color.HSVToRGB(h3, s3, v3);
            newColor.a = alpha;
            
            _blurTween.Kill();
            _blurTween = _renderer.material.DOColor(newColor, _duration);
            //_renderer.material.SetColor("_Color", newColor);
        }
        
    }
}

