using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    public class FishController : MonoBehaviour
    {
        [SerializeField] private List<Fish> _fishPrefabs;

        public bool Spawning = false;

        [SerializeField] private List<FishArea> _areas = new List<FishArea>();
        private List<Fish> _fishes;

        private float _spawnTimer;

        private int FishCount => _areas.Sum(a => a.FishCount);

        #region Unity Methods
        private void Update()
        {
            if (!Spawning)
                return;

            if (FishCount < LevelBehaviour.LevelSO.numOfFish)
            {
                if (_spawnTimer > 0)
                {
                    _spawnTimer -= Time.deltaTime;
                    return;
                }

                var idleList = _fishes.Where(f => !f.gameObject.activeSelf).ToList();
                if (idleList.Count < 1)
                    return;

                var idleFish = idleList[UnityEngine.Random.Range(0, idleList.Count)];
                if (!idleFish)
                    return;

                idleFish.gameObject.SetActive(true);
                PlaceFish(idleFish);
            }
        }

        private void OnDestroy()
        {
            foreach (var area in _areas)
            {
                area.FishComplete -= OnFishComplete;
            }
        }
        #endregion

        public void Initialize()
        {
            _fishes = new();
            //Pool fish objects for later
            for (int i = _fishes.Count; i < 8; i++)
            {
                var newFish = Instantiate(_fishPrefabs[i % _fishPrefabs.Count], -100f * Vector3.one, Quaternion.identity, transform);
                newFish.gameObject.SetActive(false);
                _fishes.Add(newFish);
            }

            foreach (var area in _areas)
            {
                area.FishComplete += OnFishComplete;
            }
        }

        public void OnFinish()
        {
            Spawning = false;
            foreach (var area in _areas)
            {
                area.Clear();
                area.FishComplete -= OnFishComplete;
            }
            _areas.Clear();
            _fishes.Clear();
        }

        private void PlaceFish(Fish fish)
        {
            FishArea minArea = _areas[UnityEngine.Random.Range(0, _areas.Count)];
            for (int i = 0; i < _areas.Count; i++)
            {
                if (_areas[i].FishCount < 2)
                {
                    minArea = _areas[i];
                    break;
                }
            }

            fish.gameObject.SetActive(true);
            minArea.AddFish(fish);
        }

        /*private void RemoveFish()
        {
            if (_fishes.Count <= 1)
                return;
            var maxCount = _areas.Max(a => a.FishCount);
            var maxArea = _areas.First(a => a.FishCount == maxCount);
            
            maxArea.RemoveFish();
        }*/

        #region Event Funcitons

        private void OnFishComplete(FishArea area, Fish fish, bool success)
        {
            if (success)
            {
                fish.ResetFish();
            }
            area.RemoveFish(fish);
        }

        #endregion
    }
}

