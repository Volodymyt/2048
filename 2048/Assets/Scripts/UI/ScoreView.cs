using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreLabel;

        public void UpdateScore(int score) => scoreLabel.text = score.ToString();
    }
}