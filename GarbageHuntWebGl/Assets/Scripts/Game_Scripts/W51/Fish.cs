using System;
using DG.Tweening;
using UnityEngine;

namespace Witmina_GarbageHunt
{

    public class Fish : CatchableItem
    {
        public bool Swimming { get; private set; }
        public event Action<Fish, bool> FishComplete;
        private float _endX;
        private float _baseMoveSpeed = 1f;

        private void Update()
        {
            if (!Swimming)
                return;

            var pos = transform.position;
            var diff = _endX - pos.x;
            if (Mathf.Abs(diff) > 0.15f)
            {
                transform.position += Vector3.right * (Mathf.Sign(diff) * Time.deltaTime * _baseMoveSpeed * LevelBehaviour.LevelSO.topSpeedFish);
            }
            else
            {
                Swimming = false;
                FishComplete?.Invoke(this, true);
            }


        }

        public void MoveX(float endX, float baseMoveSpeed)
        {
            _endX = endX;
            _baseMoveSpeed = baseMoveSpeed;
            Swimming = true;
        }

        public void ResetFish()
        {
            gameObject.SetActive(false);
            transform.position = -100f * Vector3.one;
        }

        public override void Catch()
        {
            Swimming = false;
            FishComplete?.Invoke(this, false);
        }
    }
}

