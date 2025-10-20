using UnityEngine;
using DG.Tweening;

public class W3_Conveyor : MonoBehaviour
{
    [SerializeField] private RectTransform line;
    [SerializeField] private float timeToMove;

    [SerializeField] private Transform conveyorParent;
    private Sequence roll;

    // Start is called before the first frame update
    void Start()
    {
        Tween move = line.DOAnchorPosX(1804f, timeToMove).SetEase(Ease.Linear);
        roll = DOTween.Sequence();

        roll.Append(move);
        roll.SetLoops(-1);
        roll.SetEase(Ease.Linear);
        roll.OnStepComplete(() => ResetLine());
        roll.Play();
    }

    void ResetLine()
    {
        if (line.TryGetComponent(out RectTransform _rect))
            _rect.anchoredPosition = Vector2.zero;
    }

    public void PauseRoll()
    {
        roll.Pause();
    }

    public void PlayRoll()
    {
        roll.Play();
    }
}
