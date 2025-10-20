using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    [RequireComponent(typeof(BoxCollider))]
    public class GarbageController : MonoBehaviour
    {
        public event Action GarbageSpawned;
        public event Action GarbageSunk;

        [SerializeField] private float _garbageDuration = 10f;
        [SerializeField] private float _baseSpawnInterval = 3f;
        [SerializeField] private List<Garbage> _garbagePrefabs;
        [SerializeField] private List<Garbage> _staticGarbagePrefabs;
        [SerializeField] private List<Transform> _staticGarbagePoints;
        [SerializeField] private RectTransform _offScreenIconPrefab;
        [SerializeField] private RectTransform _offScreenPanel;
        [SerializeField] private Camera _mainCamera;

        public int GarbageCount => _garbages.Count;
        public bool Spawning;

        private List<Garbage> _garbages;
        private List<Garbage> _staticGarbages;
        private List<RectTransform> _offScreenIcons;

        private float xMax;
        private float xMin;
        private float yMax;
        private float yMin;
        private float zPos;

        private Coroutine _garbageSpawnRoutine;

        private void Update()
        {
            if (!_mainCamera || !_mainCamera.enabled || !Spawning)
                return;

            for (int i = 0; i < _offScreenIcons.Count; i++)
            {
                var icon = _offScreenIcons[i];
                if (i >= _garbages.Count)
                {
                    icon.gameObject.SetActive(false);
                    continue;
                }

                var screenPos = _mainCamera.WorldToViewportPoint(_garbages[i].transform.position);
                if (screenPos.x is > -0.04f and < 1.04f)
                {
                    icon.gameObject.SetActive(false);
                    continue;
                }

                var iconX = Mathf.Clamp(screenPos.x, 0.06f, 0.94f);
                var iconPos = new Vector3(iconX, screenPos.y, screenPos.z);
                icon.position = _mainCamera.ViewportToScreenPoint(iconPos);
                icon.rotation = Quaternion.Euler(0f, 0f, iconX < 0.5f ? 180f : 0f);
                icon.gameObject.SetActive(true);
            }
        }

        public void Initialize()
        {
            // Clean up
            if (_garbages != null)
            {
                foreach (var g in _garbages)
                {
                    Destroy(g.gameObject);
                }
                _garbages.Clear();
            }
            else
            {
                _garbages = new();
            }

            // Search (or reset) the garbages on the ground
            if (_staticGarbages != null)
            {
                foreach (var sg in _staticGarbages)
                {
                    sg.transform.SetParent(transform);
                    sg.ResetGarbage();
                }
            }
            else
            {
                SpawnStaticGarbages();
                CancelInvoke(nameof(SpawnStaticGarbage));
                InvokeRepeating(nameof(SpawnStaticGarbage), LevelBehaviour.LevelSO.garbageOnSeabedRenewalInterval, LevelBehaviour.LevelSO.garbageOnSeabedRenewalInterval);
            }


            GameManager.Instance.SetTotalGarbageCount(_staticGarbages.Count);

            // Calcualte bounds
            var bounds = GetComponent<BoxCollider>().bounds;
            xMax = bounds.max.x;
            xMin = bounds.min.x;
            yMax = bounds.max.y;
            yMin = bounds.min.y;
            zPos = bounds.center.z;

            Spawning = false;

            _garbageSpawnRoutine = StartCoroutine(GarbageSpawnRoutine());

            //Pool off screen icons
            if (_offScreenIcons == null)
            {
                _offScreenIcons = new();
                for (int i = 0; i < LevelBehaviour.LevelSO.maxGarbageCount; i++)
                {
                    var icon = Instantiate(_offScreenIconPrefab, _offScreenPanel);
                    icon.gameObject.SetActive(false);
                    _offScreenIcons.Add(icon);
                }
            }
        }

        private void SpawnStaticGarbages()
        {
            _staticGarbages = new List<Garbage>();

            for (int i = 0; i < LevelBehaviour.LevelSO.garbageOnSeabed; i++)
            {
                _staticGarbages.Add(Instantiate(_staticGarbagePrefabs[i % _staticGarbagePrefabs.Count], _staticGarbagePoints[i % _staticGarbagePoints.Count].position, Quaternion.identity, transform));
                GameManager.Instance.IncrementStaticGarbageCount();
            }
        }

        public void SpawnStaticGarbage()
        {
            _staticGarbages.Add(Instantiate(_staticGarbagePrefabs[UnityEngine.Random.Range(0, _staticGarbagePrefabs.Count)], _staticGarbagePoints[UnityEngine.Random.Range(0, _staticGarbagePoints.Count)].position, Quaternion.identity, transform));
            GameManager.Instance.IncrementStaticGarbageCount();
        }

        public void OnFinish()
        {
            Spawning = false;

            foreach (var g in _garbages)
            {
                g.GarbageFinished -= OnGarbageFinished;
                Destroy(g.gameObject);
            }
            _garbages.Clear();

            if (_garbageSpawnRoutine != null)
                StopCoroutine(_garbageSpawnRoutine);

            foreach (var icon in _offScreenIcons)
            {
                icon.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // OnDestroy event cleanup
            foreach (var garbage in _garbages)
            {
                garbage.GarbageFinished -= OnGarbageFinished;
            }

            StopAllCoroutines();
        }

        public void RemoveGarbage(Garbage garbage)
        {
            if (!_garbages.Contains(garbage))
                return;

            garbage.GarbageFinished -= OnGarbageFinished;
            _garbages.Remove(garbage);
        }

        private void OnGarbageFinished(Garbage garbage, bool success)
        {
            if (!_garbages.Contains(garbage))
                return;

            if (!success)
            {
                GarbageSunk?.Invoke();
            }
        }

        private void SpawnGarbage()
        {
            var xPos = UnityEngine.Random.Range(xMin, xMax);
            var pos = new Vector3(xPos, yMax, zPos);
            var prefab = _garbagePrefabs[UnityEngine.Random.Range(0, _garbagePrefabs.Count)];
            var garbage = Instantiate(prefab, pos, Quaternion.identity, transform);

            _garbages.Add(garbage);
            garbage.GarbageFinished += OnGarbageFinished;
            garbage.Release(yMin, _garbageDuration);
            GarbageSpawned?.Invoke();
        }

        #region Coroutines
        private IEnumerator GarbageSpawnRoutine()
        {
            while (true)
            {
                if (Spawning && _garbages.Count < LevelBehaviour.LevelSO.maxGarbageCount)
                {
                    float interval = LevelBehaviour.LevelSO.garbageSpawnInterval;
                    yield return new WaitForSeconds(interval);
                    SpawnGarbage();
                }
                yield return null;
            }
        }
        #endregion

    }

}
