using DG.Tweening;
using UnityEngine;
using Utility;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private const float BossPossibility = 0.3f;
        private const float SpawnInterval = 1f;
        private const float SpawnIntervalVariance = 1f;
        private const float EnemyRunDuration = 2.5f;

        private Player _player;
        private EnemySpawner _enemySpawner;
        private float _timeUntilNextSpawn;

        private void Awake()
        {
            DOTween.Init();
            _player = FindObjectOfType<Player>();
            _enemySpawner = FindObjectOfType<EnemySpawner>();
            _timeUntilNextSpawn = 2f;
        }

        private void Update()
        {
            if (_player.Health < 0.001f)
            {
                MessageManager.SendEvent(new PlayerDeadEvent());
            }

            _timeUntilNextSpawn -= GameTime.DeltaTime;
            if (_timeUntilNextSpawn <= 0f)
            {
                _enemySpawner.Spawn(Random.value < 0.5f ? EnemyPath.Left : EnemyPath.Right, Random.value < BossPossibility, EnemyRunDuration);
                _timeUntilNextSpawn = SpawnInterval + Random.Range(-1f, 1f) * SpawnIntervalVariance;
            }
        }
    }

    public class PlayerDeadEvent : GameEvent { }
}
