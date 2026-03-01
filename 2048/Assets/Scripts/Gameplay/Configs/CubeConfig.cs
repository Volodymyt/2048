using UnityEngine;

namespace Gameplay.Configs
{
    [CreateAssetMenu(fileName = "CubeConfig", menuName = "Configs/CubeConfig")]
    public class CubeConfig : ScriptableObject
    {
        [field: SerializeField] public float LaunchForce { get; private set; } = 30f;
        [field: SerializeField] public float MergeImpulseForce { get; private set; } = 5f;
        [field: SerializeField] public float MergeJumpForce { get; private set; } = 4f;
        [field: SerializeField] public float MinMergeVelocity { get; private set; } = 0.5f;
        
        [field: SerializeField] public float DragSensitivity { get; private set; } = 0.01f;
        [field: SerializeField] public Color[] PowerOfTwoColors { get; private set; }
    }
}