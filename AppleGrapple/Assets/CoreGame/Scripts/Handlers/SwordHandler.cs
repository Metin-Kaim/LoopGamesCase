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

        int _swordIndex;
        WeaponHolderHandler _weaponHolder;
        Collider2D _weaponCollider;
        SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _weaponCollider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnDisable()
        {
            _weaponHolder = null;
            _spriteRenderer.DOFade(1, 0);
        }

        public void Init(WeaponHolderHandler weaponHolder)
        {
            _weaponHolder = weaponHolder;
            _swordIndex = swordIndex;
            swordIndex++;
        }

        public void ThrowItAway()
        {
            _weaponCollider.enabled = false;
            transform.parent = null;

            transform.DOLocalMove(transform.up * throwForce, 2f).OnComplete(() =>
            {
                PoolSignals.Instance.onReturnItemToPool?.Invoke(PoolType.Sword, gameObject);
                _weaponCollider.enabled = true;
            });
            transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360);
            _spriteRenderer.DOFade(0, 1f).SetDelay(1f).SetEase(Ease.InBounce);
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
                    if (PlayerSignals.Instance.onGetIsPointCloseToThePlayer.Invoke(collision.transform.position))
                    {
                        SoundSignals.Instance.onPlaySoundByType.Invoke(SoundType.SwordHit);
                    }
                }

                _weaponHolder.DecreaseSword(this);
                ThrowItAway();

            }
            else if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
            {
                if (_weaponHolder.gameObject == collision.gameObject) return;

                if (_weaponHolder.CompareTag("Enemy") && collision.CompareTag("Player") || _weaponHolder.CompareTag("Player") && collision.CompareTag("Enemy"))
                {
                    CameraSignals.Instance.onCameraShake?.Invoke();
                }
                if (PlayerSignals.Instance.onGetIsPointCloseToThePlayer.Invoke(collision.transform.position))
                {
                    SoundSignals.Instance.onPlaySoundByType.Invoke(SoundType.CharacterHit);
                }

                collision.GetComponent<AbsCharacterManager>().TakeDamage(transform);
            }
        }
    }
}