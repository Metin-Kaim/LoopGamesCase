using UnityEngine;

namespace Assets.CoreGame.Scripts.Handlers
{
    public class SwordBubbleHandler : MonoBehaviour
    {
        [HideInInspector] public WeaponHolderHandler _weaponHolderHandler;

        private void OnDisable()
        {
            _weaponHolderHandler = null;
        }
        private void OnEnable()
        {
            transform.localScale = Vector3.one;
        }
    }
}