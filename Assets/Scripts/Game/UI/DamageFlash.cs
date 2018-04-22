using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Game.UI
{
    public class DamageFlash : MonoBehaviour
    {
        private const float FadeOpacity = 0.5f;

        private Tweener _tweener;

        private void Start()
        {
            var graphic = GetComponent<Graphic>();
            MessageManager.ReceiveEvent<PlayerDamagedEvent>().Subscribe(ev =>
            {
                if (_tweener != null)
                {
                    _tweener.Kill(true);
                }

                var col = graphic.color;
                col.a = FadeOpacity;
                graphic.color = col;
                _tweener = graphic.DOFade(0f, 0.25f).SetEase(Ease.Linear);
            });
        }
    }
}
