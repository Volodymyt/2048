using DG.Tweening;
using Gameplay.Configs;
using Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace Gameplay.Cube
{
    public class CubeView : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] labels;
        [SerializeField] private MeshRenderer meshRenderer;

        
        private IAssetProviderService _assetProviderService;
        private MergeSystem _mergeSystem;
        private CubeConfig _cubeConfig;
        private ScoreSystem _scoreSystem;
        private CameraShake _cameraShake;
        
        private ParticleSystem _mergeParticles;
        private Collider _collider;
        
        public int Value { get; private set; }
        public bool IsMerging { get; private set; }
        
        private Rigidbody _rb;

        [Inject]
        public void Construct(
            MergeSystem mergeSystem, 
            CubeConfig cubeConfig, 
            ScoreSystem scoreSystem,
            IAssetProviderService assetProviderService,
            CameraShake cameraShake)
        {
            _mergeSystem = mergeSystem;
            _cubeConfig = cubeConfig;
            _scoreSystem = scoreSystem;
            _assetProviderService = assetProviderService;
            _cameraShake = cameraShake;
            
            var particles = _assetProviderService.LoadAssetFromResources<ParticleSystem>(Constants.MergeParticlesView);
            _mergeParticles = Instantiate(particles, transform.position, Quaternion.identity);
            _mergeParticles.Stop();
            
            _cameraShake.Initialize();
        }
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            
            _rb.sleepThreshold = 0f;
        }

        public void Init(int value, CubeConfig config)
        {
            Value = value;
            foreach (var label in labels)
                label.text = value.ToString();
            
            int colorIndex = Mathf.Clamp((int)Mathf.Log(value, 2) - 1, 0, config.PowerOfTwoColors.Length - 1);
            meshRenderer.material.color = config.PowerOfTwoColors[colorIndex];
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
            transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 0.5f)
                .OnComplete(() => transform.localScale = Vector3.one);
            _collider.enabled = false;
            _rb.AddForce(Vector3.up * _cubeConfig.MergeJumpForce, ForceMode.Impulse);
            
            _mergeParticles.transform.position = transform.position;
            _mergeParticles.Play();
            _cameraShake.Shake().Forget();
            
            DOVirtual.DelayedCall(0.4f, () => _collider.enabled = true);
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent<CubeView>(out var other)) return;

            bool canMerge = !IsMerging
                            && !other.IsMerging
                            && other.Value == Value
                            && collision.relativeVelocity.magnitude >= _cubeConfig.MinMergeVelocity;

            if (!canMerge) return;

            if (_mergeSystem.TryMerge(this, other, out var survivor))
                survivor.MergeWith(this == survivor ? other : this);
        }
        
        public void MergeWith(CubeView other)
        {
            if (IsMerging || other.IsMerging) return;

            ApplyMerge(other);
            DestroyMerged(other);
        }

        private void ApplyMerge(CubeView other)
        {
            SetMerging(true);
            other.SetMerging(true);

            int newValue = Value * 2;
            Init(newValue, _cubeConfig);

            Vector3 impulseDir = (transform.position - other.transform.position).normalized;

            SetMerging(false);
            other.SetMerging(false);

            PlayMergeAnimation();
            Launch(impulseDir * (_cubeConfig.MergeImpulseForce / 10));
            _scoreSystem.AddScore(newValue);
        }

        private void DestroyMerged(CubeView other)
        {
            _mergeSystem.UnregisterCube(other);
            Destroy(other.gameObject);
            _mergeSystem.WakeUpAll();
            _mergeSystem.CheckNeighboursAfterMerge(this);
        }
    }
}