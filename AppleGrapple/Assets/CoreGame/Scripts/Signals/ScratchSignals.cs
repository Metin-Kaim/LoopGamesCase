using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Signals
{
    public class ScratchSignals : MonoBehaviour
    {
        public static ScratchSignals Instance;

        public UnityAction<Vector2> OnScratchAtPosition;
        public UnityAction<Vector2, float> OnScratchAtPositionWithRadius;

        private void Awake()
        {
            Instance = this;
        }
    }
}