using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Cube;
using Services;
using UnityEngine;

namespace Gameplay
{
    public class AutoMerge
    {
        private readonly MergeSystem _mergeSystem;
        private readonly CubeConfig _cubeConfig;
        private readonly ScoreSystem _scoreSystem;


        public AutoMerge(
            MergeSystem mergeSystem,
            CubeConfig cubeConfig, 
            ScoreSystem scoreSystem)
        {
            _mergeSystem = mergeSystem;
            _cubeConfig = cubeConfig;
            _scoreSystem = scoreSystem;
        }

        public bool TryFindMergePair(out CubeView a, out CubeView b, CubeView exclude = null)
        {
            a = b = null;
            var cubes = _mergeSystem.Cubes
                .Where(c => c != exclude)
                .ToList();

            var group = cubes
                .GroupBy(c => c.Value)
                .FirstOrDefault(g => g.Count() >= 2);

            if (group == null) return false;

            var list = group.ToList();
            a = list[0];
            b = list[1];
            return true;
        }

        public async UniTask ExecuteAsync(CubeView a, CubeView b)
        {
            float riseHeight = 5f;
            float swingBack = 1.5f;
            float duration = 0.4f;

            a.SetKinematic(true);
            b.SetKinematic(true);

            Vector3 aMid = a.transform.position + Vector3.up * riseHeight;
            Vector3 bMid = b.transform.position + Vector3.up * riseHeight;

            await UniTask.WhenAll(
                DOTweenToUniTask(a.transform.DOMove(aMid, duration)),
                DOTweenToUniTask(b.transform.DOMove(bMid, duration))
            );

            Vector3 dir = (bMid - aMid).normalized;
            await UniTask.WhenAll(
                DOTweenToUniTask(a.transform.DOMove(aMid - dir * swingBack, duration * 0.5f)),
                DOTweenToUniTask(b.transform.DOMove(bMid + dir * swingBack, duration * 0.5f))
            );

            Vector3 mergePoint = (aMid + bMid) / 2f;
            await UniTask.WhenAll(
                DOTweenToUniTask(a.transform.DOMove(mergePoint, duration)),
                DOTweenToUniTask(b.transform.DOMove(mergePoint, duration))
            );

            int newValue = a.Value * 2;
            _mergeSystem.UnregisterCube(b);
            UnityEngine.Object.Destroy(b.gameObject);

            a.Init(newValue, _cubeConfig);
            _scoreSystem.AddScore(newValue);
            a.PlayMergeAnimation();
            a.SetKinematic(false);

            _mergeSystem.WakeUpAll();
        }

        private static UniTask DOTweenToUniTask(Tween tween)
        {
            var utcs = new UniTaskCompletionSource();
            tween.OnComplete(() => utcs.TrySetResult());
            return utcs.Task;
        }
    }
}