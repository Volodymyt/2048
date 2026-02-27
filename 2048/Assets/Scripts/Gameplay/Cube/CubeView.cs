using Cubes.Merge;
using TMPro;
using UnityEngine;
using Zenject;

namespace Gameplay.Cube
{
    public class CubeView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private MeshRenderer _renderer;
        
        public int Value { get; private set; }
        public bool IsMerging { get; private set; }
        
        private Rigidbody _rb;
        private MergeService _mergeService;
        private CubeConfig _cubeConfig;

        [Inject]
        public void Construct(MergeService mergeService, CubeConfig cubeConfig)
        {
            _mergeService = mergeService;
            _cubeConfig = cubeConfig;
        }
        
        private void Awake() => _rb = GetComponent<Rigidbody>();

        public void Init(int value, CubeConfig config)
        {
            Value = value;
            _label.text = value.ToString();
            int colorIndex = (int)Mathf.Log(value, 2) - 1;
            _renderer.material.color = config.PowerOfTwoColors[colorIndex];
        }

        public void Launch(Vector3 force) => _rb.AddForce(force, ForceMode.Impulse);

        private void SetMerging(bool state)
        {
            IsMerging = state;
            _rb.isKinematic = state;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (IsMerging) return; 
            if (!collision.gameObject.TryGetComponent<CubeView>(out var other)) return;

            float impulse = collision.impulse.magnitude;
            if (impulse < _cubeConfig.MinMergeImpulse) return;

            Vector3 directionToOther = (other.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(collision.impulse.normalized, directionToOther);
            if (dot < 0.3f) return;

            if (_mergeService.TryMerge(this, other, out var survivor))
            {
                survivor.SetMerging(true);
                other.SetMerging(true);

                int newValue = survivor.Value * 2;
                survivor.Init(newValue, _cubeConfig);
    
                Vector3 mergeImpulseDir = (survivor.transform.position - other.transform.position).normalized;
                survivor.SetMerging(false);
                survivor.Launch((mergeImpulseDir * _cubeConfig.MergeImpulseForce) / 10);
    
                _mergeService.UnregisterCube(other);
                Object.Destroy(other.gameObject);
            }
        }
    }
}