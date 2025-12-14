using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Handlers;
using Assets.CoreGame.Scripts.Managers;

namespace Assets.CoreGame.Scripts.StateMachine.Enemy
{
    public class EnemyIdleState : AbsEnemyStateMachine
    {
        public EnemyIdleState(EnemyStateManager enemyStateManager, EnemyMovementController enemyMovementController, WeaponHolderHandler weaponHolderHandler) : base(enemyStateManager, enemyMovementController, weaponHolderHandler)
        {
        }

        public override void Enter()
        {
        }

        public override void Tick()
        {
        }
        public override void OnDrawGizmos()
        {
        }
    }
}