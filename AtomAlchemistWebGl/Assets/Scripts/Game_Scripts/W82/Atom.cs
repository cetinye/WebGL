using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Witmina_AtomAlchemist
{
    public class Atom : MonoBehaviour
    {
        /*public static readonly string[] NAMES = new[]
        {
            "",
            "Hydrogen",
            "Helium",
            "Lithium",
            "Beryllium",
            "Boron",
            "Carbon",
            "Nitrogen",
            "Oxygen",
            "Fluorine",
            "Neon",
            "Sodium",
            "Magnesium",
            "Aluminium",
            "Silicon",
            "Phosphorus",
            "Sulfur",
            "Chlorine",
            "Argon",
            "Potassium",
            "Calcium",
            "Scandium"
        };*/


        public event Action NucleusHit;
        public event Action Transmuted;

        [SerializeField] private LevelBehaviour levelManager;
        [SerializeField] private List<Orbital> _orbitals;
        [SerializeField] private Transform _nucleusTransform;
        [SerializeField] private ParticleSystem _hitParticles;
        [SerializeField] private ParticleSystem _transmuteParticles;

        private Tween _nucleusTween;
        private Vector3 _scale;
        private float rotateSpeed;

        private Element _element;
        public Element Element
        {
            get => _element;
            private set
            {
                if (value is > Element.Sc or < Element.H)
                    return;

                _element = value;
                SetOrbitalColors();
                Transmuted?.Invoke();
            }
        }

        private void Awake()
        {
            _scale = _nucleusTransform.localScale;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.TryGetComponent<Proton>(out var proton)
                && !proton.Popped)
            {
                proton.Pop();
                _nucleusTween.Kill();
                _nucleusTransform.localScale = _scale;
                _nucleusTween = _nucleusTransform.DOPunchScale(.2f * Vector3.one, 0.25f);
                _hitParticles.Play();
                NucleusHit?.Invoke();
            }
        }

        public void StartAnimation()
        {
            StartCoroutine(StartRoutine());
        }

        public void AddElectron()
        {
            if (_element is Element.Sc)
                return;

            var firstVacant = _orbitals.FirstOrDefault(o => !o.Full);
            if (!firstVacant)
                return;

            var electron = Instantiate(GameManager.Instance.ElectronPrefab);
            if (firstVacant.AddElectron(electron))
            {
                Element++;
                _transmuteParticles.Play();
            }
        }

        public bool RemoveElectron(Electron electron)
        {
            var success = Element is not Element.H
                && _orbitals.Any(o => o.RemoveElectron(electron));
            if (success)
            {
                Element--;
            }

            return success;
        }

        public void SetAtom(Element element)
        {
            foreach (var o in _orbitals)
            {
                o.Clear();
            }

            for (int i = 0; i < (int)element; i++)
            {
                var firstVacant = _orbitals.FirstOrDefault(o => !o.Full);
                if (!firstVacant)
                    return;

                var electron = Instantiate(GameManager.Instance.ElectronPrefab);
                firstVacant.AddElectron(electron, true);
            }

            Element = element;
        }

        private void SetOrbitalColors()
        {
            _orbitals.ForEach(o =>
                o.SetColor(GameManager.Instance.SpriteData.ElementColors[Mathf.Max((int)Element - 1, 0)]));
        }

        public void SetOrbitalSpeeds()
        {
            rotateSpeed = LevelBehaviour.LevelSO.innerRingSpeed;

            for (int i = 0; i < _orbitals.Count; i++)
            {
                _orbitals[i].rotateSpeed = rotateSpeed;
                rotateSpeed += LevelBehaviour.LevelSO.ringSpeedMultiplier;
            }
        }

        private IEnumerator StartRoutine()
        {
            for (int i = 0; i < _orbitals.Count; i++)
            {
                _orbitals[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < _orbitals.Count; i++)
            {
                yield return new WaitForSeconds(0.1f);
                _orbitals[i].gameObject.SetActive(true);
            }
            yield return null;
        }
    }
}
