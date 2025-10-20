using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class W94_Light : MonoBehaviour
{
    [SerializeField] private float timeInterval;
    [SerializeField] private Image lightOffImage;
    [SerializeField] private Image lightOnImage;

    private void Start()
    {
        LightOn();
    }

    private void LightOn()
    {
        Sequence lightOnSeq = DOTween.Sequence();
        lightOnSeq.Append(lightOnImage.DOFade(1f, timeInterval));
        lightOnSeq.Join(lightOffImage.DOFade(0f, timeInterval));

        lightOnSeq.PlayForward();
        lightOnSeq.OnComplete(() => LightOff());
    }

    private void LightOff()
    {
        Sequence lightOffSeq = DOTween.Sequence();
        lightOffSeq.Append(lightOnImage.DOFade(0f, timeInterval));
        lightOffSeq.Join(lightOffImage.DOFade(1f, timeInterval));

        lightOffSeq.PlayForward();
        lightOffSeq.OnComplete(() => LightOn());
    }
}
