using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_rotf
{
    public class Mushroom2D : MonoBehaviour
    {
        public event Action<Mushroom2D> Pressed;

        public KeyCode keyCode;

        private Image _mushroomImage;
        private Color _litEmissionColor;

        private Coroutine _lightRoutine;

        private void Awake()
        {
            _mushroomImage = GetComponentInChildren<Image>();
            if (!_mushroomImage)
            {
                Debug.Log($"No Image found in mushroom {name}");
                return;
            }

            _mushroomImage.gameObject.SetActive(false);

        }

#if UNITY_WEBGL
        private void Update()
        {
            if (Input.GetKeyDown(keyCode))
                Pressed?.Invoke(this);
        }
#endif

        public void LightUp()
        {
            if (_lightRoutine != null)
            {
                StopCoroutine(_lightRoutine);
            }


            _lightRoutine = StartCoroutine(LightRoutine());
        }

        public void OnPress()
        {
            Pressed?.Invoke(this);
        }

        private IEnumerator LightRoutine()
        {
            _mushroomImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            _mushroomImage.gameObject.SetActive(false);
        }
    }
}
