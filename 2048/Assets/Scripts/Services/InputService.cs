using System;
using UnityEngine;
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
            if (Input.touchCount > 0)
            {
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
                        _isDragging = false;
                        OnFingerUp?.Invoke();
                        break;
                }
            }
        }

        public void Dispose()
        {
            OnFingerDown = null;
            OnFingerUp = null;
            OnFingerDrag = null;
        }
    }
}