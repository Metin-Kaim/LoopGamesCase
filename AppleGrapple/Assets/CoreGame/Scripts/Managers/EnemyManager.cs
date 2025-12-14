using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
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

        protected override void HitReaction(Vector3 hitDirection)
        {
            _enemyMovementController.SetCanMove(false);
            PlayAnimation(hitDirection, () => { _enemyMovementController.SetCanMove(true); });
        }
    }
}