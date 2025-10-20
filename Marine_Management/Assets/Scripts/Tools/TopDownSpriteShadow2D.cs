using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Witmina_Tools
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TopDownSpriteShadow2D : MonoBehaviour
    {
        [SerializeField] private Material _shadowMaterial;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private uint _orderDifference = 1;

        private SpriteRenderer _shadowRenderer;
        private Transform _transform;
        private Transform shadowTransform;

        private void Awake()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            _transform = transform;

            var shadow = new GameObject("Shadow");
            shadowTransform = shadow.transform;
            shadowTransform.SetParent(_transform.parent);
            shadowTransform.position = _transform.position + _offset;
            shadowTransform.rotation = _transform.rotation;
            shadowTransform.localScale = _transform.localScale;

            _shadowRenderer = shadow.AddComponent<SpriteRenderer>();
            _shadowRenderer.sprite = spriteRenderer.sprite;
            _shadowRenderer.material = _shadowMaterial;
            _shadowRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
            _shadowRenderer.sortingOrder = spriteRenderer.sortingOrder - (int)_orderDifference;
        }

        private void LateUpdate()
        {
            shadowTransform.position = _transform.position + _offset;
            shadowTransform.rotation = _transform.rotation;
            shadowTransform.localScale = _transform.localScale;
        }
    }

}