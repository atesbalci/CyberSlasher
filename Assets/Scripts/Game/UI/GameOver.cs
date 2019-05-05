using Game.Engine;
using Helpers;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameOver : MonoBehaviour
    {
        private GameObject _panel;

        private void Start()
        {
            _panel = transform.GetChild(0).gameObject;
            _panel.SetActive(false);
            MessageManager.ReceiveEvent<PlayerDeadEvent>().SubscribeOn(Scheduler.MainThreadEndOfFrame)
                .Subscribe(ev => _panel.SetActive(true)).AddTo(gameObject);
            GetComponent<Button>().onClick
                .AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        }
    }
}