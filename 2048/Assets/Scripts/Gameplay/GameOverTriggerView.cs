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
            if (!other.TryGetComponent<CubeView>(out _)) return;
    
            var rb = other.GetComponent<Rigidbody>();
            if (rb == null) return;
    
            if (rb.linearVelocity.z < 0)
                OnGameOver?.Invoke();
        }
    }
}