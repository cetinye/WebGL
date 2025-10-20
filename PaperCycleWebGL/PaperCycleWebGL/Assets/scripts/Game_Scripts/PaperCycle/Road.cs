using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_PaperCycle
{
    public class Road : MonoBehaviour
    {
        [SerializeField] private List<Transform> _housePoints;

        public const float Length = 10.7f;

        public void AddHouses()
        {
            var housePrefabs = GameManager.PrefabData.GetRandomHouses(2);
            var level = GameManager.Instance.PlayerLevel;

            for (int i = 0; i < housePrefabs.Count; i+=2)
            {
                var house1 = Instantiate(housePrefabs[i], transform);
                var house2 = Instantiate(housePrefabs[i+1], transform);
                var colors = GameManager.RequestAssetData.GetRandomHouseColors(2, level);
                
                house1.transform.position =
                    house2.transform.position = 
                        _housePoints[i / 2].position;

                house1.ColorData = colors[0];
                house2.ColorData = colors[1];
            }
        }
    }
}