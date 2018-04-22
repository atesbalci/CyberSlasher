using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Utility;

namespace Game
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Enemy _minionPrefab;
        [SerializeField] private Enemy _bossPrefab;
        [SerializeField] private Transform[] _leftPositionsRaw;
        [SerializeField] private Transform[] _rightPositionsRaw;

        private Vector3[] _leftPositions;
        private Vector3[] _rightPositions;

        private void Start()
        {
            _leftPositions = _leftPositionsRaw.Select(x => x.position).ToArray();
            _rightPositions = _rightPositionsRaw.Select(x => x.position).ToArray();
        }

        public void Spawn(EnemyPath path, bool boss, float duration)
        {
            var p = path == EnemyPath.Left ? _leftPositions : _rightPositions;
            var enemy = Instantiate(boss ? _bossPrefab : _minionPrefab, p[0], Quaternion.identity);
            enemy.Path = path;
            var tweener = new TweenerCore<Vector3, Path, PathOptions>[1];
            tweener[0] = enemy.transform.DOPath(p, duration, PathType.CatmullRom).OnUpdate(() =>
            {
                tweener[0].timeScale = GameTime.Scale;
                if (enemy.IsDead)
                {
                    tweener[0].Kill();
                }
            }).OnComplete(() => Destroy(enemy.gameObject));
        }
    }
}
