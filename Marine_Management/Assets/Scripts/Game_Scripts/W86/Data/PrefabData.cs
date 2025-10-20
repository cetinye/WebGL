using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Witmina_MarineManagement
{
    [CreateAssetMenu(menuName = MenuName)]
    public class PrefabData : ScriptableObject
    {
        private const string MenuName = "Data/Marine_Management/PrefabData";

        public List<Boat> BoatPrefabs;
        public List<RequestData> RequestSpritesData;
        public Transform FeedbackSpriteSuccess;
        public Transform FeedbackSpriteFail;

        public Boat GetRandomBoat(int tier, Boat last = null)
        {
            var list = BoatPrefabs.Where(boat => boat.Tier <= tier).ToList();
            if (last && BoatPrefabs.Count > 2)
            {
                list = list.Where(boat => boat != last).ToList();
            }

            return list[Random.Range(0, list.Count)];
        }

        public Boat GetRandomBoat(BoatType type)
        {
            var list = BoatPrefabs.Where(boat => boat.Type == type).ToList();
            if (list.Count < 0)
                return null;

            return list[Random.Range(0, list.Count)];
        }

        public RequestData GetRandomRequestData(int maxIndex)
        {
            var list = RequestSpritesData.ToList();
            return list[Random.Range(0, maxIndex)];
        }

        public Transform GetFeedbackSprite(bool success)
        {
            return success ? FeedbackSpriteSuccess : FeedbackSpriteFail;
        }

        public List<RequestData> GetRequestData()
        {
            List<RequestData> list = RequestSpritesData.ToList();
            return list;
        }
    }
}

