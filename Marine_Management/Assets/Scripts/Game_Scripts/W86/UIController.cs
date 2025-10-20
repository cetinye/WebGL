using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Witmina_MarineManagement
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject _introPanel;
        [SerializeField] private GameObject _servicePanel;
        [SerializeField] private GameObject _endGamePanel;
        [SerializeField] private GameObject _skipButton;
        [SerializeField] private GameObject _hudPanel;
        /*[SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private ContentSizeFitter _scrollContent;
        [SerializeField] private Button _scrollLeft;
        [SerializeField] private Button _scrollRight;
        [SerializeField] private float _scrollAmount = 1.7f;*/

        public int MaxTier;

        private List<UIServiceElement> _elements;
        private List<RequestData> _reqList;
        private float aspectRatio;

        private void Awake()
        {
            _endGamePanel.gameObject.SetActive(false);
            _introPanel.SetActive(true);
            _elements = GetComponentsInChildren<UIServiceElement>().ToList();
        }

        private void Update()
        {
            /*_scrollRight.gameObject.SetActive(_scrollRect.horizontalScrollbar.value < 0.99f);
            _scrollLeft.gameObject.SetActive(_scrollRect.horizontalScrollbar.value > 0.01f);*/

            if (_reqList == null)
                return;

            for (int i = 0; i < _reqList.Count; i++)
            {
                var active = i < MaxTier;
                if (active && !_elements[i].gameObject.activeSelf)
                {
                    _elements[i].transform.localScale = 0.2f * Vector3.one;
                    _elements[i].transform.DOScale(Vector3.one, 0.2f);
                }
                _elements[i].gameObject.SetActive(active);
            }
        }

        public void SetRequestList(List<RequestData> reqList)
        {
            _reqList = reqList;
        }

        public void Initialize()
        {
            /*_scrollRect.gameObject.SetActive(true);
            _scrollRect.horizontalScrollbar.value = 0f;
            _scrollRect.verticalScrollbar.value = 0f;*/
            _endGamePanel.gameObject.SetActive(false);
            _servicePanel.gameObject.SetActive(true);
        }

        /*public void ScrollMoveLeft()
        {
            var count = _scrollContent.transform.childCount;
            //var value = Mathf.Clamp(_scrollRect.horizontalScrollbar.value - (_scrollAmount / count), 0f, 1f);
            //_scrollRect.horizontalScrollbar.value = value;
        }*/

        /*public void ScrollMoveRight()
        {
            var count = _scrollContent.transform.childCount;
            var value = Mathf.Clamp(_scrollRect.horizontalScrollbar.value + (_scrollAmount / count), 0f, 1f);
            _scrollRect.horizontalScrollbar.value = value;
        }*/

        public void ActivateEndGamePanel()
        {
            //_scrollRect.gameObject.SetActive(false);
            _endGamePanel.SetActive(true);
            _servicePanel.SetActive(false);
        }

        public void ToggleIntroPanel(bool active)
        {
            _introPanel.SetActive(active);
        }

        public void ToggleSkipButton(bool active)
        {
            _skipButton.SetActive(active);
        }

        public void ScaleHUDPanel()
        {
            aspectRatio = (float)Screen.width / (float)Screen.height;

            if (aspectRatio >= 0.7f)
            {
                _hudPanel.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                _servicePanel.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            }

            else
            {
                _hudPanel.transform.localScale = Vector3.one;
                _servicePanel.transform.localScale = Vector3.one;
            }
        }
    }
}