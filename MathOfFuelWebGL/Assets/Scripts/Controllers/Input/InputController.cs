using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Witmina_InputController
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance = null;
        
        [SerializeField] private float _moveTreshold = 0.05f;
        public event Action<PointerEventData> Pressed;
        public event Action<PointerEventData> Moved;
        public event Action<PointerEventData> Released;
        
        private Vector3 _previousPos;

        private void Awake()
        {
            if(Instance)
                Destroy(Instance);

            Instance = this;
        }
        /*void Update()
        {
            var inputPos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                _previousPos = inputPos;
                Pressed?.Invoke(inputPos);
            }
            if (Input.GetMouseButton(0))
            {
                if (Vector3.Distance(_previousPos, inputPos) > _moveTreshold)
                    Moved?.Invoke(inputPos);
            }
            if (Input.GetMouseButtonUp(0))
            {
                Released?.Invoke(inputPos);
            }
        }*/

        public void OnPressed(BaseEventData eData)
        {
            if (eData is not PointerEventData pData)
                return;
            
            _previousPos = pData.position;
            Pressed?.Invoke(pData);
        }

        public void OnMoved(BaseEventData eData)
        {
            if (eData is not PointerEventData pData)
                return;
            
            if (Vector3.Distance(_previousPos, pData.position) > _moveTreshold)
                Moved?.Invoke(pData);
        }

        public void OnReleased(BaseEventData eData)
        {
            if (eData is not PointerEventData pData)
                return;
            
            Released?.Invoke(pData);
        }
    }
}

