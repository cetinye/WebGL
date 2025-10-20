using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Witmina_PaperCycle
{
    public class RequestUI : MonoBehaviour
    {
        [SerializeField] private Image _colorSprite;
        [SerializeField] private Image _alignmentSprite;
        [SerializeField] private Image _reverseSprite;

        public void Set(RequestData requestData)
        {
            if (requestData == null)
                return;
            
            _colorSprite.color = requestData.ColorData?.Color ?? Color.gray;
            var sprite = GameManager.RequestAssetData.GetAlignmentSprite(requestData.Alignment);
            if (sprite)
            {
                _alignmentSprite.sprite = sprite;
                _alignmentSprite.gameObject.SetActive(true);
            }
            else
            {
                _alignmentSprite.gameObject.SetActive(false);
            }
            _reverseSprite.gameObject.SetActive(requestData.Reversed);
        }
    }
}