using System.Collections.Generic;
using Gameplay.Cube;

namespace Cubes.Merge
{
    public class MergeService
    {
        private readonly List<CubeView> _cubesOnField = new();

        public void RegisterCube(CubeView cube) => _cubesOnField.Add(cube);
        public void UnregisterCube(CubeView cube) => _cubesOnField.Remove(cube);

        public bool TryMerge(CubeView a, CubeView b, out CubeView result)
        {
            result = null;

            if (a.IsMerging || b.IsMerging) return false;
            if (a.Value != b.Value) return false;

            result = a;
            return true;
        }
    }
}