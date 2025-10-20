using System;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    public class BoatBehaviour : MonoBehaviour
    {
        public event Action FishHookLaunched;
        public event Action<List<CatchableItem>> ItemCaught;
        public event Action<List<CatchableItem>> ItemCollected;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Transform _boatModel;
        [SerializeField] private FishHook _fishHook;
        [SerializeField] private LineRenderer _ropeRenderer;
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _maxDisplacement = 8.5f;
        [SerializeField] private float _maxCameraDisplacement = 3.5f;

        [SerializeField] private W51_InputController w51InputController;

        [HideInInspector] public bool Active;

        private bool _leftButtonPressed;
        private bool _rightButtonPressed;

        private Vector3 _startPos;
        private Vector3 _cameraStartPos;

        private bool _comboBonusEnabled;
        private float _comboBonusMultiplier;

        private static readonly float HookLineOffset = 0.15f;

        #region Unity Methods

        private void Awake()
        {
            _startPos = transform.localPosition;
            _cameraStartPos = _mainCamera.transform.position;
        }
        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void Update()
        {
            if (!Active)
                return;

            //Movement
            var direction = _leftButtonPressed ? -1 : _rightButtonPressed ? 1 : 0;
            var localPos = transform.localPosition;

            if (direction != 0)
            {
                //calculate displacement for the boat
                var newDisplacement = Mathf.Clamp(localPos.x + direction * _moveSpeed * _comboBonusMultiplier * Time.deltaTime,
                                            -_maxDisplacement, _maxDisplacement);
                transform.localPosition = new Vector3(newDisplacement, localPos.y, localPos.z);

                //Calculate camera shift so the boat is in frame
                var cameraOffset = Mathf.Sign(newDisplacement) *
                                   Mathf.Max(Mathf.Abs(newDisplacement) - _maxCameraDisplacement, 0);
                _mainCamera.transform.position = _cameraStartPos + cameraOffset * Vector3.right;
            }

            //Hook
            _ropeRenderer.enabled = _fishHook.Moving;
            _ropeRenderer.SetPosition(1, _fishHook.transform.localPosition + HookLineOffset * Vector3.up);
        }
        #endregion

        #region Public Methods

        public void Initialize()
        {
            transform.localPosition = _startPos;
            transform.rotation = Quaternion.identity;

            _fishHook.ResetHook();
            _ropeRenderer.SetPosition(0, Vector3.zero);
            _ropeRenderer.enabled = true;
            Active = false;

            _mainCamera.transform.position = _cameraStartPos;
            _comboBonusMultiplier = 1f;
            Subscribe();
        }

        #endregion

        #region Event Functions
        private void Subscribe()
        {
            w51InputController.ShootButtonDown += OnShootButtonDown;
            w51InputController.ShootButtonUp += OnShootButtonUp;
            w51InputController.LeftButtonDown += OnLeftButtonDown;
            w51InputController.LeftButtonUp += OnLeftButtonUp;
            w51InputController.RightButtonDown += OnRightButtonDown;
            w51InputController.RightButtonUp += OnRightButtonUp;

            _fishHook.Launched += OnFishHookLaunched;
            _fishHook.ItemCaught += OnItemCaught;
            _fishHook.ItemCollected += OnItemCollected;
        }

        private void UnSubscribe()
        {
            w51InputController.ShootButtonDown -= OnShootButtonDown;
            w51InputController.ShootButtonUp -= OnShootButtonUp;
            w51InputController.LeftButtonDown -= OnLeftButtonDown;
            w51InputController.LeftButtonUp -= OnLeftButtonUp;
            w51InputController.RightButtonDown -= OnRightButtonDown;
            w51InputController.RightButtonUp -= OnRightButtonUp;

            _fishHook.Launched -= OnFishHookLaunched;
            _fishHook.ItemCaught -= OnItemCaught;
            _fishHook.ItemCollected -= OnItemCollected;
        }
        private void OnFishHookLaunched()
        {
            FishHookLaunched?.Invoke();
        }

        private void OnItemCaught(List<CatchableItem> item)
        {
            ItemCaught?.Invoke(item);
        }

        private void OnItemCollected(List<CatchableItem> item)
        {
            ItemCollected?.Invoke(item);
        }

        private void OnShootButtonDown()
        {
            Taptic.Light();
            _fishHook.Throw(_comboBonusMultiplier);
        }

        private void OnShootButtonUp()
        {
            _fishHook.PullBack();
        }

        private void OnLeftButtonDown()
        {
            if (_rightButtonPressed)
                _rightButtonPressed = false;

            Taptic.Light();
            _boatModel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            _leftButtonPressed = true;
        }

        private void OnLeftButtonUp()
        {
            _leftButtonPressed = false;
        }

        private void OnRightButtonDown()
        {
            if (_leftButtonPressed)
                _leftButtonPressed = false;

            Taptic.Light();
            _boatModel.transform.rotation = Quaternion.identity;

            _rightButtonPressed = true;
        }

        private void OnRightButtonUp()
        {
            _rightButtonPressed = false;
        }

        #endregion


        public void OnFinish()
        {
            UnSubscribe();
            Active = false;
            //_ropeRenderer.enabled = false;
            _fishHook.OnFinish();
        }

        public void EnableComboBonus(int combo)
        {
            _comboBonusMultiplier = 1f + combo * 0.15f;
            _comboBonusEnabled = true;
        }

        public void DisableComboBonus()
        {
            _comboBonusMultiplier = 1f;
            _comboBonusEnabled = false;
        }
    }
}

