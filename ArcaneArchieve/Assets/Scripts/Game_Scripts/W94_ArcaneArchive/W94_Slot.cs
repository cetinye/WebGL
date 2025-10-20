using UnityEngine;
using UnityEngine.EventSystems;

public class W94_Slot : MonoBehaviour, IDropHandler
{
    public bool occupied = false;

    public GameObject backSlot;

    private void Update()
    {
        if (W94_GameManager.instance.state == W94_GameManager.GameState.playing)
            UpdateOccupationStatus();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && !occupied)
        {
            occupied = true;
            eventData.pointerDrag.GetComponent<W94_DragDrop>().droppedOnSth = true;

            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
            eventData.pointerDrag.transform.SetParent(this.gameObject.transform);

            W94_GameManager.instance.ArrangeFrames(transform.parent.parent);
        }
    }

    public void UpdateOccupationStatus()
    {
        if (transform.childCount > 0)
            occupied = true;

        else
            occupied = false;
    }
}
