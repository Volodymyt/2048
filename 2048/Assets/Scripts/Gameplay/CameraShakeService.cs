using Cysharp.Threading.Tasks;
using UnityEngine;

public class CameraShakeService
{
    private Camera _camera;
    private Vector3 _originalPosition;

    public void Initialize()
    {
        _camera = Camera.main;
        _originalPosition = _camera.transform.position;
    }

    public async UniTaskVoid Shake(float duration = 0.2f, float magnitude = 0.1f)
    {
        if (_camera == null) return;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            _camera.transform.position = _originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }
        _camera.transform.position = _originalPosition;
    }
}