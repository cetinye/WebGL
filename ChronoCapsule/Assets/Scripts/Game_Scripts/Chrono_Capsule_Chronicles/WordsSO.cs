using UnityEngine;

namespace Chrono_Capsule_Chronicles
{
    [CreateAssetMenu(fileName = "CCC_WordsSO", menuName = "Data/CCC/WordsSO")]
    public class WordsSO : ScriptableObject
    {
        public TextAsset threeLetters;
        public TextAsset fourLetters;
        public TextAsset fiveLetters;
        public TextAsset sixLetters;
        public TextAsset sevenLetters;
        public TextAsset eightLetters;
        public TextAsset nineLetters;
    }
}