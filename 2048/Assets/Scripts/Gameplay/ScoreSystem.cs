using System;
using UnityEngine;

namespace Gameplay
{
    public class ScoreSystem
    {
        public event Action<int> OnScoreChanged;
        
        public int Score { get; private set; }

        public void AddScore(int cubeValue)
        {
            Debug.Log(cubeValue);
            Score += cubeValue / 2;
            OnScoreChanged?.Invoke(Score);
        }

        public void Reset()
        {
            Score = 0;
            OnScoreChanged?.Invoke(Score);
        }
    }
}