using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Signals;
using System.Collections;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class SwordBubbleSpawnController : MonoBehaviour
    {
        private IEnumerator Start()
        {
            Vector2 gameAreaBoundary = GameSignals.Instance.GetGameAreaBoundary.Invoke();

            while (true)
            {
                float waitTime = Random.Range(.2f, 3f);
                yield return new WaitForSeconds(waitTime);

                float posX = Random.Range(-gameAreaBoundary.x / 2f + 2, gameAreaBoundary.x / 2f - 2);
                float posY = Random.Range(-gameAreaBoundary.y / 2f + 2, gameAreaBoundary.y / 2f - 2);

                Vector3 spawnPos = new Vector3(posX, posY, 0f);

                GameObject bubble = PoolSignals.Instance.onGetItemFromPool.Invoke(PoolType.SwordBubble);
                bubble.transform.position = spawnPos;
            }
        }
    }
}