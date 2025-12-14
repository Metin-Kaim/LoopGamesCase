using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Handlers
{
    public class SwordHandler : MonoBehaviour
    {
        private static int swordIndex = 0;

        [SerializeField] private float throwForce = 5f;
        [SerializeField] private float throwScaleValue = .9f;

        int _swordIndex;
        WeaponHolderHandler _weaponHolder;
        Collider2D _weaponCollider;

        private void Awake()
        {
            _weaponCollider = GetComponent<Collider2D>();
        }

        private void OnDisable()
        {
            _weaponHolder = null;
        }

        public void Init(WeaponHolderHandler weaponHolder)
        {
            _weaponHolder = weaponHolder;
            _swordIndex = swordIndex;
            swordIndex++;
        }

        public void ThrowItAway(Vector2 direction)
        {
            _weaponCollider.enabled = false;
            // Detach from weapon holder and move it towards the direction with DOTween
            transform.parent = null;

            transform.DOLocalMove(transform.up * throwForce, 2f).OnComplete(() =>
            {
                PoolSignals.Instance.onReturnItemToPool?.Invoke(PoolType.Sword, gameObject);
                _weaponCollider.enabled = true;
            });
            transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360);
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

                if (_swordIndex > collision.GetComponent<SwordHandler>()._swordIndex)
                {
                    GameObject swordHitVFX = PoolSignals.Instance.onGetItemFromPool.Invoke(PoolType.SwordHitVFX);
                    swordHitVFX.transform.position = collision.transform.position;
                }

                _weaponHolder.DecreaseSword(this);
                ThrowItAway((collision.transform.position - transform.position).normalized);

            }
            else if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
            {
                if (_weaponHolder.gameObject == collision.gameObject) return;

                if (_weaponHolder.CompareTag("Enemy") && collision.CompareTag("Player") || _weaponHolder.CompareTag("Player") && collision.CompareTag("Enemy"))
                {
                    CameraSignals.Instance.onCameraShake?.Invoke();
                }

                collision.GetComponent<AbsCharacterManager>().TakeDamage(transform);
            }
        }
    }
}