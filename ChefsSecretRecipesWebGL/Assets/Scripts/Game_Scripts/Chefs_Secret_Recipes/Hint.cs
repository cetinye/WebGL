using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chefs_Secret_Recipes
{
    public class Hint : MonoBehaviour
    {
        [SerializeField] private Image hintImage;
        [SerializeField] private TMP_Text hintText;

        [Header("DOShake Variables")]
        [SerializeField] private float timeToShake;
        [SerializeField] private Vector3 shakeVector;
        [SerializeField] private int shakeVibrato;
        [SerializeField] private float shakeRandomness;

        [Header("DOScale Variables")]
        [SerializeField] private float timeToScale;
        [SerializeField] private Vector3 scaleTarget;

        [Header("DOFade Variables")]
        [SerializeField] private float timeToFade;
        [SerializeField] private float fadeTarget;
        private Sequence sequence;

        void Start()
        {
            sequence = DOTween.Sequence();

            sequence.Append(hintImage.transform.DOShakeRotation(timeToShake, shakeVector, shakeVibrato, shakeRandomness));
            sequence.Join(hintImage.transform.DOScale(scaleTarget, timeToScale).SetLoops(2, LoopType.Yoyo));
            sequence.Join(hintImage.DOFade(fadeTarget, timeToFade));
        }

        public void SetHint(Sprite image, int value)
        {
            hintImage.sprite = image;
            hintText.text = value.ToString("F0");
        }

        public Sprite GetSprite()
        {
            return hintImage.sprite;
        }

        public int GetValue()
        {
            return int.Parse(hintText.text);
        }
    }
}