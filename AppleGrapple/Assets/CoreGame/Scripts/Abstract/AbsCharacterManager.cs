using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

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

        protected abstract void HitReaction(Vector3 hitDirection);

        private void Die()
        {
            // Optional: Add death effect
            Destroy(gameObject);
        }
    }
}