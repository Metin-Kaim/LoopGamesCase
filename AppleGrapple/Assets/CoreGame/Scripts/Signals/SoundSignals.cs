using Assets.CoreGame.Scripts.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Signals
{
    public class SoundSignals : MonoBehaviour
    {
        public static SoundSignals Instance;

        public UnityAction<SoundType> onPlaySoundByType;

        private void Awake()
        {
            Instance = this;
        }
    }
}