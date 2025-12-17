using Assets.CoreGame.Scripts.Signals;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject endCard;
        [SerializeField] bool showJoystick;
        [SerializeField] private Image[] joystickImages;

        private void Awake()
        {
            foreach (var image in joystickImages)
            {
                image.enabled = showJoystick;
            }
        }

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