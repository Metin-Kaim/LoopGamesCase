using Assets.CoreGame.Scripts.Controllers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.CoreGame.Scripts.Abstract
{
    public abstract class AbsCharacterManager : MonoBehaviour
    {
        [SerializeField] protected byte health = 3;
        [SerializeField] protected Transform visual;
        [SerializeField] private DOTweenAnimation hitMaskTween;

        private SpriteRenderer[] _spriteRenderers;

        protected virtual void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        public virtual void TakeDamage(Transform hitPoint)
        {
            health--;

            if (health <= 0)
            {
                Die();
            }
            else
            {
                Vector3 hitDirection = (visual.position - hitPoint.position).normalized;
                HitReaction(hitDirection);
            }
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

        private void Die()
        {
            // Optional: Add death effect
            Destroy(gameObject);
        }
    }
}