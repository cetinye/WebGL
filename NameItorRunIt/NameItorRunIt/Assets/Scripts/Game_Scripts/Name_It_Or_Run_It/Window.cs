using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;

namespace Name_It_Or_Run_It
{
    public class Window : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private Item item;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image windowImage;
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text textToEnable;
        [SerializeField] private RectTransform positionCenterPlaceholder;
        [SerializeField] private RectTransform positionDownPlaceholder;
        [SerializeField] private Vector3 scaleTo;
        [SerializeField] private float scaleTimeAtStart;
        [SerializeField] private float tweenTime;

        public void Select()
        {
            StartCoroutine(SelectRoutine());
        }

        public void SetItem(Item item)
        {
            this.item = item;
            itemImage.sprite = item.itemSprite;
            windowImage.enabled = true;
            itemImage.enabled = true;
            ScaleAtStart();
        }

        public void Reset()
        {
            rectTransform.localScale = Vector3.one;
            itemImage.sprite = null;
            itemImage.gameObject.SetActive(true);
            textToEnable.gameObject.SetActive(false);
            windowImage.enabled = false;
            itemImage.enabled = false;
            this.gameObject.SetActive(true);
            ScaleAtStart();
        }

        public void Close()
        {
            MoveToDown().Play().OnComplete(() =>
            {
                this.gameObject.SetActive(false);
            });
        }

        public void MoveTo(RectTransform target, float tweenTime)
        {
            rectTransform.DOAnchorPos(target.anchoredPosition, tweenTime);
        }

        IEnumerator SelectRoutine()
        {
            levelManager.SetSelectedItem(item, this);
            AudioManager.instance.PlayOneShot("WindowScaleUp");
            Sequence move = MoveToCenter().Play();
            yield return move.WaitForCompletion();
            yield return new WaitForSeconds(levelManager.timeToShowItem);
            textToEnable.gameObject.SetActive(true);
            itemImage.gameObject.SetActive(false);

            GameStateManager.SetGameState(GameStateManager.GameState.ASK_QUESTION);
        }

        private Sequence MoveToCenter()
        {
            Sequence moveToCenter = DOTween.Sequence();

            moveToCenter.Append(rectTransform.DOAnchorPos(positionCenterPlaceholder.anchoredPosition, tweenTime));
            moveToCenter.Join(rectTransform.DOScale(scaleTo, tweenTime));

            return moveToCenter;
        }

        private Sequence MoveToDown()
        {
            Sequence moveToDown = DOTween.Sequence();

            moveToDown.Append(rectTransform.DOAnchorPos(positionDownPlaceholder.anchoredPosition, tweenTime / 2));
            moveToDown.Join(rectTransform.DOScale(0f, tweenTime / 2));

            return moveToDown;
        }

        private void ScaleAtStart()
        {
            AudioManager.instance.Play("WindowMove");
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, scaleTimeAtStart);
        }
    }
}