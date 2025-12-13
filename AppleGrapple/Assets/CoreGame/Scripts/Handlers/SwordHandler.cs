using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Signals;
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

                if (_weaponHolder.CompareTag("Player"))
                {
                    CameraSignals.Instance.onCameraShake?.Invoke();
                }

                _weaponHolder.DecreaseSword(this);
            }
            else if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
            {
                if (_weaponHolder.gameObject == collision.gameObject) return;

                if(_weaponHolder.CompareTag("Enemy") && collision.CompareTag("Player") || _weaponHolder.CompareTag("Player") && collision.CompareTag("Enemy"))
                {
                    CameraSignals.Instance.onCameraShake?.Invoke();
                }

                collision.GetComponent<AbsCharacterManager>().TakeDamage(transform);
            }
        }
    }
}