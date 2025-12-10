using Assets.CoreGame.Scripts.Signals;
using System.Collections;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class ProceduralLevelGenerator : MonoBehaviour
    {
        [Header("Ground Settings")]
        public int groundWidth = 15;
        public int groundHeight = 15;
        public float tileSize = 1f;
        public WeightedTile[] groundTiles;

        [Header("Boundary Settings")]
        public int boundaryWidth = 7;
        public int boundaryHeight = 7;
        public GameObject boundaryPrefab;

        void Awake()
        {
            GenerateGround();
            GenerateBoundary();
        }
        private void OnEnable()
        {
            GameSignals.Instance.GetGameAreaBoundary += OnGetGameAreaBoundary;
        }
        private Vector2 OnGetGameAreaBoundary()
        {
            return new Vector2(boundaryWidth * tileSize, boundaryHeight * tileSize);
        }
        private void OnDisable()
        {
            GameSignals.Instance.GetGameAreaBoundary -= OnGetGameAreaBoundary;
        }

        private void GenerateBoundary()
        {
            GameObject boundaryHolder = new GameObject("Boundary");
            boundaryHolder.transform.parent = transform;
            boundaryHolder.transform.localPosition = Vector3.zero;

            float halfW = (boundaryWidth - 1) * tileSize / 2f;
            float halfH = (boundaryHeight - 1) * tileSize / 2f;

            for (int x = 0; x < boundaryWidth; x++)
            {
                float posX = x * tileSize - halfW;

                Vector3 topPos = new Vector3(posX, halfH, 0f);
                Instantiate(boundaryPrefab, topPos, Quaternion.identity, boundaryHolder.transform);

                Vector3 bottomPos = new Vector3(posX, -halfH, 0f);
                Instantiate(boundaryPrefab, bottomPos, Quaternion.identity, boundaryHolder.transform);
            }

            for (int y = 1; y < boundaryHeight - 1; y++)
            {
                float posY = y * tileSize - halfH;

                Vector3 leftPos = new Vector3(-halfW, posY, 0f);
                Instantiate(boundaryPrefab, leftPos, Quaternion.identity, boundaryHolder.transform);

                Vector3 rightPos = new Vector3(halfW, posY, 0f);
                Instantiate(boundaryPrefab, rightPos, Quaternion.identity, boundaryHolder.transform);
            }
        }


        void GenerateGround()
        {
            GameObject groundHolder = new GameObject("Ground");
            groundHolder.transform.parent = transform;
            groundHolder.transform.localPosition = Vector3.zero;

            float totalWeight = 0f;
            foreach (var tile in groundTiles)
                totalWeight += tile.weight;

            float halfW = (groundWidth - 1) * tileSize / 2f;
            float halfH = (groundHeight - 1) * tileSize / 2f;

            for (int x = 0; x < groundWidth; x++)
            {
                for (int y = 0; y < groundHeight; y++)
                {
                    GameObject prefab = GetWeightedRandomPrefab(totalWeight);
                    Vector3 pos = new Vector3(x * tileSize - halfW, y * tileSize - halfH, 0f);
                    Instantiate(prefab, pos, Quaternion.identity, groundHolder.transform);
                }
            }
        }

        GameObject GetWeightedRandomPrefab(float totalWeight)
        {
            float r = Random.value * totalWeight;
            float cumulative = 0f;

            foreach (var tile in groundTiles)
            {
                cumulative += tile.weight;
                if (r <= cumulative)
                    return tile.prefab;
            }

            // Fallback
            return groundTiles[groundTiles.Length - 1].prefab;
        }

        [System.Serializable]
        public class WeightedTile
        {
            public GameObject prefab;
            public float weight;
        }
    }
}