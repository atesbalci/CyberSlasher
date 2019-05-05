using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using Game.Models;
using Helpers;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Engine
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] _leftPositionsRaw;
        [SerializeField] private Transform[] _rightPositionsRaw;

        private BossPool _bossPool;
        private MinionPool _minionPool;
        private Vector3[] _leftPositions;
        private Vector3[] _rightPositions;
        
        [Inject]
        public void Initialize(BossPool bossPool, MinionPool minionPool)
        {
            _bossPool = bossPool;
            _minionPool = minionPool;
            _leftPositions = _leftPositionsRaw.Select(x => x.position).ToArray();
            _rightPositions = _rightPositionsRaw.Select(x => x.position).ToArray();
            MessageManager.ReceiveEvent<PlayerDamagedEvent>().Subscribe(ev =>
            {
                if (ev.Source.Type == EnemyType.Boss)
                {
                    _bossPool.Despawn(ev.Source);
                }
                else
                {
                    _minionPool.Despawn(ev.Source);
                }
            });
        }

        public void Spawn(EnemyPath path, bool boss, float duration)
        {
            var p = path == EnemyPath.Left ? _leftPositions : _rightPositions;
            var enemy = boss ? _bossPool.Spawn() : _minionPool.Spawn();
            enemy.transform.rotation = Quaternion.identity;
            enemy.transform.position = p[0];
            enemy.Type = boss ? EnemyType.Boss : EnemyType.Minion;
            enemy.Path = path;
            var tweener = new TweenerCore<Vector3, Path, PathOptions>[1];
            tweener[0] = enemy.transform.DOPath(p, duration, PathType.CatmullRom).OnUpdate(() =>
            {
                tweener[0].timeScale = GameTime.Scale;
                if (enemy.IsDead || !enemy.gameObject.activeSelf)
                {
                    tweener[0].Kill();
                }
            }).OnComplete(() =>
            {
                if (boss)
                {
                    _bossPool.Despawn(enemy);
                }
                else
                {
                    _minionPool.Despawn(enemy);
                }
            });
        }
    }
    
    public class BossPool : MonoMemoryPool<Enemy> { }
    public class MinionPool : MonoMemoryPool<Enemy> { }
}
