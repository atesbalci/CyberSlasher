using Game.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Views
{
    public class SlashTrailView : MonoBehaviour
    {
        private SlashStateData _slashStateData;

        [Inject]
        public void Initialize(SlashStateData slashStateData)
        {
            _slashStateData = slashStateData;
            _slashStateData.Position.Subscribe(pos => transform.position = pos).AddTo(gameObject);
        }
    }
}