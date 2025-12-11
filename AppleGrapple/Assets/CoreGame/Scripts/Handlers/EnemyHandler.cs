using UnityEngine;

namespace Assets.CoreGame.Scripts.Handlers
{
    [RequireComponent(typeof(WeaponHolderHandler))]
    public class EnemyHandler : MonoBehaviour
    {
        WeaponHolderHandler weaponHolderHandler;

        private void Awake()
        {
            weaponHolderHandler = GetComponent<WeaponHolderHandler>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("SwordBubble"))
                weaponHolderHandler.SwordBubbleCollected(collision.gameObject);
        }
    }
}