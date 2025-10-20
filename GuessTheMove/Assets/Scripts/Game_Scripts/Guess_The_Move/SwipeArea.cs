using UnityEngine;
using UnityEngine.EventSystems;

namespace Guess_The_Move
{
    public class SwipeArea : MonoBehaviour, IDragHandler, IPointerExitHandler
    {
        [SerializeField] private Vector3 dragVectorDirection;

        public void OnDrag(PointerEventData eventData)
        {
            dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //LevelManager.instance.Swiped(GetDragDirection(dragVectorDirection) == DraggedDirection.Right);
        }

        private DraggedDirection GetDragDirection(Vector3 dragVector)
        {
            var positiveX = Mathf.Abs(dragVector.x);
            var positiveY = Mathf.Abs(dragVector.y);
            var draggedDir = DraggedDirection.None;
            if (positiveX > positiveY) draggedDir = dragVector.x > 0 ? DraggedDirection.Right : DraggedDirection.Left;

            return draggedDir;
        }

        private enum DraggedDirection
        {
            None,
            Right,
            Left
        }
    }
}