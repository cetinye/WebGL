using UnityEngine;
using UnityEngine.EventSystems;

public class W94_DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler
{
    public bool droppedOnSth;

    [SerializeField] private Canvas _canvasTop;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector3 _oldPosition;
    private Transform _oldParent;
    private int _oldSlotIndex;

    private void Awake()
    {
        _canvasTop = transform.parent.parent.parent.parent.GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Taptic.Vibrate();
        _oldPosition = _rectTransform.position;

        W94_GameManager.instance.ArrangeFrames(transform.parent.parent);

        //change shelf order for book to render in front of every other book
        _oldParent = transform.parent.parent;
        _oldParent.SetAsLastSibling();

        //change slot order for book to render in front of Right slot book
        _oldSlotIndex = transform.parent.GetSiblingIndex();
        transform.parent.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        W94_AudioManager.instance.PlayOneShot("PickUp");
        W94_GameManager.instance.IncreaseTotalMoveCounter();

        droppedOnSth = false;

        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvasTop.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        W94_AudioManager.instance.PlayOneShot("Place");

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        if (!droppedOnSth)
        {
            _rectTransform.position = _oldPosition;
        }
        else
        {
            //check combination on new shelf book is placed
            transform.parent.parent.GetComponent<W94_Shelf>().CheckCombination();

            //check old shelf book was moved from if its now empty or not
            _oldParent.GetComponent<W94_Shelf>().CheckIfShelfEmptied();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //revert slot order
        transform.parent.SetSiblingIndex(_oldSlotIndex);
    }
}
