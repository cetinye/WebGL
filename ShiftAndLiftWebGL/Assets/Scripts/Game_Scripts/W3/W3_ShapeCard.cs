using DG.Tweening;
using ShiftAndLift;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class W3_ShapeCard : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // FIELD
    [SerializeField] GameObject _sprite;
    [SerializeField] ParticleSystem _effect;
    [SerializeField] SpriteRenderer _cardSpRenderer;
    [SerializeField] SpriteRenderer _shapeSpRenderer;

    private Vector3 _screenPoint;
    private Vector3 _offset;
    private Vector3 _currentPosition;

    public W3_GameManager _gameManager;
    public Vector3 _basePosition;
    public int _id;

    private Vector2 _initialInputPos;
    private float _difference = 0;
    private Rigidbody2D _rb;
    private BoxCollider2D _col;

    // MAIN
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_gameManager._levelTimerStatus) return;

        if (eventData is not PointerEventData pData)
            return;

        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _rb.bodyType = RigidbodyType2D.Static;

        _initialInputPos = _initialInputPos = pData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_gameManager._levelTimerStatus) return;

        if (eventData is not PointerEventData pData)
            return;

        _effect.gameObject.SetActive(true);

        _difference = pData.position.x - _initialInputPos.x;

        var pos = eventData.position;
        Vector3 curScreenPoint = new Vector3(pos.x, pos.y, _screenPoint.z);
        Vector3 curPosition = _gameManager.cameraReference.ScreenToWorldPoint(curScreenPoint) + _offset;
        _currentPosition = new Vector3(curPosition.x, transform.position.y, transform.position.z);
        transform.position = _currentPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_gameManager._levelTimerStatus) return;
        _effect.gameObject.SetActive(false);

        if (_difference < 0)
        {
            W3_AudioManager.instance.PlayOneShot("Swipe");
            if (_gameManager.nonAppearedIds.Contains(_id))
            {
                _gameManager.IncDecScore(true);
                InstantiateResult(true);
                _gameManager.nonAppearedIds.Remove(_id);
            }
            else
            {
                _gameManager.IncDecScore(false);
                InstantiateResult(false);
                _gameManager.appearedIds.Remove(_id);
            }

            _gameManager.idsOnScreen.Remove(_id);
            BeDestroyed();
        }
        else if (_difference > 0)
        {
            W3_AudioManager.instance.PlayOneShot("Swipe");
            if (_gameManager.appearedIds.Contains(_id))
            {
                _gameManager.IncreaseTruckBox();
                _gameManager.IncDecScore(true);
                InstantiateResult(true);
                _gameManager.appearedIds.Remove(_id);
            }
            else
            {
                _gameManager.IncDecScore(false);
                InstantiateResult(false);
                _gameManager.nonAppearedIds.Remove(_id);
            }

            _gameManager.idsOnScreen.Remove(_id);
            BeDestroyed();
        }

        if (_gameManager.idsOnScreen.Count <= 0)
        {
            _gameManager.ChangeSceneToStorage();
        }

        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }
    // METHOD
    /// <summary>
    /// Sets some fields of shapeCard.
    /// </summary>
    /// <param name="sprite"></param>
    public void SetShapeCard(int id, Sprite sprite, Vector2 spSize)
    {
        _id = id;
        _shapeSpRenderer.sprite = sprite;
        if (_gameManager._shapeCount > 5)
        {
            _sprite.transform.localScale = new Vector3(0.4f, 0.4f);
        }

        _cardSpRenderer.size = spSize;
    }

    private void InstantiateResult(bool isCorrect)
    {
        var resultObjectIndex = isCorrect ? 0 : 1;
        var result = Instantiate(_gameManager.resultObjects[resultObjectIndex],
            _basePosition, Quaternion.identity, _gameManager._resultObjectParent);
        _gameManager.instantiatedResultObjects.Add(result);
    }

    private void BeDestroyed()
    {
        _gameManager.instantiatedShapeCards.Remove(this);
        Instantiate(_gameManager._popEffect, _basePosition, quaternion.identity, _gameManager._popsParent);
        Destroy(gameObject);
    }

    public Tween MoveTween(Vector3 targetPos, float speed)
    {
        Tweener tween = transform.DOMove(targetPos, speed).SetEase(Ease.Linear);
        return tween;
    }
}