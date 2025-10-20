using System;
using UnityEngine;

namespace Witmina_MarineManagement
{
    [Serializable]
    public sealed class AudioFxData
    {
        public AudioFxType Type;
        public AudioClip Clip;
    }

    [Serializable]
    public sealed class ServiceAudioFxData
    {
        public RequestType Type;
        public AudioClip Clip;
    }

    [Serializable]
    public sealed class RequestData
    {
        public RequestType RequestType;
        public Sprite RequestSprite;
    }

    [Serializable]
    public sealed class PowerUpIconData
    {
        public PowerUpType PowerUpType;
        public GameObject GameObject;
    }
}
