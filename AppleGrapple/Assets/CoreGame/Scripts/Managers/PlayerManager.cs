using Assets.CoreGame.Scripts.Abstract;
using Assets.CoreGame.Scripts.Controllers;
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

        protected override void HitReaction(Vector3 hitDirection)
        {
            _playerMovementController.SetCanMove(false);
            PlayAnimation(hitDirection, () => { _playerMovementController.SetCanMove(true); });
        }

        protected override void OnDie()
        {
            //Open End Card
            base.OnDie();
        }
    }
}