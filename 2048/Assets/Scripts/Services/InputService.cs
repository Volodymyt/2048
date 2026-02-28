using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Services
{
    public class InputService : ITickable, IDisposable
    {
        public event Action OnFingerDown;
        public event Action OnFingerUp;
        public event Action<float> OnFingerDrag;

        private bool _isDragging;
        private float _previousX;

        public void Tick()
        {
#if UNITY_EDITOR
            HandleMouseInput();
#else
    HandleTouchInput();
#endif
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _previousX = touch.position.x;
                    _isDragging = true;
                    OnFingerDown?.Invoke();
                    break;
                case TouchPhase.Moved:
                    if (_isDragging)
                    {
                        float delta = touch.position.x - _previousX;
                        _previousX = touch.position.x;
                        OnFingerDrag?.Invoke(delta);
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (IsPointerOverUI()) return;
                    _isDragging = false;
                    OnFingerUp?.Invoke();
                    break;
            }
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _previousX = Input.mousePosition.x;
                _isDragging = true;
                OnFingerDown?.Invoke();
            }
            else if (Input.GetMouseButton(0) && _isDragging)
            {
                float delta = Input.mousePosition.x - _previousX;
                _previousX = Input.mousePosition.x;
                OnFingerDrag?.Invoke(delta);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (IsPointerOverUI()) return;
                _isDragging = false;
                OnFingerUp?.Invoke();
            }
        }

        private bool IsPointerOverUI()
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
    
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
    
            return results.Any(r => r.gameObject.GetComponent<Button>() != null);
        }
        
        public void Dispose()
        {
            OnFingerDown = null;
            OnFingerUp = null;
            OnFingerDrag = null;
        }
    }
}