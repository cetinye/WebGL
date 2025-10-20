using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_PaperCycle
{
    [CreateAssetMenu(menuName = MenuName)]
    public class RequestAssetData : ScriptableObject
    {
        private const string MenuName = "Data/Paper Cycle/RequestAssetData";
        
        [SerializeField] private List<AlignmentSpriteData> _alignmentSprites;
        [SerializeField] private List<HouseColorData> _houseColors;

        public Sprite GetAlignmentSprite(HouseAlignment alignment)
        {
            var sprite = _alignmentSprites.FirstOrDefault(a => a.Alignment == alignment);
            return sprite?.Sprite;
        }

        public Color GetHouseColor(HouseColor colorName)
        {
            return _houseColors.First(h => h.Name == colorName).Color;
        }

        public List<HouseColorData> GetRandomHouseColors(int count, int playerLevel)
        {
            playerLevel = Mathf.Clamp(playerLevel, 1, 25);

            var candidateList = _houseColors.Where(h => h.MinLevel <= playerLevel).ToList();
            var list = new List<HouseColorData>();
            for (int i = 0; i < count; i++)
            {
                var index = Random.Range(0, candidateList.Count);
                list.Add(candidateList[index]);
                candidateList.RemoveAt(index);
            }

            return list;
        }
    }
}