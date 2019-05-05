using Game.Engine;
using Game.Models;
using UnityEngine;
using Zenject;

namespace Game.Injection
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Player _player;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _minionPrefab;
        [SerializeField] private GameObject _bossPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<SlashStateData>().AsSingle();
            Container.BindInstance(_player).AsSingle();
            Container.BindMemoryPool<Enemy, BossPool>().WithInitialSize(5).FromComponentInNewPrefab(_bossPrefab);
            Container.BindMemoryPool<Enemy, MinionPool>().WithInitialSize(10).FromComponentInNewPrefab(_minionPrefab);
        }
    }
}