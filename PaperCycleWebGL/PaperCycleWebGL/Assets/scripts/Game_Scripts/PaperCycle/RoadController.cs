using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_PaperCycle
{
    public class RoadController : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private int _initialRoadCount = 3;
        [SerializeField] private float _distanceToSpawn = 20;
        [SerializeField] private float _distanceToDestroy = 10;

        public bool Active;

        private List<Road> _roads = new();
        private void Update()
        {
            if (!Active || _roads == null || _roads.Count < 1)
                return;
            
            var lastRoadTransform = _roads.Last().transform;
            if (lastRoadTransform.position.z - _playerTransform.position.z < _distanceToSpawn)
            {
                AddRoad();
            }

            var firstRoadTransform = _roads.First().transform;
            if (_playerTransform.position.z - firstRoadTransform.position.z > _distanceToDestroy)
            {
                RemoveRoad();
            }
        }
        
        public void Initialize()
        {
            for (int i = 0; i < _roads.Count; i++)
            {
                Destroy(_roads[i].gameObject);
            }
            _roads.Clear();
            
            while (_roads.Count <= _initialRoadCount)
            {
                AddRoad(_roads.Count != 0);
            }
        }

        private void AddRoad(bool addHouses = true)
        {
            var road = Instantiate(GameManager.PrefabData.RoadPrefab, transform);
            
            var pos = _roads.Count == 0 ? transform.position :
                _roads.Last().transform.position + Road.Length * Vector3.forward;
            road.transform.position = pos;
            
            if(addHouses)
                road.AddHouses();
            
            _roads.Add(road);
            StaticBatchingUtility.Combine(road.gameObject);
        }

        private void RemoveRoad()
        {
            var road = _roads.First();
            _roads.RemoveAt(0);
            Destroy(road.gameObject);
        }
    }
}

