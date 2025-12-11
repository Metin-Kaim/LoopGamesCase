using Assets.CoreGame.Scripts.Abstract;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Handlers
{
    public class SwordHandler : MonoBehaviour
    {
        WeaponHolderHandler _weaponHolder;

        private void OnDisable()
        {
            _weaponHolder = null;
        }

        public void Init(WeaponHolderHandler weaponHolder)
        {
            _weaponHolder = weaponHolder;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_weaponHolder == null) return;

            if (collision.CompareTag("Sword"))
            {
                if (_weaponHolder == collision.GetComponent<SwordHandler>()._weaponHolder)
                    return;

                _weaponHolder.DecreaseSword(this);
            }
            else if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
            {
                if (_weaponHolder.gameObject == collision.gameObject) return;

                collision.GetComponent<AbsCharacterManager>().TakeDamage(transform);
            }
        }
    }
}