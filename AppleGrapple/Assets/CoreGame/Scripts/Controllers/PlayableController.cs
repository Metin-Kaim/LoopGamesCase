using Luna.Unity;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class PlayableController : MonoBehaviour
    {
        public void GoToStore()
        {
            LifeCycle.GameEnded();
            Playable.InstallFullGame();
        }
    }
}