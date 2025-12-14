using Assets.CoreGame.Scripts.Signals;
using System.Collections;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject endCard;

        private void OnEnable()
        {
            GameSignals.Instance.onGameEnded += OnGameEnded;
        }
        private void OnGameEnded()
        {
            endCard.SetActive(true);
        }
        private void OnDisable()
        {
            GameSignals.Instance.onGameEnded -= OnGameEnded;
        }
    }
}