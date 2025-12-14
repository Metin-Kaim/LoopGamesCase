using Assets.CoreGame.Scripts.Signals;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Handlers
{
    public class SwordHitVFXListener : MonoBehaviour
    {
        private void OnParticleSystemStopped()
        {
            PoolSignals.Instance.onReturnItemToPool.Invoke(Enums.PoolType.SwordHitVFX, gameObject);
        }
    }
}