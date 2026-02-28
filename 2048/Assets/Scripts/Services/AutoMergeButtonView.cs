using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class AutoMergeButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        public void SetInteractable(bool state) => _button.interactable = state;

        public void OnClick(System.Action callback) => _button.onClick.AddListener(() => callback());
    }
}