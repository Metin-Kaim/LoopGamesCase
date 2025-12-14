using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Handlers;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Abstract
{
    public abstract class AbsCharacterManager : MonoBehaviour
    {
        [SerializeField] protected byte health = 3;
        [SerializeField] protected Transform visual;
        [SerializeField] private DOTweenAnimation hitMaskTween;

        protected Collider2D collider;
        protected HealthController healthController;

        private SpriteRenderer[] _spriteRenderers;
        private WeaponHolderHandler _weaponHolderHandler;

        protected virtual void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            healthController = GetComponent<HealthController>();
            _weaponHolderHandler = GetComponent<WeaponHolderHandler>();
            collider = GetComponent<Collider2D>();
        }

        public virtual void TakeDamage(Transform hitPoint)
        {
            healthController.DecreaseHealth();

            Vector3 hitDirection = (visual.position - hitPoint.position).normalized;
            HitReaction(hitDirection);

            if (healthController.IsDead)
                OnDie();
        }

        protected virtual void OnDie()
        {
            collider.enabled = false;
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

            transform.DOMove(visual.position + hitDirection * .8f, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.DOColor(Color.white, 0.2f).From().SetEase(Ease.Linear);
        }
    }
}