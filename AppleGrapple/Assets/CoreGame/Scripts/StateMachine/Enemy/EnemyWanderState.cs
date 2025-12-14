using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Handlers;
using Assets.CoreGame.Scripts.Managers;
using UnityEngine;

namespace Assets.CoreGame.Scripts.StateMachine.Enemy
{
    public class EnemyWanderState : AbsEnemyStateMachine
    {
        private bool _isBubbleFound;
        private float detectionRadius = 4f;
        private float _timer;
        private float _checkInterval = .5f;

        public EnemyWanderState(EnemyStateManager enemyStateManager, EnemyMovementController enemyMovementController, WeaponHolderHandler weaponHolderHandler) : base(enemyStateManager, enemyMovementController, weaponHolderHandler)
        { }

        public override void Enter()
        {
            _isBubbleFound = false;

            enemyMovementController.SetNewRandomPosition();
        }

        public override void Tick()
        {
            if (enemyMovementController.IsStopped)
            {
                _isBubbleFound = false;
                enemyMovementController.SetNewRandomPosition();
            }

            if (!_isBubbleFound)
            {
                _timer += Time.deltaTime;
                if (_timer >= _checkInterval)
                {
                    _timer = 0f;
                    CheckForBubbles();
                    CheckForEnemies();
                }
            }
        }

        private void CheckForEnemies()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyMovementController.transform.position, detectionRadius, enemyMovementController.CharacterLayers);

            foreach (var collider in colliders)
            {
                if(collider.gameObject == enemyMovementController.gameObject) continue;

                if (collider.TryGetComponent(out WeaponHolderHandler otherWeaponHandler))
                {
                    enemyStateManager.Target = otherWeaponHandler;
                    if (otherWeaponHandler.SwordCount >= weaponHolderHandler.SwordCount)
                    {
                        enemyStateManager.ChangeState(EnemyStateType.Escape);
                    }
                    else
                    {
                        enemyStateManager.ChangeState(EnemyStateType.Chase);
                    }
                    return;
                }
            }
        }
        private void CheckForBubbles()
        {
            Collider2D collider = Physics2D.OverlapCircle(enemyMovementController.transform.position, detectionRadius, enemyMovementController.BubbleLayer);
            if (collider == null) return;

            _isBubbleFound = true;
            enemyMovementController.SetTargetPosition(collider.transform.position);
        }


        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(enemyMovementController.transform.position, detectionRadius);
        }
    }
}