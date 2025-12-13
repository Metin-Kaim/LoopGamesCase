using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Signals
{
    public class CameraSignals : MonoBehaviour
    {
        public static CameraSignals Instance;

        public UnityAction onCameraShake;

        private void Awake()
        {
            Instance = this;
        }
    }
}