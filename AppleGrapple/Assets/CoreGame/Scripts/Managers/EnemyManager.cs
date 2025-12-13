using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Managers
{
    public class EnemyManager : AbsCharacterManager
    {
        EnemyMovementController _enemyMovementController;

        private void Awake()
        {
            _enemyMovementController = GetComponent<EnemyMovementController>();
        }

        protected override void HitReaction(Vector3 hitDirection)
        {
            _enemyMovementController.CanMove = false;
            transform.DOKill();
            transform.DOMove(visual.position + hitDirection * .8f, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _enemyMovementController.CanMove = true;
            });
        }
    }
}