using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Signals
{
    public class ScratchSignals : MonoBehaviour
    {
        public static ScratchSignals Instance;

        public UnityAction<Vector2> OnScratchAtPosition;

        private void Awake()
        {
            Instance = this;
        }
    }
}