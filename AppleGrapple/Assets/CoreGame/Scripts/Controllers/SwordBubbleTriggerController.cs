using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Handlers;
using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class SwordBubbleTriggerController : MonoBehaviour
    {
        [SerializeField] WeaponHolderHandler weaponHolderHandler;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out SwordBubbleHandler handler) && handler._weaponHolderHandler == null)
            {
                collision.transform.DOMove(transform.position, 0.1f).OnComplete(() =>
                {
                    weaponHolderHandler.SwordBubbleCollected();
                    PoolSignals.Instance.onReturnItemToPool.Invoke(PoolType.SwordBubble, collision.gameObject);
                });
                collision.transform.DOScale(.3f, 0.1f);
            }
        }
    }
}