using UnityEngine;

namespace Gameplay.Configs
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Configs/BoardConfig")]
    public class BoardConfig : ScriptableObject
    {
        [field: SerializeField] public float BoardHalfWidth { get; private set; } = 2.5f;
        [field: SerializeField] public Vector3 SpawnPoint { get; private set; } = new(0, -2.288f, -22.27f);
    }
}