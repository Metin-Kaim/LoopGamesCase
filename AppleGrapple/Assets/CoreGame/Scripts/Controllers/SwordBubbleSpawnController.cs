using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Signals;
using System.Collections;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class SwordBubbleSpawnController : MonoBehaviour
    {
        private bool _canSpawn = true;

        private void OnEnable()
        {
            GameSignals.Instance.onGameEnded += () => _canSpawn = false;
        }
        private void OnDisable()
        {
            GameSignals.Instance.onGameEnded -= () => _canSpawn = false;
        }

        private IEnumerator Start()
        {
            while (_canSpawn)
            {
                float waitTime = Random.Range(.2f, 3f);
                yield return new WaitForSeconds(waitTime);

                Vector2 spawnPos = GameSignals.Instance.GetRandomPointInGameBoundary.Invoke();

                GameObject bubble = PoolSignals.Instance.onGetItemFromPool.Invoke(PoolType.SwordBubble);
                bubble.transform.position = spawnPos;
            }
        }
    }
}