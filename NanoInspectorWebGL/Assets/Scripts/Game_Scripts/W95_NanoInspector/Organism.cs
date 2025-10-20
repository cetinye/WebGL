using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace NanoInspector
{
    public class Organism : MonoBehaviour
    {
        public Color color;
        public Image image;

        public Colors colorType;
        public MovementTypes movementType;

        [Header("Shake Variables")]
        [SerializeField] private float shakeStrength = 10;
        [SerializeField] private float shakeDuration = 1;
        [SerializeField] private int shakeVibration = 5;
        [SerializeField] private float shakeElasticity = 0.5f;

        [Header("Scale Variables")]
        [SerializeField] private float scaleStrength;
        [SerializeField] private float scaleDuration;
        [SerializeField] private int scaleVibration;
        [SerializeField] private float scaleElasticity;

        [Header("Rotation Variables")]
        [SerializeField] private float rotateDuration;

        private void Awake()
        {
            color = Color.white;
            image = GetComponent<Image>();

            float z = Random.Range(-360, 360);
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, z, transform.rotation.w);
        }

        public void SetColor(Color color, int index)
        {
            this.color = color;
            this.image.color = this.color;

            this.colorType = (Colors)index;
        }

        public void SetImage(Sprite image)
        {
            this.image.sprite = image;
        }

        public void StartMovement()
        {
            switch (movementType)
            {
                case MovementTypes.None:
                    break;

                case MovementTypes.Shaking:
                    Tween shake = transform.DOPunchRotation(Vector3.one * shakeStrength, shakeDuration, shakeVibration, shakeElasticity).SetEase(Ease.Linear).SetLoops(-1);
                    shake.Play();
                    break;
                case MovementTypes.Scaling:
                    Tween scale = transform.DOPunchScale(Vector3.one * scaleStrength, scaleDuration, scaleVibration, scaleElasticity).SetEase(Ease.Linear).SetLoops(-1);
                    scale.Play();
                    break;
                case MovementTypes.RotatingRight:
                    Tween rotateLeft = transform.DORotate(new Vector3(0, 0, transform.rotation.z + 720f), rotateDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
                    rotateLeft.Play();
                    break;
                case MovementTypes.RotatingLeft:
                    Tween rotateRight = transform.DORotate(new Vector3(0, 0, transform.rotation.z - 720f), rotateDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
                    rotateRight.Play();
                    break;
                default:
                    break;
            }
        }

        public Colors GetRandomColor()
        {
            int randIndex = Random.Range(0, (int)Enum.GetValues(typeof(Colors)).Cast<Colors>().Max());
            return (Colors)randIndex;
        }

        public MovementTypes GetRandomMovement()
        {
            int randIndex = Random.Range(0, (int)Enum.GetValues(typeof(MovementTypes)).Cast<MovementTypes>().Max());
            return (MovementTypes)randIndex;
        }

        public enum MovementTypes
        {
            None,
            Shaking,
            Scaling,
            RotatingRight,
            RotatingLeft
        }

        public enum Colors
        {
            Red,
            Pink,
            Purple,
            Blue,
            LightBlue,
            Green,
            Brown,
            Yellow,
            Orange
        }
    }
}