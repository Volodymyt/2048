using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _finalScoreLabel;
        [SerializeField] private Button _restartButton;

        public Button RestartButton => _restartButton;

        public void Show(int score)
        {
            _finalScoreLabel.text = $"Score: {score}";
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}