using System.Collections.Generic;
using UnityEngine;

namespace Witmina_rotf
{
    [CreateAssetMenu(menuName = MenuName)]
    public class LevelData : ScriptableObject
    {
        private const string MenuName = "Data/RhythmOfForest/LevelData";

        [SerializeField] private List<TextAsset> steadyThreeRhythms;
        [SerializeField] private List<TextAsset> syncopatedThreeRhythms;
        [SerializeField] private List<TextAsset> steadyFourRhythms;
        [SerializeField] private List<TextAsset> syncopatedFourRhythms;
        [SerializeField] private List<TextAsset> steadyFiveRhythms;
        [SerializeField] private List<TextAsset> syncopatedFiveRhythms;
        [SerializeField] private List<TextAsset> steadySixRhythms;
        [SerializeField] private List<TextAsset> syncopatedSixRhythms;
        [SerializeField] private List<TextAsset> steadySevenRhythms;
        [SerializeField] private List<TextAsset> syncopatedSevenRhythms;
        [SerializeField] private List<TextAsset> steadyEightRhythms;
        [SerializeField] private List<TextAsset> syncopatedEightRhythms;

        public TextAsset GetLevelData(int rhythmCount, int steadyOrSyncopated)
        {
            if (steadyOrSyncopated == 1)
            {
                return rhythmCount switch
                {
                    3 => steadyThreeRhythms[Random.Range(0, steadyThreeRhythms.Count)],
                    4 => steadyFourRhythms[Random.Range(0, steadyFourRhythms.Count)],
                    5 => steadyFiveRhythms[Random.Range(0, steadyFiveRhythms.Count)],
                    6 => steadySixRhythms[Random.Range(0, steadySixRhythms.Count)],
                    7 => steadySevenRhythms[Random.Range(0, steadySevenRhythms.Count)],
                    8 => steadyEightRhythms[Random.Range(0, steadyEightRhythms.Count)],
                    _ => null,
                };
            }
            else
            {
                return rhythmCount switch
                {
                    3 => syncopatedThreeRhythms[Random.Range(0, syncopatedThreeRhythms.Count)],
                    4 => syncopatedFourRhythms[Random.Range(0, syncopatedFourRhythms.Count)],
                    5 => syncopatedFiveRhythms[Random.Range(0, syncopatedFiveRhythms.Count)],
                    6 => syncopatedSixRhythms[Random.Range(0, syncopatedSixRhythms.Count)],
                    7 => syncopatedSevenRhythms[Random.Range(0, syncopatedSevenRhythms.Count)],
                    8 => syncopatedEightRhythms[Random.Range(0, syncopatedEightRhythms.Count)],
                    _ => null,
                };
            }
        }
    }
}