using UnityEngine;

namespace Gameplay.Cube
{
    [CreateAssetMenu(fileName = "CubeConfig", menuName = "Configs/CubeConfig")]
    public class CubeConfig : ScriptableObject
    {
        [field: SerializeField] public float LaunchForce { get; private set; } = 10f;
        [field: SerializeField] public float MinMergeImpulse { get; private set; } = 0.1f;
        [field: SerializeField] public float MergeImpulseForce { get; private set; } = 5f;
        [field: SerializeField] public Color[] PowerOfTwoColors { get; private set; }
    }
}