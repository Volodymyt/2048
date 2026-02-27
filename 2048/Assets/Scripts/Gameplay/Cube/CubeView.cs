using TMPro;
using UnityEngine;

namespace Gameplay.Cube
{
    public class CubeView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private MeshRenderer _renderer;

        public int Value { get; private set; }
        public bool IsMerging { get; private set; }
        private Rigidbody _rb;

        private void Awake() => _rb = GetComponent<Rigidbody>();

        public void Init(int value, CubeConfig config)
        {
            Value = value;
            _label.text = value.ToString();
            int colorIndex = (int)Mathf.Log(value, 2) - 1;
            _renderer.material.color = config.PowerOfTwoColors[colorIndex];
        }

        public void Launch(Vector3 force) => _rb.AddForce(force, ForceMode.Impulse);

        public void SetMerging(bool state)
        {
            IsMerging = state;
            _rb.isKinematic = state;
        }
    }
}