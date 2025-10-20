using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Witmina_SweetMemory
{
    [CreateAssetMenu(menuName = MenuName)]
    public class CakeData : ScriptableObject
    {
        private const string MenuName = "Data/Sweet_Memory/CakeData";

        public List<Cake> CakePrefabs;
        public List<Color> Colors;
        public List<Sprite> Toppings;
        public List<Sprite> Candles;
        public List<Sprite> Numbered_Candles;

        public Cake GetRandomCake(int playerLevel)
        {
            var cakeList = CakePrefabs.Where(c => c.Level <= playerLevel).ToList();
            var cake = Instantiate(cakeList[Random.Range(0, cakeList.Count)]);

            var maxCakeColor = GameManager.LevelSO.cakeColorsMaxRange switch
            {
                2 => CakeFlavor.Strawberry,
                _ => CakeFlavor.Blueberry,
            };
            cake.Flavor = (CakeFlavor)Random.Range(0, (int)maxCakeColor + 1);

            var maxTopping = GameManager.LevelSO.fruitTypesOnCakeMaxRange switch
            {
                0 => ToppingType.None,
                3 => ToppingType.Strawberry,
                5 => ToppingType.Lemon,
                _ => throw new System.NotImplementedException(),
            };
            cake.ToppingType = (ToppingType)Random.Range(0, (int)maxTopping + 1);

            if (GameManager.LevelSO.numberedCandlesOnCake == 0 && GameManager.LevelSO.numOfCandlesOnTheCake == 0)
            {
                cake.CandleType = CandleType.None;
            }
            else if (GameManager.LevelSO.numOfCandlesOnTheCake == 5)
            {
                cake.CandleType = CandleType.Colored;
                cake.CandleColor = (CandleColor)Random.Range(0, (int)CandleColor.Blue + 1);
                cake.CandleCount = Random.Range(1, cake.MaxCandles + 1);
            }
            else if (GameManager.LevelSO.numOfCandlesOnTheCake == 5 && GameManager.LevelSO.numberedCandlesOnCake == 1)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    cake.CandleType = CandleType.Numbered;
                    cake.CandleNumber = Random.Range(0, 10);
                }
                else
                {
                    cake.CandleType = CandleType.Colored;
                    cake.CandleColor = (CandleColor)Random.Range(0, (int)CandleColor.Blue + 1);
                    cake.CandleCount = Random.Range(1, cake.MaxCandles + 1);
                }
            }
            else if (GameManager.LevelSO.numOfCandlesOnTheCake == 5 && GameManager.LevelSO.numOfCandlesOnTheCake == 2)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    cake.CandleType = CandleType.Numbered;
                    cake.CandleNumber = Random.Range(18, 60);
                }
                else
                {
                    cake.CandleType = CandleType.Colored;
                    cake.CandleColor = (CandleColor)Random.Range(0, (int)CandleColor.Blue + 1);
                    cake.CandleCount = Random.Range(1, cake.MaxCandles + 1);
                }
            }

            if (GameManager.LevelSO.cakePrice == 0)
            {
                cake.Price = 0;
            }
            else if (GameManager.LevelSO.cakePrice == 1)
            {
                cake.Price = Random.Range(8, 30);
            }

            return cake;
        }
    }
}

