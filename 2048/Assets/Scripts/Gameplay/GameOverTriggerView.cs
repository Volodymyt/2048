using System;
using Gameplay.Cube;
using UnityEngine;

namespace Gameplay
{
    public class GameOverTriggerView : MonoBehaviour
    {
        public event Action OnGameOver;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<CubeView>(out var cube)) return;
            
            // перевіряємо що куб рухається назад (до гравця)
            var rb = other.GetComponent<Rigidbody>();
            if (rb == null) return;
            
            float dot = Vector3.Dot(rb.linearVelocity.normalized, -transform.forward);
            if (dot > 0.3f)
                OnGameOver?.Invoke();
        }
    }
}