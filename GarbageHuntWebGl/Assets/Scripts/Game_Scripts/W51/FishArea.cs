using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    public enum FishDirection
    {
        LeftRight,
        RightLeft,
    }

    public class FishAreaException : Exception
    {
        public FishAreaException(string s) : base(s) { }
    }

    [RequireComponent(typeof(BoxCollider))]
    public class FishArea : MonoBehaviour
    {
        [SerializeField] private float _fishBaseMoveSpeed = 1f;
        [SerializeField] private FishDirection _direction;
        public event Action<FishArea, Fish, bool> FishComplete;

        private BoxCollider _collider;
        private List<Fish> _fishes = new();

        private static readonly int MaxFishesPerArea = 2;

        public int FishCount => _fishes.Count;

        #region Unity Methods
        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
        }

        private void OnDestroy()
        {
            foreach (var fish in _fishes)
            {
                fish.FishComplete -= OnFishComplete;
            }
        }
        #endregion


        //Method for adding fish.
        public void AddFish(Fish fish)
        {
            if (_fishes.Contains(fish))
            {
                throw new FishAreaException($"Area {name} was tried given a duplicate fish \"{fish}\"");
            }

            if (_fishes.Count == MaxFishesPerArea)
            {
                throw new FishAreaException($"Area {name} was tried given too much fish \"{fish}\"");
            }

            var bounds = _collider.bounds;
            var isLtoR = _direction is FishDirection.LeftRight;
            var sign = isLtoR ? 1 : -1;
            var startX = isLtoR ? bounds.min.x : bounds.max.x;

            // Add a random offset
            startX -= UnityEngine.Random.Range(0f, 1f) * sign;

            // Shift the fish until it doesnt overlap with any other fish
            for (int i = 0;
                 i < 12 &&
                 _fishes.Any(f => Mathf.Abs(f.transform.localPosition.x - startX) < 0.6f);
                 i++)
            {
                startX -= UnityEngine.Random.Range(4.4f, 15.8f) * sign;
            }


            var endX = isLtoR ? bounds.max.x : bounds.min.x;
            var srcPos = new Vector3(startX, bounds.center.y, bounds.center.z);

            _fishes.Add(fish);
            fish.FishComplete += OnFishComplete;
            fish.MoveX(endX, _fishBaseMoveSpeed);

            fish.transform.SetParent(transform);
            fish.transform.position = srcPos;
            fish.transform.rotation = Quaternion.Euler(0f, isLtoR ? 0f : 180f, 0f);
        }

        public void RemoveFish(Fish fish)
        {
            if (!_fishes.Contains(fish))
            {
                return;
                throw new FishAreaException($"Area {name} does not contain \"{fish}\"");
            }
            fish.FishComplete -= OnFishComplete;
            _fishes.Remove(fish);
        }

        public void RemoveFish()
        {
            if (_fishes.Count < 1)
            {
                throw new FishAreaException($"Area {name} does not contain any fish, but tried to remove");
            }
            RemoveFish(_fishes.Last());
        }

        public void Clear()
        {
            for (int i = 0; i < _fishes.Count; i++)
            {
                Destroy(_fishes[i].gameObject);
            }
            _fishes.Clear();
        }

        private void OnFishComplete(Fish fish, bool success)
        {
            fish.FishComplete -= OnFishComplete;
            FishComplete?.Invoke(this, fish, success);
        }

        public int GetFishCount()
        {
            return _fishes.Count;
        }
    }

}
