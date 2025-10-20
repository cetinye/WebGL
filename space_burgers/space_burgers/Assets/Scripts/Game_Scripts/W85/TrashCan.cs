using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public class TrashCan : MonoBehaviour
    {
        [SerializeField] private Transform _topLid;
        [SerializeField] private Transform _botLid;
        [SerializeField] private float duration;

        private float _lidValue;
        public float LidValue
        {
            get => _lidValue;
            set
            {
                _lidValue = value;
                _topLid.localScale = new Vector3(1f, _lidValue, 1f);
                _botLid.localScale = new Vector3(1f, _lidValue, 1f);
            }
        }

        private bool _wasOpen;
        public bool Open { get; set; }

        private bool _audioPlayed;

        private void Awake()
        {
            Open = false;
            _wasOpen = false;
        }

        private void Update()
        {
            if (Open)
            {
                if (!_audioPlayed)
                {
                    _audioPlayed = true;
                    GameManager.PlayAudioFx(AudioFxType.TrashOpen);
                }
                if (LidValue > 0)
                    LidValue -= (2f / (duration)) * Time.deltaTime;
                else
                    LidValue = 0;
                Open = false;
            }
            else
            {
                if (_audioPlayed)
                {
                    _audioPlayed = false;
                    GameManager.PlayAudioFx(AudioFxType.TrashClose);
                }
                if (LidValue < 1)
                    LidValue += (2f / (duration)) * Time.deltaTime;
                else
                    LidValue = 1;
            }
        }

        private void LateUpdate()
        {
            if (_wasOpen == Open)
                return;
            
            _wasOpen = Open;
            GameManager.PlayAudioFx(Open ? AudioFxType.TrashOpen : AudioFxType.TrashClose);

        }
    }
}

