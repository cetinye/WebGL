using System;
using UnityEngine;

namespace Witmina_PaperCycle
{
    [Serializable]
    public sealed class AudioFxData
    {
        public AudioFxType Type;
        public AudioClip Clip;
    }

    [Serializable]
    public sealed class HouseColorData
    {
        public int MinLevel = 1;
        public HouseColor Name;
        public Color Color;
    }

    [Serializable]
    public sealed class AlignmentSpriteData
    {
        public HouseAlignment Alignment;
        public Sprite Sprite;
    }

    public sealed class RequestData
    {
        public HouseColorData ColorData;
        public HouseAlignment Alignment;
        public bool Reversed;
    }
}