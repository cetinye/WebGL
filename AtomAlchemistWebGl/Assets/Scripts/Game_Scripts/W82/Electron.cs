using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Witmina_AtomAlchemist
{
    public class Electron : MonoBehaviour
    {
        public static event Action<Electron> ElectronHit;
        public bool Flying { get; set; } = true;

        [SerializeField] private GameObject _sprite;
        [SerializeField] private ParticleSystem _hitParticle;
        [SerializeField] private ParticleSystem _particle;

        private Tween _punchTween;
        private Vector3 _scale;

        private void Awake()
        {
            _scale = _sprite.transform.localScale;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.TryGetComponent<Proton>(out var proton)
                && !proton.Popped)
            {
                proton.Pop();
                _punchTween.Kill();
                _sprite.transform.localScale = _scale;
                _punchTween = _sprite.transform.DOPunchScale(.2f * Vector3.one, 0.25f);
                _hitParticle.Play();
                ElectronHit?.Invoke(this);
            }
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public void Pop()
        {
            GetComponent<Collider2D>().enabled = false;
            _sprite.SetActive(false);
            _particle.Play();
            Destroy(gameObject, _particle.main.duration);
        }

        public void Stop()
        {
            Flying = false;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
    }

}
