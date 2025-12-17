using Assets.CoreGame.Scripts.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class EnemySpawnController : MonoBehaviour
    {
        [SerializeField] private List<EnemySpawnInfo> enemySpawnInfos;

        private List<GameObject> _enemyList;

        private void OnEnable()
        {
            GameSignals.Instance.onEnemyDied += OnEnemyDied;
        }
        private void OnDisable()
        {
            GameSignals.Instance.onEnemyDied -= OnEnemyDied;
        }

        private void Start()
        {
            _enemyList = new List<GameObject>();

            foreach (var enemyInfo in enemySpawnInfos)
            {
                for (int i = 0; i < enemyInfo.count; i++)
                {
                    Vector2 rndPos;
                    do
                    {
                        rndPos = GameSignals.Instance.onGetRandomPointInGameBoundary.Invoke();

                    } while (PlayerSignals.Instance.onGetIsPointCloseToThePlayer.Invoke(rndPos));


                    GameObject enemy = Instantiate(enemyInfo.enemyPrefab, rndPos, Quaternion.identity, transform);

                    _enemyList.Add(enemy);
                }
            }
        }

        public void OnEnemyDied(GameObject enemy)
        {
            _enemyList.Remove(enemy);

            if (_enemyList.Count == 0)
            {
                GameSignals.Instance.onGameEnded.Invoke();
            }
        }

    }

    [System.Serializable]
    public struct EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public byte count;
    }
}