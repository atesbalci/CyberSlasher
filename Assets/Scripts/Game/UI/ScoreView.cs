using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private Text _text;

        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        private void Update()
        {
            _text.text = Mathf.FloorToInt(_gameManager.Score).ToString();
        }
    }
}
