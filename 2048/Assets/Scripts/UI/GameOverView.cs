using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private TMP_Text finalScoreLabel;
        [SerializeField] private Button restartButton;

        public Button RestartButton => restartButton;

        public void Show(int score)
        {
            finalScoreLabel.text = $"Score: {score}";
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}