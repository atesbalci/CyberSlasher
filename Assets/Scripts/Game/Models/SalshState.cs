using UniRx;
using UnityEngine;

namespace Game.Models
{
    public class SlashStateData
    {
        public ReactiveProperty<SlashType> Type { get; }
        public ReactiveProperty<Vector3> Position { get; }

        public SlashStateData()
        {
            Type = new ReactiveProperty<SlashType>();
            Position = new ReactiveProperty<Vector3>();
        }
    }

    public enum SlashType
    {
        Inactive,
        Active
    }
}