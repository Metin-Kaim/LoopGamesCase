using DG.Tweening;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Abstract
{
    public abstract class AbsCharacterManager : MonoBehaviour
    {
        [SerializeField] protected byte health = 3;
        [SerializeField] protected Transform visual;

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

        private void HitReaction(Vector3 hitDirection)
        {
            visual.DOMove(visual.position + hitDirection * 0.3f, 0.1f).SetLoops(2, LoopType.Yoyo);
        }

        private void Die()
        {
            // Optional: Add death effect
            Destroy(gameObject);
        }
    }
}