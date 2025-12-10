using Assets.CoreGame.Scripts.Signals;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Handlers
{
    public class PlayerHandler : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("SwordBubble"))
                SwordBubbleSignals.Instance.onSwordBubbleCollected?.Invoke(collision.gameObject);
        }
    }
}