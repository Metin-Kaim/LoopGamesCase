using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Handlers;
using Assets.CoreGame.Scripts.Managers;
using System.Linq;
using UnityEngine;

namespace Assets.CoreGame.Scripts.StateMachine.Enemy
{
    public class EnemyEscapeState : AbsEnemyStateMachine
    {
        WeaponHolderHandler _target;
        float _timer;
        float _checkInterval = 0.2f;
        private float detectionRadius = 4f;

        Vector3 escapePosition;

        public EnemyEscapeState(EnemyStateManager enemyStateManager, EnemyMovementController enemyMovementController, WeaponHolderHandler weaponHolderHandler) : base(enemyStateManager, enemyMovementController, weaponHolderHandler)
        { }

        public override void Enter()
        {
            _target = enemyStateManager.Target;
            if (_target == null)
                enemyStateManager.ChangeState(EnemyStateType.Wander);

            SetEscapePosition();
        }

        public override void Tick()
        {
            //keep moving away from target
            _timer += Time.deltaTime;
            if (_timer > _checkInterval)
            {
                _timer = 0;

                //Check et enemy'leri
                Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyMovementController.transform.position, detectionRadius, enemyMovementController.CharacterLayers);

                Collider2D checkedTarget = colliders.FirstOrDefault(c => c.GetComponent<WeaponHolderHandler>() == _target);

                if (checkedTarget == null) // target kayboldu
                {
                    enemyStateManager.Target = null;
                    enemyStateManager.ChangeState(EnemyStateType.Wander);
                    return;
                }

                Collider2D[] bubbles = Physics2D.OverlapCircleAll(enemyMovementController.transform.position, detectionRadius, enemyMovementController.BubbleLayer);

                if (_target.SwordCount < weaponHolderHandler.SwordCount)
                {
                    enemyStateManager.ChangeState(EnemyStateType.Chase);
                    return;
                }

                SetEscapePosition();
            }
        }
        private void SetEscapePosition()
        {
            Vector3 directionAwayFromTarget = (enemyMovementController.transform.position - _target.transform.position).normalized;
            escapePosition = enemyMovementController.transform.position + directionAwayFromTarget * 3 + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
            enemyMovementController.SetTargetPosition(escapePosition);
        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(enemyMovementController.transform.position, detectionRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(escapePosition, .3f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(enemyMovementController.transform.position, escapePosition);
        }
    }
}