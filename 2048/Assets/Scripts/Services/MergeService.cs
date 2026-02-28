using System.Collections.Generic;
using Gameplay.Cube;
using UnityEngine;

namespace Services
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
                if (cube == newCube) continue;
                if (cube.IsMerging) continue;
                if (cube.Value != newCube.Value) continue;

                float distance = Vector3.Distance(cube.transform.position, newCube.transform.position);
                if (distance < 1.1f)
                {
                    newCube.MergeWith(cube);
                    return;
                }
            }
        }
    }
}