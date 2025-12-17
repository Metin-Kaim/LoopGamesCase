using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Managers
{
    [RequireComponent(typeof(EnemyMovementController))]
    public class EnemyManager : AbsCharacterManager
    {
        EnemyMovementController _enemyMovementController;

        protected override void Awake()
        {
            base.Awake();
            _enemyMovementController = GetComponent<EnemyMovementController>();
        }

        protected override void OnGameEnded()
        {
            base.OnGameEnded();
            _enemyMovementController.SetCanMove(false);
        }

        protected override void HitReaction(Vector3 hitDirection)
        {
            _enemyMovementController.SetCanMove(false);
            PlayAnimation(hitDirection, () => { if (!healthController.IsDead) _enemyMovementController.SetCanMove(true); });
        }

        protected override void OnDie()
        {
            base.OnDie();

            transform.DOScale(0.2f, .5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

            for (int i = 0; i < 5; i++)
            {
                GameObject bubble = PoolSignals.Instance.onGetItemFromPool.Invoke(PoolType.SwordBubble);
                Vector2 rndPos = Random.insideUnitCircle + (Vector2)transform.position;
                bubble.transform.position = rndPos;
            }

            GameSignals.Instance.onEnemyDied.Invoke(gameObject);
        }
    }
}