using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Managers
{
    [RequireComponent(typeof(PlayerMovementController))]
    public class PlayerManager : AbsCharacterManager
    {
        PlayerMovementController _playerMovementController;

        protected override void Awake()
        {
            base.Awake();
            _playerMovementController = GetComponent<PlayerMovementController>();
        }

        private void OnEnable()
        {
            GameSignals.Instance.onGameEnded += OnGameEnded;
        }

        protected override void OnGameEnded()
        {
            base.OnGameEnded();
            _playerMovementController.SetCanMove(false);
        }

        private void OnDisable()
        {
            GameSignals.Instance.onGameEnded -= OnGameEnded;
        }

        protected override void HitReaction(Vector3 hitDirection)
        {
            _playerMovementController.SetCanMove(false);
            PlayAnimation(hitDirection, () => { _playerMovementController.SetCanMove(true); });
        }

        protected override void OnDie()
        {
            base.OnDie();

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DORotate(new Vector3(0, 0, 360), 3, RotateMode.FastBeyond360));
            seq.Join(transform.DOScale(3f, 3f));
            seq.AppendInterval(.5f);
            seq.JoinCallback(() =>
            {
                SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in spriteRenderers)
                {
                    renderer.sortingOrder = 0;
                    renderer.DOFade(0, 10f).SetEase(Ease.Linear);
                }
                GetComponentInChildren<CanvasGroup>().DOFade(0,10).SetEase(Ease.Linear);
            });
            seq.AppendInterval(11f);
            seq.AppendCallback(() =>
            {
                gameObject.SetActive(false);
            });

            GameSignals.Instance.onGameEnded.Invoke();
        }
    }
}