using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Witmina_SweetMemory
{
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField] private List<Sprite> _carSprites;
        [SerializeField] private float _characterOffset = 680f;
        [SerializeField] private float _carOffset = 750f;
        [SerializeField] private float _delayMin = 1f;
        [SerializeField] private float _delayMax = 4f;

        [SerializeField] private Transform _car;
        [SerializeField] private Transform _car2;
        [SerializeField] private Image _neon;
        [SerializeField] private Image _rain;
        [SerializeField] private Transform _woman1;
        [SerializeField] private Transform _woman2;
        [SerializeField] private Transform _woman3;
        [SerializeField] private Transform _woman4;
        [SerializeField] private float _heelsVolume = 0.2f;
        [SerializeField] private float _carVolume = 0.2f;

        [HideInInspector] public bool Active;

        private float _w1Speed;
        private float _w2Speed;
        private float _w3Speed;
        private float _w4Speed;
        private float _w1Delay;
        private float _w2Delay;
        private float _w3Delay;
        private float _w4Delay;
        private float _car1Speed;
        private float _car2Speed;
        private float _car1Delay;
        private float _car2Delay;
        private float _rainDelay;
        private float _rainDuration;
        private Image _car1Image;
        private Image _car2Image;

        private bool _w1Sound;
        private bool _w2Sound;
        private bool _w3Sound;
        private bool _w4Sound;
        private bool _car1Sound;
        private bool _car2Sound;

        public void Initialize()
        {
            _woman1.gameObject.SetActive(true);
            _woman2.gameObject.SetActive(true);
            _woman3.gameObject.SetActive(true);
            _woman4.gameObject.SetActive(true);
            _car.gameObject.SetActive(true);
            _car2.gameObject.SetActive(true);

            _woman1.localPosition = _characterOffset * Vector3.left;
            _woman2.localPosition = _characterOffset * Vector3.right;
            _woman3.localPosition = _characterOffset * Vector3.left;
            _woman4.localPosition = _characterOffset * Vector3.right;
            _car.transform.localPosition = _carOffset * Vector3.left;
            _car.transform.localScale = new Vector3(-_car.transform.localScale.x, _car.transform.localScale.y, _car.transform.localScale.z);
            _car2.transform.localScale = new Vector3(-_car2.transform.localScale.x, _car2.transform.localScale.y, _car2.transform.localScale.z);

            _neon.GetComponent<Animator>().enabled = true;

            _w1Speed = Random.Range(450f, 500f);
            _w2Speed = Random.Range(450f, 500f);
            _w3Speed = Random.Range(450f, 500f);
            _w4Speed = Random.Range(450f, 500f);
            _w1Delay = 0.33f;
            _w2Delay = 0.66f;
            _w3Delay = 1.33f;
            _w4Delay = 1.99f;
            _car1Speed = Random.Range(450f, 500f);
            _car2Speed = Random.Range(450f, 500f);
            _car1Delay = Random.Range(_delayMin, _delayMax);
            _car2Delay = Random.Range(_delayMin, _delayMax);
            _car1Image = _car.GetComponentInChildren<Image>();
            _car2Image = _car2.GetComponentInChildren<Image>();
            _car1Image.sprite = _carSprites[Random.Range(0, _carSprites.Count)];
            _car2Image.sprite = _carSprites[Random.Range(0, _carSprites.Count)];

            _rainDuration = Random.Range(10f, 15f);
        }

        // Update is called once per frame
        void Update()
        {
            if (!Active)
                return;

            //Woman1
            if (_w1Delay > 0)
            {
                _w1Delay -= Time.deltaTime;
            }
            else
            {
                _woman1.localPosition += _w1Speed * Time.deltaTime * Vector3.right;
                if (!_w1Sound && _woman1.localPosition.x > -_characterOffset / 4f)
                {
                    _w1Sound = true;
                    AudioController.Play(AudioType.Heels, _heelsVolume);
                }
                if (_woman1.localPosition.x > _characterOffset)
                {
                    _w1Delay = 0.33f;
                    _w1Speed = Random.Range(450f, 500f);
                    _woman1.localPosition = _characterOffset * Vector3.left;
                    _w1Sound = false;
                }
            }

            //Woman2
            if (_w2Delay > 0)
            {
                _w2Delay -= Time.deltaTime;
            }
            else
            {
                _woman2.localPosition += _w2Speed * Time.deltaTime * Vector3.left;
                if (!_w2Sound && _woman2.localPosition.x < _characterOffset / 4f)
                {
                    _w2Sound = true;
                    AudioController.Play(AudioType.Heels, _heelsVolume);
                }
                if (_woman2.localPosition.x < -_characterOffset)
                {
                    _w2Speed = Random.Range(450f, 500f);
                    _w2Delay = 0.66f;
                    _woman2.localPosition = _characterOffset * Vector3.right;
                    _w2Sound = false;
                }
            }

            //Woman3
            if (_w3Delay > 0)
            {
                _w3Delay -= Time.deltaTime;
            }
            else
            {
                _woman3.localPosition += _w3Speed * Time.deltaTime * Vector3.right;
                if (!_w3Sound && _woman3.localPosition.x > -_characterOffset / 4f)
                {
                    _w3Sound = true;
                    AudioController.Play(AudioType.Heels, _heelsVolume);
                }
                if (_woman3.localPosition.x > _characterOffset)
                {
                    _w3Delay = 0.33f;
                    _w3Speed = Random.Range(450f, 500f);
                    _woman3.localPosition = _characterOffset * Vector3.left;
                    _w3Sound = false;
                }
            }

            //Woman4
            if (_w4Delay > 0)
            {
                _w4Delay -= Time.deltaTime;
            }
            else
            {
                _woman4.localPosition += _w4Speed * Time.deltaTime * Vector3.left;
                if (!_w4Sound && _woman4.localPosition.x < _characterOffset / 4f)
                {
                    _w4Sound = true;
                    AudioController.Play(AudioType.Heels, _heelsVolume);
                }
                if (_woman4.localPosition.x < -_characterOffset)
                {
                    _w4Speed = Random.Range(450f, 500f);
                    _w4Delay = 1.99f;
                    _woman4.localPosition = _characterOffset * Vector3.right;
                    _w4Sound = false;
                }
            }

            //CAR1
            if (_car1Delay > 0)
            {
                _car1Delay -= Time.deltaTime;
            }
            else
            {
                _car.localPosition += _car1Speed * Time.deltaTime * Vector3.right;
                if (!_car1Sound && _car.transform.localPosition.x > -_carOffset / 2f)
                {
                    _car1Sound = true;
                    AudioController.Play(AudioType.Car, _carVolume);
                }
                if (_car.transform.localPosition.x > _carOffset)
                {
                    _car1Delay = Random.Range(_delayMin, _delayMax);
                    _car1Speed = Random.Range(450f, 500f);
                    _car.transform.localPosition = _carOffset * Vector3.left;
                    _car1Image.sprite = _carSprites[Random.Range(0, _carSprites.Count)];
                    _car1Sound = false;
                }
            }

            //CAR2
            if (_car2Delay > 0)
            {
                _car2Delay -= Time.deltaTime;
            }
            else
            {
                _car2.localPosition += _car2Speed * Time.deltaTime * Vector3.left;
                if (!_car2Sound && _car.transform.localPosition.x < _carOffset / 2f)
                {
                    _car2Sound = true;
                    AudioController.Play(AudioType.Car, _carVolume);
                }
                if (_car2.transform.localPosition.x < -_carOffset)
                {
                    _car2Delay = Random.Range(_delayMin, _delayMax);
                    _car2Speed = Random.Range(450f, 500f);
                    _car2.transform.localPosition = _carOffset * Vector3.right;
                    _car2Image.sprite = _carSprites[Random.Range(0, _carSprites.Count)];
                    _car2Sound = false;
                }
            }

            //Rain
            if (_rainDuration > 0)
            {
                _rainDuration -= Time.deltaTime;
            }
            else
            {
                if (_rainDelay > 0)
                {
                    _rain.gameObject.SetActive(false);
                    AudioController.ToggleRain(false);

                    _rainDelay -= Time.deltaTime;
                }
                else
                {
                    _rainDelay = Random.Range(10f, 15f);
                    _rainDuration = Random.Range(10f, 15f);

                    _rain.gameObject.SetActive(true);
                    AudioController.ToggleRain(true);
                }
            }
        }

        public void OnEnd()
        {
            _woman1.gameObject.SetActive(false);
            _woman2.gameObject.SetActive(false);
            _car.gameObject.SetActive(false);
            _car2.gameObject.SetActive(false);
        }
    }

}
