using Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreLabel;

        public void UpdateScore(int score) => _scoreLabel.text = score.ToString();
    }
}