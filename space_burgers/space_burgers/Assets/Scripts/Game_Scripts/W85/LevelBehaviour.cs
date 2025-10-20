using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Witmina_InputController;
using Random = UnityEngine.Random;

namespace Witmina_SpaceBurgers
{
    public class LevelBehaviour : MonoBehaviour
    {
        public int levelId;
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
        public static LevelSO LevelSO;
        private List<int> scores = new List<int>();

        [SerializeField] private UIController _uiController;
        [SerializeField] private FoodArea _foodArea;
        [SerializeField] private Customer _customer;
        [SerializeField] private ServeButton _serveButton;
        private float _levelTimer;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private float _customerEnterDuration = 1f;
        [SerializeField] private float _customerExitDuration = 0.8f;

        [Header("Trays")]
        [SerializeField] private GameObject cheeseTray;
        [SerializeField] private GameObject lettuceTray;
        [SerializeField] private GameObject tomatoTray;
        [SerializeField] private GameObject onionTray;
        [SerializeField] private GameObject beefTray;
        [SerializeField] private GameObject friesTray;
        [SerializeField] private GameObject bottlesTray;
        [SerializeField] private GameObject drinkTray;
        [SerializeField] private GameObject cupTray;

        private float _timer;

        [HideInInspector] public int totalOrderCount = 0;

        private LayerMask _containerMask;

        public float Timer
        {
            get => _timer;
            private set
            {
                _timer = value;
                _timerText.text = "Time: " + Mathf.CeilToInt(_timer);
            }
        }

        private float flashInterval = 0.5f;
        private bool isFlashable = true;

        private bool _finished;
        private bool _timerRunning;

        private int _customerSatisfied;
        private int _customerUpset;

        private Vector3 _customerInitPos;
        private Container[] _containers;
        private int maxLevelWKeys;

        private void Awake()
        {
            _levelTimer = 60f;

            var aspect = 2f * Screen.width / Screen.height;
            _customerInitPos = _customer.transform.localPosition;
            _containers = GetComponentsInChildren<Container>();

            _containerMask = LayerMask.GetMask("UI");
            transform.localScale = new Vector3(aspect, 1f, 1f);
            _foodArea.Active = false;
        }

        private void OnEnable()
        {
            InputController.Instance.Pressed += OnPressed;
            //InputController.Instance.Moved += OnMoved;
            InputController.Instance.Released += OnReleased;
        }

        private void OnDisable()
        {
            InputController.Instance.Pressed -= OnPressed;
            //InputController.Instance.Moved -= OnMoved;
            InputController.Instance.Released -= OnReleased;
        }

        private void OnDestroy()
        {
            _serveButton.Pressed -= Serve;

            InputController.Instance.Pressed -= OnPressed;
            //InputController.Instance.Moved -= OnMoved;
            InputController.Instance.Released -= OnReleased;
        }

        public void AssignLevel()
        {
            maxLevelWKeys = levels.Count;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            LevelSO = levels[levelId - 1];

            _uiController.UpdateLevel(levelId);

            DisableTrays();
        }

        private void Update()
        {
            if (_finished || !_timerRunning)
                return;


            if (Timer > 0)
                Timer -= Time.deltaTime;
            else
            {
                _timerRunning = false;
                Timer = 0;
                _uiController.UpdateTime(0);
                End();
            }

            _uiController.UpdateTime(Timer);

            if (Timer <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.Instance.PlayFx("Countdown", 0.7f, 1f);
                // FlashRed();
            }
        }

        public void Load()
        {
            _timer = _levelTimer;
            foreach (var container in _containers)
            {
                container.Initialize();
            }

            _customerSatisfied = 0;
            _customerUpset = 0;
            _foodArea.Initialize();
            _customer.Initialize();
            _customer.ResetSprite();
            _customer.transform.localPosition = 9f * Vector3.right;

            SetCustomerOrderCount();
        }

        private void CalculateLevelScore(bool isSuccess)
        {
            if (isSuccess)
            {
                scores.Add(LevelSO.maxInLevel);
            }
            else
            {
                scores.Add(-LevelSO.penaltyPoints);
            }
        }

        public int CalculateTotalScore()
        {
            float total = 0f;

            for (int i = 0; i < scores.Count; i++)
            {
                total += (float)scores[i];
            }

            total /= scores.Count;
            return Mathf.Clamp(Mathf.CeilToInt(total), 0, 1000);
        }

        public void BeginLevel()
        {
            _customer.transform.DOKill();
            _customer.transform.DOLocalJump(_customerInitPos, 0.2f, 4, _customerEnterDuration)
                .SetEase(Ease.Linear)
                .OnComplete(ActivateOrdering);
        }

        public void Serve()
        {
            var result = EvaluateOrders();
            GameManager.PlayAudioFx(AudioFxType.OrderServe);

            int upCounter = PlayerPrefs.GetInt("SpaceBurgers_UpCounter", 0);
            int downCounter = PlayerPrefs.GetInt("SpaceBurgers_DownCounter", 0);

            if (result)
            {
                GameManager.PlayAudioFx(AudioFxType.OrderWin);
                GameManager.PlayAudioFx(AudioFxType.OrderWin2, 0.5f);

                _customerSatisfied++;

                if (++upCounter >= LevelSO.numOfCorrectsForLevelUp * 2)
                {
                    Debug.Log("Up Counter: " + upCounter + " >= " + LevelSO.numOfCorrectsForLevelUp);

                    upCounter = 0;
                    downCounter = 0;
                    ++levelId;
                }
            }
            else
            {
                GameManager.PlayAudioFx(AudioFxType.OrderFail);
                _customerUpset++;

                if (++downCounter >= LevelSO.levelDownCriteria)
                {
                    Debug.Log("Down Counter: " + downCounter + " >= " + LevelSO.levelDownCriteria);

                    upCounter = 0;
                    downCounter = 0;
                    --levelId;
                }
            }

            PlayerPrefs.SetInt("SpaceBurgers_UpCounter", upCounter);
            PlayerPrefs.SetInt("SpaceBurgers_DownCounter", downCounter);

            _uiController.UpdateCorrectCount(_customerSatisfied);
            _uiController.UpdateWrongCount(_customerUpset);

            Finish(result);
        }

        private void Finish(bool success)
        {
            _customer.TimeUp -= OnCustomerTimeUp;
            _serveButton.Pressed -= Serve;

            CalculateLevelScore(success);

            StartCoroutine(FinishRoutine(success));
        }

        private void End()
        {
            _foodArea.OnFinish();
            _finished = true;
            _timerRunning = false;
            _customer.TimerRunning = false;
            _customer.TimeUp -= OnCustomerTimeUp;
            _serveButton.Pressed -= Serve;
            GameManager.Instance.End(_customerSatisfied, _customerUpset);
        }

        public bool EvaluateOrders()
        {
            if (_customer.Orders.Count != _foodArea.Foods.Count(f => f.gameObject.activeSelf))
            {
                return false;
            }

            foreach (var order in _customer.Orders)
            {
                var food = _foodArea.Foods.FirstOrDefault(f => f.Type == order.FoodType);
                if (!food || order.Ingredients.Count != food.Ingredients.Count)
                    return false;

                for (int i = 0; i < order.Ingredients.Count && i < food.Ingredients.Count; i++)
                {
                    if (order.Ingredients[i] != food.Ingredients[i].Type)
                        return false;
                }
            }

            return true;
        }

        private void ActivateOrdering()
        {
            _customer.GenerateOrders();

            totalOrderCount++;

            _foodArea.Active = true;
            _serveButton.Pressed += Serve;
            _finished = false;

            _customer.TimeUp += OnCustomerTimeUp;
            _timerRunning = true;
            _customer.TimerRunning = true;
            _customer.StartMaskMovement();
        }

        public int GetUpsetCustomerCount()
        {
            return _customerUpset;
        }

        public float GetCustomerRemainingTime()
        {
            return _customer.Timer;
        }

        private void OnCustomerTimeUp()
        {
            GameManager.PlayAudioFx(AudioFxType.OrderFail);
            Finish(false);
        }

        private void SetCustomerOrderCount()
        {
            var maxOrders = LevelSO.maxOrderCount;
            _customer.OrderCount = Random.Range(1, maxOrders + 1);
        }

        private void OnPressed(PointerEventData pData)
        {
            var origin = GameManager.Instance.MainCamera.ScreenToWorldPoint(pData.position);
            var direction = GameManager.Instance.MainCamera.transform.forward;
            var hit = Physics2D.Raycast(origin, direction,
                Single.MaxValue, _containerMask);

            if (!hit)
                return;

            if (hit.collider.TryGetComponent<Container>(out var container))
                container.OnSelect(pData);
            if (hit.collider.TryGetComponent<ServeButton>(out var serve))
                serve.OnTouchDown(pData);
        }

        private void OnReleased(PointerEventData pData)
        {
            var origin = GameManager.Instance.MainCamera.ScreenToWorldPoint(pData.position);
            var direction = GameManager.Instance.MainCamera.transform.forward;
            var hit = Physics2D.Raycast(origin, direction,
                Single.MaxValue, _containerMask);

            if (!hit)
                return;

            if (hit.collider.TryGetComponent<ServeButton>(out var serve))
                serve.OnTouchUp(pData);
        }

        private IEnumerator FinishRoutine(bool success)
        {
            _timerRunning = false;
            _customer.TimerRunning = false;
            _foodArea.OnFinish();

            if (success)
                _customer.OrderCount--;
            else
                _customer.OrderCount = 0;

            _customer.GiveFeedback(success);

            yield return new WaitForSeconds(1f);

            AssignLevel();

            _customer.Initialize();
            _foodArea.Initialize();

            yield return new WaitForSeconds(0.5f);

            if (_customer.OrderCount <= 0)
            {
                SetCustomerOrderCount();

                _customer.transform.DOKill();
                _customer.RotateSprite(0);
                _customer.transform.DOLocalJump(9f * Vector3.right, 0.2f, 4, _customerExitDuration)
                    .SetEase(Ease.Linear);

                yield return new WaitForSeconds(_customerExitDuration);

                _customer.ResetSprite();
                _customer.transform.DOKill();
                _customer.RotateSprite(180);
                _customer.transform.localPosition = 9f * Vector3.right;
                _customer.transform.DOLocalJump(_customerInitPos, 0.2f, 4, _customerEnterDuration)
                    .SetEase(Ease.Linear);

                yield return new WaitForSeconds(_customerEnterDuration);
            }

            ActivateOrdering();
            yield return null;
        }

        public void DisableTrays()
        {
            onionTray.SetActive(false);
            beefTray.SetActive(false);
            tomatoTray.SetActive(false);
            lettuceTray.SetActive(false);
            cheeseTray.SetActive(false);
            friesTray.SetActive(false);
            bottlesTray.SetActive(false);
            drinkTray.SetActive(false);
            cupTray.SetActive(false);

            if (LevelSO.ingredientsCount - 3 >= 1)
            {
                beefTray.SetActive(true);
                onionTray.SetActive(true);
            }
            if (LevelSO.ingredientsCount - 3 >= 2)
                tomatoTray.SetActive(true);
            if (LevelSO.ingredientsCount - 3 >= 3)
                lettuceTray.SetActive(true);
            if (LevelSO.ingredientsCount - 3 >= 4)
                cheeseTray.SetActive(true);

            if (LevelSO.byProducts >= 2)
            {
                drinkTray.SetActive(true);
                cupTray.SetActive(true);
            }
            if (LevelSO.byProducts >= 1)
            {
                friesTray.SetActive(true);
                bottlesTray.SetActive(true);
            }
        }

        // private void FlashRed()
        // {
        //     TMP_Text timerText = GameManager.Instance.GetInGameTopbarTimer();

        //     Sequence redFlash = DOTween.Sequence();

        //     redFlash.Append(timerText.DOColor(Color.red, flashInterval))
        //             .SetEase(Ease.Linear)
        //             .Append(timerText.DOColor(Color.white, flashInterval))
        //             .SetEase(Ease.Linear)
        //             .SetLoops(6);

        //     redFlash.Play();
        // }
    }
}