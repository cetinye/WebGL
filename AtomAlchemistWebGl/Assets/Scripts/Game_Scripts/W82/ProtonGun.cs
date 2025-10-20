using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Witmina_AtomAlchemist
{
    public class ProtonGun : MonoBehaviour
    {
        public event Action Overheated;

        [SerializeField] private List<SpriteRenderer> _sprite = new List<SpriteRenderer>();
        [SerializeField] private Transform _nuzzle;
        [SerializeField] private Transform spriteParent;
        [SerializeField] private Proton _protonPrefab;
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private ParticleSystem _smoke;
        [SerializeField] private float _shootSpeed = 3f;
        [SerializeField] private float barIncreaseTime;
        [SerializeField] private float decreaseInterval;
        [SerializeField] private float _shootCooldown = 0.16f;
        [SerializeField] private int _heatRate = 1;
        [SerializeField] private float _overheatTimer = 2f;
        [SerializeField] private Color _overheatTint;
        [SerializeField] private Transform gunNuzzle;
        [SerializeField] private float knockbackAmount;
        [SerializeField] private float knockbackDuration;

        private float _cooldown;
        [SerializeField] private int _heat;
        private bool _overHeat;
        private Sequence _colorTween;
        private Tween _shakeTween;
        private Tween _barTween;
        private float rate = 0.023f;

        private bool OverHeat
        {
            get => _overHeat;
            set
            {
                _overHeat = value;
                _colorTween.Kill();

                foreach (SpriteRenderer barRenderer in _sprite)
                {
                    _colorTween?.Join(barRenderer.DOColor(_overHeat ? _overheatTint : Color.white, 0.15f));
                }

                if (!_overHeat)
                    return;

                Overheated?.Invoke();
                _smoke.Play();
                _shakeTween.Kill();
                _shakeTween = transform.DOShakePosition(0.15f, 0.25f * Vector3.right,
                    25, 5).SetEase(Ease.InOutCirc);
            }
        }

        void Start()
        {
            InvokeRepeating(nameof(DecreaseHeat), decreaseInterval, decreaseInterval);
        }

        private void OnDestroy()
        {
            _colorTween.Kill();
            _shakeTween.Kill();
            _barTween.Kill();
        }

        private void Update()
        {
            if (_cooldown > 0f)
            {
                _cooldown -= Time.deltaTime;
            }
            else
            {
                _cooldown = 0;
            }
        }

        public void Initialize()
        {
            _colorTween.Kill();
            _shakeTween.Kill();
            _barTween.Kill();

            _cooldown = 0f;
            _heat = 0;
            _overHeat = false;

            foreach (SpriteRenderer barRenderer in _sprite)
            {
                barRenderer.color = Color.white;
            }
        }

        public bool Shoot()
        {
            if (_cooldown > 0 || _overHeat)
                return false;

            Knockback();
            _cooldown = _shootCooldown;
            IncreaseHeat();
            var proton = Instantiate(_protonPrefab,
                _nuzzle.transform.position, Quaternion.identity);

            _particles.Play();
            proton.Shoot(_shootSpeed * GameManager.Instance.SpeedMultiplier * Vector3.up);
            return true;
        }

        private void DecreaseHeat()
        {
            if (_heat > 0f)
            {
                _heat -= 1;
            }

            if (_heat == 8)
                OverHeat = false;

            spriteParent.DOScaleY(0.61f + (_heat * rate), barIncreaseTime);
        }

        private void IncreaseHeat()
        {
            _heat += _heatRate;

            if (_heat >= 17)
            {
                AudioController.instance.PlayOneShot("LaserCooldown");
                _heat = 17;
                OverHeat = true;
            }

            spriteParent.DOScaleY(0.61f + (_heat * rate), barIncreaseTime);
        }

        private void Knockback()
        {
            gunNuzzle.DOMoveY(gunNuzzle.position.y - knockbackAmount, knockbackDuration).SetLoops(2, LoopType.Yoyo);
        }
    }
}
