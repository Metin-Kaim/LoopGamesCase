using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField] private byte health;
        [SerializeField] private float middleLayerAnimationDelay = 0.3f;
        [SerializeField] private float middleLayerAnimationDuration = 0.3f;

        [SerializeField] private Image healtImageFront;
        [SerializeField] private Image healtImageMiddle;

        private bool _isDead;
        private byte _maxHealth;
        private Tweener _healthTween;

        public bool IsDead => _isDead;

        private void Awake()
        {
            _maxHealth = health;
        }

        public virtual void DecreaseHealth()
        {
            health--;
            healtImageFront.fillAmount = health / (float)_maxHealth;

            _healthTween?.Kill();
            _healthTween = healtImageMiddle.DOFillAmount(healtImageFront.fillAmount, middleLayerAnimationDuration).SetDelay(middleLayerAnimationDelay);

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _isDead = true;
        }
    }
}