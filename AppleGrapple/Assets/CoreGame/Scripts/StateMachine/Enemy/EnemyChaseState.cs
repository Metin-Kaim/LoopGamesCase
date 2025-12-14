using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Handlers;
using Assets.CoreGame.Scripts.Managers;
using System.Linq;
using UnityEngine;

namespace Assets.CoreGame.Scripts.StateMachine.Enemy
{
    public class EnemyChaseState : AbsEnemyStateMachine
    {
        WeaponHolderHandler _target;
        float _timer;
        float _checkInterval = 0.2f;
        private float detectionRadius = 4f;

        public EnemyChaseState(EnemyStateManager enemyStateManager, EnemyMovementController enemyMovementController, WeaponHolderHandler weaponHolderHandler) : base(enemyStateManager, enemyMovementController, weaponHolderHandler)
        { }

        public override void Enter()
        {
            _target = enemyStateManager.Target;

            if (_target == null)
                enemyStateManager.ChangeState(EnemyStateType.Wander);

            enemyMovementController.SetTargetPosition(_target.transform.position);
        }
        public override void Tick()
        {
            if (_target != null)
            {
                _timer += Time.deltaTime;
                if (_timer >= _checkInterval)
                {
                    //Check et enemy'leri
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyMovementController.transform.position, detectionRadius, enemyMovementController.CharacterLayers);

                    Collider2D checkedTarget = colliders.FirstOrDefault(c => c.GetComponent<WeaponHolderHandler>() == _target);

                    if (checkedTarget == null) // target kayboldu
                    {
                        enemyStateManager.Target = null;
                        enemyStateManager.ChangeState(EnemyStateType.Wander);
                        return;
                    }

                    _timer = 0f;

                    if (_target.SwordCount >= weaponHolderHandler.SwordCount)
                    {
                        enemyStateManager.ChangeState(EnemyStateType.Escape);
                        return;
                    }

                    enemyMovementController.SetTargetPosition(_target.transform.position, 0.6f);
                }
            }
            else
            {
                enemyStateManager.ChangeState(EnemyStateType.Wander);
            }
        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(enemyMovementController.transform.position, detectionRadius);

            if (enemyStateManager.Target == null) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(enemyStateManager.Target.transform.position, 0.3f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(enemyMovementController.transform.position, enemyStateManager.Target.transform.position);
        }
    }
}