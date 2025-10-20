using System.Collections.Generic;
using UnityEngine;

namespace Witmina_AtomAlchemist
{
    [CreateAssetMenu(menuName = MenuName)]
    public class SpriteData : ScriptableObject
    {
        private const string MenuName = "Data/Atom Alchemist/SpriteData";

        public List<Sprite> ElementSprites;
        public List<Color> ElementColors;
        public List<string> ElementNames;
    }
}

