using DG.Tweening;
using ShiftAndLift;
using UnityEngine;

public class W3_Shape : MonoBehaviour
{
    // FIELD
    private Vector3 _startPos, _midPos, _endPos;

    public W3_GameManager _gameManager;
    public SpriteRenderer _spriteRenderer;
    public int _id;
    public bool _isModelExit;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        GetPositions();
        transform.position = _startPos;
        MoveTween();
    }

    // METHOD
    /// <summary>
    /// Sets sprite for instantiated shape.
    /// </summary>
    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    /// <summary>
    /// Gets positions and sets to variables from positionList.
    /// </summary>
    private void GetPositions()
    {
        _startPos = _gameManager.shapePositions[0].position;
        _midPos = _gameManager.shapePositions[1].position;
        _endPos = _gameManager.shapePositions[2].position;
    }

    /// <summary>
    /// Shows the shapes.
    /// </summary>
    private void MoveTween()
    {
        float moveSpeed = _gameManager._moveSpeed;
        float exitSpeed = moveSpeed - 0.3f;
        exitSpeed = Mathf.Clamp(exitSpeed, 0.15f, moveSpeed);

        Tween moveToMid = transform.DOMove(_midPos, moveSpeed).SetEase(Ease.Linear).OnComplete(() => _gameManager.conveyor.PauseRoll());
        Tween moveToEnd = transform.DOMove(_endPos, exitSpeed).SetEase(Ease.Linear).OnComplete(() => _gameManager.conveyor.PlayRoll());

        W3_AudioManager.instance.PlayOneShot("BoxMove");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(moveToMid).onComplete = () => _gameManager.conveyor.PauseRoll();
        sequence.AppendInterval(_gameManager._productShowTime);
        sequence.AppendCallback(() => _gameManager.conveyor.PlayRoll());
        sequence.Append(moveToEnd);
        sequence.onComplete = () => { Destroy(gameObject); _isModelExit = true; };
        sequence.SetAutoKill(true);
    }
}