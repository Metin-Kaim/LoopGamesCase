using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Handlers;
using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Abstract
{
    public abstract class AbsCharacterManager : MonoBehaviour
    {
        [SerializeField] private DOTweenAnimation hitMaskTween;

        protected HealthController healthController;

        private Collider2D _collider;
        private SpriteRenderer[] _spriteRenderers;
        private WeaponHolderHandler _weaponHolderHandler;
        private SwordBubbleTriggerController _swordBubbleTriggerController;

        protected virtual void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            healthController = GetComponent<HealthController>();
            _weaponHolderHandler = GetComponent<WeaponHolderHandler>();
            _collider = GetComponent<Collider2D>();
            _swordBubbleTriggerController = GetComponentInChildren<SwordBubbleTriggerController>();
        }
        private void OnEnable()
        {
            GameSignals.Instance.onGameEnded += OnGameEnded;
        }

        protected virtual void OnGameEnded()
        {
            _weaponHolderHandler.StopRotation();
            _collider.enabled = false;
        }

        private void OnDisable()
        {
            GameSignals.Instance.onGameEnded -= OnGameEnded;

        }
        public virtual void TakeDamage(Transform hitPoint)
        {
            healthController.DecreaseHealth();

            Vector3 hitDirection = (transform.position - hitPoint.position).normalized;
            HitReaction(hitDirection);

            if (healthController.IsDead)
                OnDie();
        }

        protected virtual void OnDie()
        {
            _collider.enabled = false;
            _swordBubbleTriggerController.gameObject.SetActive(false);
            _weaponHolderHandler.ClearSwords();
        }

        protected abstract void HitReaction(Vector3 hitDirection);

        protected void PlayAnimation(Vector3 hitDirection, UnityAction onComplete)
        {
            if (!hitMaskTween.gameObject.activeSelf)
                hitMaskTween.gameObject.SetActive(true);
            hitMaskTween.DORestart();

            transform.DOKill();

            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.DOKill(true);

            Vector2 targetPos = GameSignals.Instance.onSetThePositionWithinTheBoundaries.Invoke(transform.position + hitDirection * .8f);
            transform.DOMove(targetPos, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.DOColor(Color.white, 0.2f).From().SetEase(Ease.Linear);
        }
    }
}