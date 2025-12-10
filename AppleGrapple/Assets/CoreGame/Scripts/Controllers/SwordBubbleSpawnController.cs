using Assets.CoreGame.Scripts.Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class SwordBubbleSpawnController : MonoBehaviour
    {
        [SerializeField] private GameObject swordBubble;
        [SerializeField] private short initialBubbleCountInPool = 10;

        [SerializeField] private List<GameObject> swordBubbles;

        private void Awake()
        {
            //Initialize pool of sword bubbles
            swordBubbles = new List<GameObject>();
            for (int i = 0; i < initialBubbleCountInPool; i++)
            {
                GameObject bubble = Instantiate(swordBubble, transform);
                OnSwordBubbleCollected(bubble);
            }
        }

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

                if (swordBubbles.Count > 0)
                {
                    GameObject bubble = swordBubbles[0];
                    swordBubbles.RemoveAt(0);
                    bubble.transform.position = spawnPos;
                    bubble.SetActive(true);
                }
                else
                    Instantiate(swordBubble, spawnPos, Quaternion.identity, transform);
            }
        }

        private void OnEnable()
        {
            SwordBubbleSignals.Instance.onSwordBubbleCollected += OnSwordBubbleCollected;
        }
        private void OnDisable()
        {
            SwordBubbleSignals.Instance.onSwordBubbleCollected -= OnSwordBubbleCollected;
        }

        private void OnSwordBubbleCollected(GameObject bubble)
        {
            bubble.SetActive(false);
            swordBubbles.Add(bubble);
        }
    }
}