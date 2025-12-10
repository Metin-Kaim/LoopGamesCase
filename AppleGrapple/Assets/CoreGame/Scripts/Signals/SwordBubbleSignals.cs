using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Signals
{
    public class SwordBubbleSignals : MonoBehaviour
    {
        public static SwordBubbleSignals Instance;

        public UnityAction<GameObject> onSwordBubbleCollected;

        private void Awake()
        {
            Instance = this;
        }
    }
}