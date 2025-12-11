using Assets.CoreGame.Scripts.Controllers;
using Assets.CoreGame.Scripts.Handlers;
using Assets.CoreGame.Scripts.Managers;

namespace Assets.CoreGame.Scripts.Abstract
{
    public abstract class AbsEnemyStateMachine
    {
        protected EnemyStateManager enemyStateManager;
        protected EnemyMovementController enemyMovementController;
        protected WeaponHolderHandler weaponHolderHandler;

        protected AbsEnemyStateMachine(EnemyStateManager enemyStateManager, EnemyMovementController enemyMovementController, WeaponHolderHandler weaponHolderHandler)
        {
            this.enemyStateManager = enemyStateManager;
            this.enemyMovementController = enemyMovementController;
            this.weaponHolderHandler = weaponHolderHandler;
        }

        public abstract void Enter();
        public abstract void Tick();
        public abstract void Exit();

        public abstract void OnDrawGizmos();
    }
}