using System.Collections.Generic;
using Gameplay.Cube;
using UnityEngine;

namespace Gameplay
{
    public class MergeSystem
    {
        private readonly List<CubeView> _cubesOnField = new();

        public void RegisterCube(CubeView cube) => _cubesOnField.Add(cube);
        public void UnregisterCube(CubeView cube) => _cubesOnField.Remove(cube);
        public void Clear() => _cubesOnField.Clear();
        
        public IReadOnlyList<CubeView> Cubes => _cubesOnField;
        
        public bool TryMerge(CubeView a, CubeView b, out CubeView result)
        {
            result = null;

            if (a.IsMerging || b.IsMerging) return false;
            if (a.Value != b.Value) return false;

            result = a;
            return true;
        }
        
        public void WakeUpAll()
        {
            foreach (var cube in _cubesOnField)
                cube.WakeUp();
        }
        
        public void CheckNeighboursAfterMerge(CubeView newCube)
        {
            for (int i = _cubesOnField.Count - 1; i >= 0; i--)
            {
                var cube = _cubesOnField[i];

                bool canMerge = cube != newCube
                                && !cube.IsMerging
                                && cube.Value == newCube.Value
                                && Vector3.Distance(cube.transform.position, newCube.transform.position) < 1.1f;

                if (!canMerge) continue;

                newCube.MergeWith(cube);
                return;
            }
        }
    }
}