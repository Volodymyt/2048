using UnityEngine;

namespace Gameplay.Cube
{
    [CreateAssetMenu(fileName = "CubeConfig", menuName = "Configs/CubeConfig")]
    public class CubeConfig : ScriptableObject
    {
        [field: SerializeField] public float LaunchForce { get; private set; } = 35f;
        [field: SerializeField] public float MergeImpulseForce { get; private set; } = 5f;
        [field: SerializeField] public float MergeJumpForce { get; private set; } = 10f;
        [field: SerializeField] public float MinMergeVelocity { get; private set; } = 0.5f;
        [field: SerializeField] public Color[] PowerOfTwoColors { get; private set; }
    }
}