using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace Gameplay.Cube
{
    public class CubeView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private MeshRenderer _renderer;

        
        private IAssetProviderService _assetProviderService;
        private MergeService _mergeService;
        private CubeConfig _cubeConfig;
        private ScoreService _scoreService;
        private ParticleSystem _mergeParticles;
        private CameraShakeService _cameraShakeService;
        
        public int Value { get; private set; }
        public bool IsMerging { get; private set; }
        
        private Rigidbody _rb;

        [Inject]
        public void Construct(
            MergeService mergeService, 
            CubeConfig cubeConfig, 
            ScoreService scoreService,
            IAssetProviderService assetProviderService,
            CameraShakeService cameraShakeService)
        {
            _mergeService = mergeService;
            _cubeConfig = cubeConfig;
            _scoreService = scoreService;
            _assetProviderService = assetProviderService;
            _cameraShakeService = cameraShakeService;
            
            var particles = _assetProviderService.LoadAssetFromResources<ParticleSystem>("MergeParticlesView");
            _mergeParticles = Instantiate(particles, transform.position, Quaternion.identity);
            _mergeParticles.Stop();
            
            _cameraShakeService.Initialize();
        }
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.sleepThreshold = 0f;
        }

        public void Init(int value, CubeConfig config)
        {
            Value = value;
            _label.text = value.ToString();
            int colorIndex = (int)Mathf.Log(value, 2) - 1;
            _renderer.material.color = config.PowerOfTwoColors[colorIndex];
        }

        public void Launch(Vector3 force) => _rb.AddForce(force, ForceMode.Impulse);
        
        public void WakeUp() => _rb.WakeUp();
        
        public void SetKinematic(bool state) => _rb.isKinematic = state;

        private void SetMerging(bool state)
        {
            IsMerging = state;
            _rb.isKinematic = state;
        }
        
        public void PlayMergeAnimation()
        {
            transform.DOKill();
            transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 0.5f);
            _rb.AddForce(Vector3.up * _cubeConfig.MergeJumpForce, ForceMode.Impulse);
            
            _mergeParticles.transform.position = transform.position;
            _mergeParticles.Play();
            _cameraShakeService.Shake().Forget();
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (IsMerging) return;
            if (!collision.gameObject.TryGetComponent<CubeView>(out var other)) return;
            if (other.IsMerging) return;
            if (other.Value != Value) return;

            float relativeVelocity = collision.relativeVelocity.magnitude;
            if (relativeVelocity < _cubeConfig.MinMergeVelocity) return;

            if (_mergeService.TryMerge(this, other, out var survivor))
                survivor.MergeWith(this == survivor ? other : this);
        }
        
        public void MergeWith(CubeView other)
        {
            if (IsMerging || other.IsMerging) return;
    
            SetMerging(true);
            other.SetMerging(true);

            int newValue = Value * 2;
            Init(newValue, _cubeConfig);

            Vector3 mergeImpulseDir = (transform.position - other.transform.position).normalized;
            SetMerging(false);
            other.SetMerging(false);
            PlayMergeAnimation();
            Launch(mergeImpulseDir * _cubeConfig.MergeImpulseForce / 10);

            _mergeService.UnregisterCube(other);
            Destroy(other.gameObject);
            _mergeService.WakeUpAll();
            _mergeService.CheckNeighboursAfterMerge(this);
            _scoreService.AddScore(newValue);
        }
    }
}