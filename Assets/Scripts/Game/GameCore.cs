using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class GameCore : MonoBehaviour
    {
        private void Awake()
        {
            DOTween.Init();
        }
    }
}
