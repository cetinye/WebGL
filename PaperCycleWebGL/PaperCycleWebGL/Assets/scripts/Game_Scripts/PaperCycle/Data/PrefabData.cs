using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_PaperCycle
{
    [CreateAssetMenu(menuName = MenuName)]
    public class PrefabData : ScriptableObject
    {
        private const string MenuName = "Data/Paper Cycle/PrefabData";

        [SerializeField] private GameObject _paperPrefab;
        [SerializeField] private Road _roadPrefab;
        [SerializeField] private List<HouseBehaviour> _housePrefabs;
        public GameObject PaperPrefab => _paperPrefab;
        public Road RoadPrefab => _roadPrefab;

        public List<HouseBehaviour> GetRandomHouses(int rowCount)
        {
            var leftHouses = _housePrefabs.Where(h => h.HouseAlignment is HouseAlignment.Left).ToList();
            var rightHouses = _housePrefabs.Where(h => h.HouseAlignment is HouseAlignment.Right).ToList();

            var houses = new List<HouseBehaviour>();
            for (var i = 0;
                 i < rowCount && leftHouses.Count > 0 && rightHouses.Count > 0;
                 i++)
            {
                var leftRoll = Random.Range(0, leftHouses.Count);
                var rightRoll = Random.Range(0, rightHouses.Count);
                houses.Add(leftHouses[leftRoll]);
                leftHouses.RemoveAt(leftRoll);
                houses.Add(rightHouses[rightRoll]);
                rightHouses.RemoveAt(rightRoll);
            }

            return houses;
        }
    }
}