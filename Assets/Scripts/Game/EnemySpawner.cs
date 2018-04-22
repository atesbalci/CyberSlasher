using System.Linq;
using DG.Tweening;
using UnityEngine;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Spawn(true, false);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Spawn(false, false);
            }
        }

        public void Spawn(bool left, bool boss)
        {
            var path = left ? _leftPositions : _rightPositions;
            var enemy = Instantiate(boss ? _bossPrefab : _minionPrefab, path[0], Quaternion.identity);
            enemy.transform.DOPath(path, 5f, PathType.CatmullRom);
        }
    }
}
